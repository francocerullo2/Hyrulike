using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int facingDirection = 1; // 1 mira a la derecha, -1 a la izquierda

    public Rigidbody2D rb;
    public Animator anim;
    public Player_Combat player_Combat;
    public Player_Guard player_Guard;
    private bool isKnockedBack; // mientras sea true el jugador no se puede mover
    public bool isShooting; // lo activa Player_Bow mientras dispara, para que no se mueva
    public bool isGuarding; // lo activa Player_Guard mientras se cubre, para que no se mueva

    [Header("Dash")]
    public float dashDuration = .15f; // lo que dura el impulso
    public float dashCooldown = .8f; // tiempo hasta poder volver a hacer dash
    public GameObject dashEffect; // polvo que sale al hacer dash (opcional)
    public AudioClip dashSound;
    public bool isDashing; // mientras sea true no se le puede hacer daño ni empujar
    private float dashTimer;

    [Header("Pasos")]
    public AudioClip stepSound;
    public float stepInterval = .3f; // segundos entre una pisada y la siguiente
    public float stepVolume = .5f;
    private float stepTimer;


    // El input de ataque va en Update y no en FixedUpdate para que no se pierda ninguna pulsacion
    private void Update()
    {
        // En pausa no se puede atacar
        if (PauseMenu.isPaused)
        {
            return;
        }

        // Mientras anda suena una pisada cada cierto tiempo (en el dash y cuando le empujan no)
        stepTimer -= Time.deltaTime;
        if (rb.linearVelocity.sqrMagnitude > .1f && isDashing == false && isKnockedBack == false && stepTimer <= 0)
        {
            AudioManager.Instance.PlaySFX(stepSound, stepVolume);
            stepTimer = stepInterval;
        }

        // En guardia o durante el dash no se puede atacar
        if(Input.GetButtonDown("Slash") && isGuarding == false && isDashing == false)
        {
            player_Combat.Attack();
        }

        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
        }

        // El dash se tiene desde el principio: solo hace falta que el cooldown haya acabado y no este haciendo otra cosa
        if (Input.GetButtonDown("Dash") && dashTimer <= 0
            && isDashing == false && isShooting == false && isGuarding == false && isKnockedBack == false
            && anim.GetBool("isAttacking") == false)
        {
            StartCoroutine(Dash());
        }
    }

    // Impulso corto y rapido hacia donde se esta moviendo (o hacia donde mira si esta quieto)
    IEnumerator Dash()
    {
        isDashing = true;
        AudioManager.Instance.PlaySFX(dashSound);
        dashTimer = dashCooldown * StatsManager.Instance.DashCooldownMultiplier(); // con la mejora Niebla se recarga antes

        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (direction == Vector2.zero)
        {
            direction = new Vector2(Mathf.Sign(transform.localScale.x), 0);
        }
        rb.linearVelocity = direction.normalized * StatsManager.Instance.DashSpeed();

        if (dashEffect != null)
        {
            Destroy(Instantiate(dashEffect, transform.position, Quaternion.identity), 1);
        }

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }

    // FixedUpdate va a intervalos fijos y no depende de los fps, aqui va todo lo de fisicas
    void FixedUpdate()
    {
        // Durante el dash la velocidad la pone Dash(), no se lee el input
        if (isDashing == true)
        {
            return;
        }

        // Si le estan empujando no leemos el input, que se mueva solo por el knockback
        if (isShooting == true)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if (isGuarding == true && isKnockedBack == false)
        {
            // En guardia no se mueve, pero si se puede girar para elegir que lado cubre el escudo
            rb.linearVelocity = Vector2.zero;
            anim.SetFloat("horizontal", 0);
            anim.SetFloat("vertical", 0);

            float horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal > 0 && transform.localScale.x < 0 || horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }
        }
        else if (isKnockedBack == false)
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
            rb.linearVelocity = movement * StatsManager.Instance.Speed();
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
        // Durante el dash esquiva los golpes, no le empujan
        if (isDashing)
        {
            return;
        }

        // Si el escudo para el golpe, el empujon es mas flojo
        if (player_Guard.BlocksHit(enemy.position))
        {
            force *= player_Guard.BlockedKnockbackMultiplier();
        }

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