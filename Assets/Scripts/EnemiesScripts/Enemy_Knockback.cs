using System.Collections;
using UnityEngine;

/// <summary>
/// Empuja y aturde al enemigo cuando recibe un golpe.
/// Mientras esta aturdido Enemy_Movement no lo mueve (estado Knockback).
/// </summary>
public class Enemy_Knockback : MonoBehaviour
{
    public float pushMultiplier = 1; // lo que se le empuja (0 = solo se aturde y no se mueve, como el jefe)
    public float stunCooldown = 0;   // tiempo minimo entre dos aturdimientos (el jefe lo usa para que no se le pueda bloquear sin parar)

    private float stunCooldownTimer;

    private Rigidbody2D rb;
    private Enemy_Movement enemy_Movement;

    /// <summary>Coge el Rigidbody2D y el Enemy_Movement del enemigo.</summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_Movement = GetComponent<Enemy_Movement>();
    }

    /// <summary>Va bajando el tiempo que falta para poder aturdirlo otra vez.</summary>
    private void Update()
    {
        if (stunCooldownTimer > 0)
        {
            stunCooldownTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Empuja al enemigo lejos del jugador cuando recibe un golpe.
    /// </summary>
    /// <param name="playerTransform">Quien pega (el jugador o su flecha), para saber hacia donde empujar.</param>
    /// <param name="knockbackForce">La fuerza del empujon.</param>
    /// <param name="knockbackTime">Segundos que sale volando.</param>
    /// <param name="stunTime">Segundos que se queda aturdido despues.</param>
    public void Knockback(Transform playerTransform, float knockbackForce, float knockbackTime, float stunTime)
    {
        // Desactivar el componente no impide que otros scripts llamen a este metodo, asi que se comprueba a mano
        // (por ejemplo el TNT con la mecha encendida lo desactiva para que el golpe no corte la animacion)
        if (!enabled)
        {
            return;
        }

        // El jefe no se aturde mientras es el monje o mientras se transforma
        Boss_Phases boss = GetComponent<Boss_Phases>();
        if (boss != null && !boss.CanTakeDamage())
        {
            return;
        }

        // Si le aturdieron hace poco, el golpe le quita vida pero no le para
        if (stunCooldownTimer > 0)
        {
            return;
        }
        stunCooldownTimer = stunCooldown;


        // Lo ponemos en estado Knockback para que Enemy_Movement no lo mueva mientras tanto
        enemy_Movement.ChangeState(EnemyState.Knockback);
        StartCoroutine(StunTimer(knockbackTime,stunTime));
        Vector2 direction = (transform.position - playerTransform.position).normalized;
        rb.linearVelocity = direction * knockbackForce * pushMultiplier;
    }

    /// <summary>
    /// Primero sale volando un rato, luego se para y se queda aturdido, y despues vuelve a Idle.
    /// </summary>
    /// <param name="knockbackTime">Segundos que sale volando.</param>
    /// <param name="stunTime">Segundos que se queda aturdido.</param>
    IEnumerator StunTimer(float knockbackTime, float stunTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        enemy_Movement.ChangeState(EnemyState.Idle);
    }
}
