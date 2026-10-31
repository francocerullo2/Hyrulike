using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public int facingDirection = 1; // 1 mira a la derecha, -1 a la izquierda

    public Rigidbody2D rb;
    public Animator anim;
    public Player_Combat player_Combat;
    private bool isKnockedBack; // mientras sea true el jugador no se puede mover

    // El input de ataque va en Update y no en FixedUpdate para que no se pierda ninguna pulsacion
    private void Update()
    {
        if(Input.GetButtonDown("Slash"))
        {
            player_Combat.Attack();
        }
    }

    // FixedUpdate va a intervalos fijos y no depende de los fps, aqui va todo lo de fisicas
    void FixedUpdate()
    {
        // Si le estan empujando no leemos el input, que se mueva solo por el knockback
        if (isKnockedBack == false)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Si cambia de direccion damos la vuelta al sprite
            if (horizontal > 0 && transform.localScale.x < 0 || horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }

            // Guarda hacia donde mira el jugador para el ataque (lado, arriba o abajo)
            if (horizontal != 0)
            {
                anim.SetFloat("lastHorizontal", 1);
                anim.SetFloat("lastVertical", 0);
            }
            else if (vertical != 0)
            {
                anim.SetFloat("lastHorizontal", 0);
                anim.SetFloat("lastVertical", vertical);
            }

            // Le pasamos el input al animator para que cambie entre idle y andar
            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));

            // Limita la velocidad diagonal a la misma velocidad que la horizontal o vertical.
            Vector2 movement = new Vector2(horizontal, vertical);
            movement = Vector2.ClampMagnitude(movement, 1f);
            rb.linearVelocity = movement * speed;
        }
    }

    // Da la vuelta al sprite invirtiendo la escala en X
    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(
            transform.localScale.x * -1,
            transform.localScale.y, 
            transform.localScale.z);
    }

    // Empuja al jugador en direccion contraria al enemigo que le ha pegado
    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));

    }

    // Espera el tiempo de stun, frena al jugador y le devuelve el control
    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }
}