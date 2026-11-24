using System;
using UnityEngine;

/// <summary>
/// Vida de los enemigos (tambien del jefe y del TNT).
/// Al morir avisa al LevelManager para saber cuando se ha limpiado el nivel.
/// </summary>
public class Enemy_Health : MonoBehaviour
{
   public int currentHealth;
   public int maxHealth;

   public GameObject deadPrefab; // animacion de muerte (la misma que la del jugador)
   public float deadDuration = 1.4f; // lo que dura la animacion, luego se borra

   private bool isDead; // por si le llegan dos golpes en el mismo frame, que no muera dos veces

   /// <summary>Todos los enemigos empiezan con la vida llena.</summary>
   private void Start()
    {
        // Todos los enemigos empiezan con la vida llena
        currentHealth = maxHealth;

    }

    /// <summary>
    /// Quita o da vida al enemigo.
    /// Antes mira los casos especiales: el jefe no recibe daño en algunas fases y el TNT no muere, se enciende.
    /// Si llega a 0 se muere y avisa al LevelManager.
    /// </summary>
    /// <param name="amount">Negativo para hacer daño, positivo para curar.</param>
    public void ChangeHealth(int amount)
    {
        // El jefe no recibe daño mientras es el monje o mientras se transforma
        Boss_Phases boss = GetComponent<Boss_Phases>();
        if (amount < 0 && boss != null && !boss.CanTakeDamage())
        {
            return;
        }

        // Los explosivos no mueren de un golpe, se encienden
        Enemy_Explosion explosive = GetComponent<Enemy_Explosion>();
        if (amount < 0 && explosive != null)
        {
            explosive.LightFuse();
            return;
        }

        currentHealth += amount;

        // Si le han hecho daño, se pone alerta y va a por el jugador aunque este lejos
        if (amount < 0)
        {
            GetComponent<Enemy_Movement>().Alert();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.hitSound);
        }

        // El jefe mira si ha bajado de la mitad de vida para pasar a la fase 2
        if (boss != null)
        {
            boss.CheckPhase(currentHealth, maxHealth);
        }

        // Que no se pase del maximo, y si llega a 0 se muere
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if(currentHealth <= 0 && !isDead)
        {
            isDead = true;
            StatsManager.Instance.enemiesKilled++;
            LevelManager.Instance.EnemyDefeated();

            // Deja la animacion de muerte en su sitio y el enemigo desaparece
            GameObject dead = Instantiate(deadPrefab, transform.position, Quaternion.identity);
            Destroy(dead, deadDuration);
            Destroy(gameObject);
        }

    }
}
