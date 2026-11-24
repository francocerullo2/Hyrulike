using UnityEngine;

/// <summary>
/// Barril de TNT: no hace nada hasta que el jugador lo toca o le pega.
/// Entonces enciende la mecha y al rato explota, dañando al jugador si sigue cerca.
/// </summary>
public class Enemy_Explosion : MonoBehaviour
{
    public int damage = 2;
    public float explosionRange = 1.5f;
    public float knockbackForce;
    public float stunTime;
    public float fuseTime = 2; // segundos de mecha antes de explotar
    public float explosionDuration = .8f; // lo que dura la animacion de la explosion
    public Transform explosionPoint;
    public GameObject explosionPrefab;
    public LayerMask playerLayer;
    public AudioClip explosionSound;

    private bool isLit; // para que no se encienda dos veces

    /// <summary>
    /// Enciende la mecha y programa la explosion para dentro de fuseTime segundos.
    /// Lo llama Enemy_Health al recibir un golpe, o el propio barril cuando el jugador lo toca.
    /// </summary>
    public void LightFuse()
    {
        if (isLit)
        {
            return;
        }
        isLit = true;

        // Que el golpe no lo empuje ni le cambie el estado, si no se cortaria la animacion de la mecha
        GetComponent<Enemy_Knockback>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        // El estado Attacking del TNT tiene la animacion de la mecha
        GetComponent<Enemy_Movement>().ChangeState(EnemyState.Attacking);
        Invoke("Explode", fuseTime);
    }

    /// <summary>
    /// Explota: si el jugador esta dentro del radio lo empuja y le quita vida, y el barril desaparece.
    /// Se llama con Invoke desde LightFuse.
    /// </summary>
    private void Explode()
    {
        AudioManager.Instance.PlaySFX(explosionSound);

        // Igual que Enemy_Combat: si el jugador esta dentro del radio lo empuja y le quita vida
        Collider2D[] hits = Physics2D.OverlapCircleAll(explosionPoint.position, explosionRange, playerLayer);

        if (hits.Length > 0)
        {
            hits[0].GetComponent<PlayerMovement>().Knockback(transform, knockbackForce, stunTime);
            hits[0].GetComponent<PlayerHealth>().ChangeHealth(-damage);
        }

        // El efecto de la explosion se borra solo cuando acaba la animacion
        GameObject explosion = Instantiate(explosionPrefab, explosionPoint.position, Quaternion.identity);
        Destroy(explosion, explosionDuration);

        // Lo ha activado el jugador, asi que cuenta como enemigo derrotado en el resumen
        // (pero no para el LevelManager: el TNT no hace falta explotarlo para acabar el nivel)
        StatsManager.Instance.enemiesKilled++;
        Destroy(gameObject);
    }

    /// <summary>
    /// Si el jugador choca con el barril, se enciende.
    /// El collider de la raiz es trigger y el de los pies no, asi que el toque puede llegar por cualquiera de los dos.
    /// </summary>
    /// <param name="collision">El choque que manda Unity.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LightFuse();
        }
    }

    /// <summary>
    /// Lo mismo que OnCollisionEnter2D pero para el collider trigger.
    /// </summary>
    /// <param name="collision">El collider con el que ha entrado.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LightFuse();
        }
    }

    /// <summary>Circulo naranja en el editor para ver hasta donde llega la explosion.</summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, .5f, 0);
        Gizmos.DrawWireSphere(explosionPoint.position, explosionRange);
    }
}
