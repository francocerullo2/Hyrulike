using UnityEngine;

/// <summary>
/// IA de los enemigos con una maquina de estados (Idle, Chasing, Attacking, Knockback).
/// Segun donde este el jugador decide si patrullar, perseguir o atacar, y pone la animacion de cada estado.
/// La usan todos los enemigos y el jefe; cada uno se configura con los campos del Inspector.
/// </summary>
public class Enemy_Movement : MonoBehaviour
{

    public float speed;
    public float attackRange = 2; // a esta distancia deja de perseguir y ataca
    public float attackCoolDown = 2;
    public float playerDetectRange = 5; // radio en el que "ve" al jugador
    public Transform detectionPoint;
    public LayerMask playerLayer;
    public bool patrol = true;          // si va de un punto a otro cuando no ve al jugador
    public bool chasePlayer = true;     // si persigue al jugador al verlo (el lancero si, el torch y el arquero no)
    public Vector2[] patrolPoints;      // si se deja vacio, el enemigo no patrulla y se queda quieto como antes
    public float pauseDuration = 1.5f;  // lo que espera en cada punto
    public float alertDuration = 5;     // segundos que va a por el jugador despues de recibir un golpe
    public float alertRangeMultiplier = 3; // cuanto mas lejos "ve" al jugador mientras esta alerta
    public bool canFlip = true;         // el jefe no se gira porque sus sprites son de frente
    public bool hasHitAnimation = false; // solo el jefe tiene animacion de recibir golpe (parametro isHit en su animator)

    private float alertTimer;

