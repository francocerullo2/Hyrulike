using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public int facingDirection = 1; // 1 for right, -1 for left

    public Rigidbody2D rb;
    public Animator anim;
    public Player_Combat player_Combat;
    private bool isKnockedBack;

    private void Update()
    {
        if(Input.GetButtonDown("Slash"))
        {
            player_Combat.Attack();
        }
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        // Get the input from the player
        if (isKnockedBack == false)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Flip the player sprite if the direction changes
            if (horizontal > 0 && transform.localScale.x < 0 || horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }

            // Set the animator parameters based on the input
            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));

            // Limita la velocidad diagonal a la misma velocidad que la horizontal o vertical.
            Vector2 movement = new Vector2(horizontal, vertical);
            movement = Vector2.ClampMagnitude(movement, 1f);
            rb.linearVelocity = movement * speed;
        }
    }

    // Flip the player sprite when changing direction
    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(
            transform.localScale.x * -1,
            transform.localScale.y, 
            transform.localScale.z);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));

    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }
}