using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{

    public float speed;
    public float attackRange = 2; // a esta distancia deja de perseguir y ataca
    public float attackCoolDown = 2;
    public float playerDetectRange = 5; // radio en el que "ve" al jugador
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private float attackCoolDownTimer;
    private int facingDirection = -1; // 1 mira a la derecha, -1 a la izquierda
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

            if (enemyState == EnemyState.Chasing)
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

        // Busca al jugador dentro del rango de deteccion
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;
            FacePlayer();

            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                rb.linearVelocity = Vector2.zero;

                // Comprueba si el cooldown de ataque esta listo
                if (attackCoolDownTimer <= 0)
                {
                    attackCoolDownTimer = attackCoolDown;
                    ChangeState(EnemyState.Attacking);
                }
                else
                {
                    // En rango pero esperando el cooldown: se queda quieto mirando al jugador
                    ChangeState(EnemyState.Idle);
                }
            }
            else
            {
                // Lo ve pero esta lejos, a perseguirlo
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            // No hay nadie cerca, se queda parado
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }

    // Gira al enemigo si el jugador esta en el lado contrario
    private void FacePlayer()
    {
        if (player.position.x > transform.position.x && facingDirection == -1 ||
            player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }
    }

    // Igual que en el jugador, invierte la escala en X para girar el sprite
    private void Flip()
    {
        facingDirection *= -1;
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

    // Dibuja el rango de deteccion en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
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