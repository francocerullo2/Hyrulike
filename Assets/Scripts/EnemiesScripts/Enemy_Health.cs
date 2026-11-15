using System;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
   public int currentHealth;
   public int maxHealth;

   public GameObject deadPrefab; // animacion de muerte (la misma que la del jugador)
   public float deadDuration = 1.4f; // lo que dura la animacion, luego se borra

   private bool isDead; // por si le llegan dos golpes en el mismo frame, que no muera dos veces

   private void Start()
    {
        // Todos los enemigos empiezan con la vida llena
        currentHealth = maxHealth;

    }

    // Negativo para hacer daño, positivo para curar
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
