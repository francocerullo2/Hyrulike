using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{

    public float speed;
    public float attackRange = 2;
    public float attackCoolDown = 2;
    public float playerDetectRange = 5;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private float attackCoolDownTimer;
    private int facingDirection = -1; // 1 for right, -1 for left
    private EnemyState enemyState;

   
    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);

    }


    // FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here. 
    void FixedUpdate()
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
            // Stop moving when attacking
            rb.linearVelocity = Vector2.zero;
        }
        
    }

    // Chase the player when in range
    void Chase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void CheckForPlayer()
    {
        // Mientras ataca no cambia de estado; el evento de la animación lo devuelve a Idle
        if (enemyState == EnemyState.Attacking)
        {
            return;
        }

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
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
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

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void ChangeState(EnemyState newState)
    {
        // Reset all animation states
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", false);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", false);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", false);

        // Change the enemy state
        enemyState = newState;

        // Set the new animation state
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", true);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", true);


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
    }


}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}