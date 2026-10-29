using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    public TMP_Text healthText;

    private void Start()
    {
        healthText.text = "VIDAS: " + currentHealth + " / " + maxHealth;
    }
    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        healthText.text = "VIDAS: " + currentHealth + " / " + maxHealth;

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }

    }
}
