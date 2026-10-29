using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.XR;

public class Enemy_Movement : MonoBehaviour
{

    public float speed;
    private int facingDirection = -1; // 1 for right, -1 for left

    private EnemyState enemyState;

    public float attackRange = 2;
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
        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange)
        { 
            ChangeState(EnemyState.Attacking);
        }
        else if (player.position.x > transform.position.x && facingDirection == -1 ||
            player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player == null)
            {
                player = collision.transform;
            }
            ChangeState(EnemyState.Chasing);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {   
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
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
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}