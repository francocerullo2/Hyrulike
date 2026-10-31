using System;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
   public int currentHealth;
   public int maxHealth;

   private void Start()
    {
        // Todos los enemigos empiezan con la vida llena
        currentHealth = maxHealth;

    }

    // Negativo para hacer daño, positivo para curar
    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        // Que no se pase del maximo, y si llega a 0 se muere
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }

    }
}