    private int currentPatrolIndex;
    private float pauseTimer;
    private bool isPatrolling; // la patrulla usa el estado Chasing para la animacion de andar, esto distingue si persigue o patrulla
    private float attackCoolDownTimer;
    private EnemyState enemyState;


    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    /// <summary>Coge los componentes y empieza en Idle.</summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);

    }


    /// <summary>
    /// Cada paso de fisicas decide que hacer y baja los contadores.
    /// FixedUpdate va a intervalos fijos y no depende de los fps, aqui va lo de fisicas.
    /// </summary>
    void FixedUpdate()
    {
        // Si esta siendo empujado no hace nada, ya se encarga Enemy_Knockback
        if (enemyState != EnemyState.Knockback)
        {
            CheckForPlayer();

            if(attackCoolDownTimer > 0)
            {
                attackCoolDownTimer -= Time.deltaTime;
            }

            if (alertTimer > 0)
            {
                alertTimer -= Time.deltaTime;
            }

            // Si patrulla, la velocidad ya la pone Patrol()
            if (enemyState == EnemyState.Chasing && !isPatrolling)
            {
                Chase();
            }
            else if (enemyState == EnemyState.Attacking)
            {
                // Mientras ataca se queda quieto
                rb.linearVelocity = Vector2.zero;
            }
        }
        
        
    }

    /// <summary>Va directo hacia el jugador.</summary>
    void Chase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    /// <summary>
    /// Aqui se decide que hace el enemigo segun donde este el jugador:
    /// si esta cerca ataca (si el cooldown lo deja), si lo ve lo persigue, y si no patrulla o se queda quieto.
    /// </summary>
    private void CheckForPlayer()
    {
        // Mientras ataca no cambia de estado; el evento de la animación lo devuelve a Idle
        if (enemyState == EnemyState.Attacking)
        {
            return;
        }

        isPatrolling = false;

        // Busca al jugador dentro del rango de deteccion (mas grande si le acaban de pegar)
        float detectRange = playerDetectRange;
        if (alertTimer > 0)
        {
            detectRange *= alertRangeMultiplier;
        }
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, detectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;

            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                FacePlayer();
                rb.linearVelocity = Vector2.zero;

                // Comprueba si el cooldown de ataque esta listo
                if (attackCoolDownTimer <= 0)
                {
                    attackCoolDownTimer = attackCoolDown;
                    SetAttackDirection();
                    ChangeState(EnemyState.Attacking);
                }
                else
                {
                    // En rango pero esperando el cooldown: se queda quieto mirando al jugador
                    ChangeState(EnemyState.Idle);
                }
            }
            else if (chasePlayer)
            {
                // Lo ve pero esta lejos, a perseguirlo
                FacePlayer();
                ChangeState(EnemyState.Chasing);
            }
            else
            {
                // Lo ve pero no es de los que persiguen, sigue a lo suyo
                PatrolOrIdle();
            }
        }
        else
        {
            // No hay nadie cerca
            PatrolOrIdle();
        }
    }

    /// <summary>
    /// Si tiene la patrulla activada y puntos puestos patrulla, si no se queda parado.
    /// </summary>
    private void PatrolOrIdle()
    {
        if (patrol && patrolPoints.Length > 0)
        {
            isPatrolling = true;
            Patrol();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }

    /// <summary>
    /// Lo llama Enemy_Health al recibir daño: durante un rato ve al jugador desde mucho mas lejos y va a por el.
    /// Asi si le disparas con el arco desde fuera de su rango no se queda quieto.
    /// </summary>
    public void Alert()
    {
        alertTimer = alertDuration;
    }

    /// <summary>
    /// Elige la animacion de ataque (lado, arriba o abajo) segun donde este el jugador.
    /// La izquierda ya la cubre el Flip, por eso attackX siempre es 1 o 0.
    /// Se usan valores redondos para que el blend tree no mezcle dos animaciones y pegue dos veces.
    /// </summary>
    private void SetAttackDirection()
    {
        Vector2 direction = player.position - transform.position;

        // Se le da preferencia a arriba/abajo: cuando esta pegado al jugador la diferencia de altura es muy pequeña
        // (solo chocan los pies) y si no casi siempre elegiria el ataque de lado
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x) * 0.5f)
        {
            anim.SetFloat("attackX", 0);
            anim.SetFloat("attackY", Mathf.Sign(direction.y));
        }
        else
        {
            anim.SetFloat("attackX", 1);
            anim.SetFloat("attackY", 0);
        }
    }

    /// <summary>Gira al enemigo si el jugador esta en el lado contrario.</summary>
    private void FacePlayer()
    {
        FaceTarget(player.position);
    }

    /// <summary>
    /// Gira al enemigo si el punto al que va esta en el lado contrario.
    /// Los sprites miran a la derecha, asi que escala positiva = derecha y negativa = izquierda (igual que en PlayerMovement).
    /// </summary>
    /// <param name="target">El punto al que tiene que mirar.</param>
    private void FaceTarget(Vector2 target)
    {
        // El jefe tiene sprites de frente: si se girase, la katana izquierda pasaria a la mano derecha
        if (!canFlip)
        {
            return;
        }

        if (target.x > transform.position.x && transform.localScale.x < 0 ||
            target.x < transform.position.x && transform.localScale.x > 0)
        {
            Flip();
        }
    }

    /// <summary>
    /// Igual que en el jugador, invierte la escala en X para girar el sprite.
    /// </summary>
    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// Cambia de estado y actualiza los bools del Animator para que se vea la animacion que toca.
    /// </summary>
    /// <param name="newState">El estado al que pasa.</param>
    public void ChangeState(EnemyState newState)
    {
        // Apagamos la animacion del estado en el que estaba
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", false);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", false);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", false);
        else if (enemyState == EnemyState.Knockback && hasHitAnimation)
            anim.SetBool("isHit", false);

        // Cambiamos al estado nuevo
        enemyState = newState;

        // Y encendemos la animacion del estado nuevo
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", true);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", true);
        else if (enemyState == EnemyState.Knockback && hasHitAnimation)
            anim.SetBool("isHit", true);


    }

    /// <summary>Va al punto que toca; al llegar espera un rato y pasa al siguiente.</summary>
    private void Patrol()
    {
        if (pauseTimer > 0)
        {
            pauseTimer -= Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
            return;
        }

        Vector2 target = patrolPoints[currentPatrolIndex];
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;
        ChangeState(EnemyState.Chasing); // reutiliza la animacion de andar
        FaceTarget(target);

        if (Vector2.Distance(transform.position, target) < .1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            pauseTimer = pauseDuration;
        }
    }


    /// <summary>Dibuja el rango de deteccion y los puntos de patrulla en el editor.</summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);

        // Puntos de patrulla en azul para verlos en la escena
        Gizmos.color = Color.blue;
        foreach (Vector2 point in patrolPoints)
        {
            Gizmos.DrawWireSphere(point, .2f);
        }
    }


}

/// <summary>
/// Los estados posibles del enemigo: quieto, persiguiendo (o patrullando), atacando y empujado.
/// </summary>
public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Knockback
}