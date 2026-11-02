using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Tiempo sin poder recibir daño despues de un golpe
    public float invulnerabilityTime = 1f;
    private float invulnerabilityTimer;

    // Texto de la UI donde se ven las vidas
    public TMP_Text healthText;

    private void Start()
    {
        // Pintamos las vidas nada mas empezar para que no salga el texto vacio
        healthText.text = "VIDAS: " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
    }

    private void Update()
    {
        // Va bajando el contador de invulnerabilidad
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }
    }

    // Sirve para quitar vida (amount negativo) o curar (amount positivo)
    public void ChangeHealth(int amount)
    {
        // Si recibe daño mientras es invulnerable, se ignora
        if (amount < 0)
        {
            if (invulnerabilityTimer > 0)
            {
                return;
            }
            invulnerabilityTimer = invulnerabilityTime;
        }

        // Mantiene la vida entre 0 y el maximo
        StatsManager.Instance.currentHealth = Mathf.Clamp(StatsManager.Instance.currentHealth + amount, 0, StatsManager.Instance.maxHealth);
        healthText.text = "VIDAS: " + StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;

        // Sin vidas = muerto, de momento solo lo desactivamos
        if (StatsManager.Instance.currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }

    }
}
