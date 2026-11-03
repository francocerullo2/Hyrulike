using UnityEngine;

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

    private int currentPatrolIndex;
    private float pauseTimer;
    private bool isPatrolling; // la patrulla usa el estado Chasing para la animacion de andar, esto distingue si persigue o patrulla
    private float attackCoolDownTimer;
    private EnemyState enemyState;


    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    // Start se llama una vez al empezar, antes del primer Update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);

    }


    // FixedUpdate va a intervalos fijos y no depende de los fps, aqui va lo de fisicas
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

    // Va directo hacia el jugador
    void Chase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    // Aqui se decide que hace el enemigo segun donde este el jugador
    private void CheckForPlayer()
    {
        // Mientras ataca no cambia de estado; el evento de la animación lo devuelve a Idle
        if (enemyState == EnemyState.Attacking)
        {
            return;
        }

        isPatrolling = false;

        // Busca al jugador dentro del rango de deteccion
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

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

    // Si tiene la patrulla activada y puntos puestos patrulla, si no se queda parado
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

    // Elige la animacion de ataque (lado, arriba o abajo) segun donde este el jugador
    // La izquierda ya la cubre el Flip, por eso attackX siempre es 1 o 0
    // Se usan valores redondos para que el blend tree no mezcle dos animaciones y pegue dos veces
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

    // Gira al enemigo si el jugador esta en el lado contrario
    private void FacePlayer()
    {
        FaceTarget(player.position);
    }

    // Gira al enemigo si el punto al que va esta en el lado contrario
    // Los sprites miran a la derecha, asi que escala positiva = derecha y negativa = izquierda (igual que en PlayerMovement)
    private void FaceTarget(Vector2 target)
    {
        if (target.x > transform.position.x && transform.localScale.x < 0 ||
            target.x < transform.position.x && transform.localScale.x > 0)
        {
            Flip();
        }
    }

    // Igual que en el jugador, invierte la escala en X para girar el sprite
    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    // Cambia de estado y actualiza los bools del animator para que se vea la animacion que toca
    public void ChangeState(EnemyState newState)
    {
        // Apagamos la animacion del estado en el que estaba
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", false);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", false);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", false);

        // Cambiamos al estado nuevo
        enemyState = newState;

        // Y encendemos la animacion del estado nuevo
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", true);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", true);


    }

    // Va al punto que toca; al llegar espera un rato y pasa al siguiente
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


    // Dibuja el rango de deteccion en el editor
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

// Los estados posibles del enemigo
public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Knockback
}