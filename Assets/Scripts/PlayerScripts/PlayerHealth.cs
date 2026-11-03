using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Tiempo sin poder recibir daño despues de un golpe
    public float invulnerabilityTime = 1f;
    private float invulnerabilityTimer;

    public Player_Guard player_Guard;
    public PlayerMovement playerMovement;

    // Lo que pasa al morir
    public GameObject deadPrefab; // animacion de muerte (Sprites/Dead)
    public float deadDuration = 1.4f; // lo que dura la animacion, luego se borra

    private void Start()
    {
        // Pintamos los corazones nada mas empezar
        HeartsUI.Instance.UpdateHearts(StatsManager.Instance.currentHealth, StatsManager.Instance.MaxHealth());
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
    // attacker es quien pega, para saber si el escudo lo para. Si es null (curas, explosiones) no se puede bloquear
    public void ChangeHealth(int amount, Transform attacker = null)
    {
        // Si recibe daño mientras es invulnerable, se ignora
        if (amount < 0)
        {
            // Durante el dash esquiva cualquier golpe, tambien las explosiones
            if (playerMovement.isDashing)
            {
                return;
            }

            // Golpe parado con el escudo: no quita vida
            if (attacker != null && player_Guard.BlocksHit(attacker.position))
            {
                return;
            }

            if (invulnerabilityTimer > 0)
            {
                return;
            }
            invulnerabilityTimer = invulnerabilityTime + StatsManager.Instance.InvulnerabilityBonus(); // con la mejora Armadura dura mas
        }

        // Mantiene la vida entre 0 y el maximo
        StatsManager.Instance.currentHealth = Mathf.Clamp(StatsManager.Instance.currentHealth + amount, 0, StatsManager.Instance.MaxHealth());
        HeartsUI.Instance.UpdateHearts(StatsManager.Instance.currentHealth, StatsManager.Instance.MaxHealth());

        // Con la mejora Amuleto no muere: revive con 2 corazones, un rato invulnerable, y el amuleto se gasta
        if (StatsManager.Instance.currentHealth <= 0 && StatsManager.Instance.HasAmulet())
        {
            StatsManager.Instance.upgradeLevels[StatsManager.AMULET] = 0;
            StatsManager.Instance.currentHealth = 2;
            invulnerabilityTimer = 2;
            HeartsUI.Instance.UpdateHearts(StatsManager.Instance.currentHealth, StatsManager.Instance.MaxHealth());
            return;
        }

        // Sin vidas = muerto: deja la animacion de muerte en su sitio, desaparece y sale el Game Over
        if (StatsManager.Instance.currentHealth <= 0)
        {
            GameObject dead = Instantiate(deadPrefab, transform.position, Quaternion.identity);
            Destroy(dead, deadDuration);
            gameObject.SetActive(false);
            GameOverMenu.Instance.Show();
        }

    }
}
