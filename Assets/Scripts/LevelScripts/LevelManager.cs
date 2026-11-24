using UnityEngine;

/// <summary>
/// Uno en cada nivel: cuenta los enemigos y cuando no queda ninguno abre la salida del nivel.
/// Al cruzarla se elige mejora y se pasa a nextLevel.
/// En el nivel final nextLevel se deja vacio: al matar al jefe sale la pantalla de victoria en vez de la salida.
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Igual que StatsManager.Instance, pero este no pasa de una escena a otra: cada nivel tiene el suyo.
    /// </summary>
    public static LevelManager Instance;

    public string nextLevel; // nombre de la escena siguiente, tiene que estar en Build Settings (vacio en el nivel final)

    private int enemiesLeft;

    /// <summary>
    /// Se guarda como Instance para que Enemy_Health y LevelExit lo encuentren.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Cuenta los enemigos del nivel. Si no hay ninguno, abre la salida directamente.
    /// </summary>
    private void Start()
    {
        // Se cuentan los enemigos por su Enemy_Health, menos el TNT: es pasivo y no hace falta explotarlo para acabar el nivel
        foreach (Enemy_Health enemy in FindObjectsByType<Enemy_Health>())
        {
            if (enemy.GetComponent<Enemy_Explosion>() == null)
            {
                enemiesLeft++;
            }
        }

        // Si el nivel no tiene enemigos la salida esta abierta desde el principio
        if (enemiesLeft == 0)
        {
            OpenExit();
        }
    }

    /// <summary>
    /// Resta un enemigo y, si era el ultimo, abre la salida o, en el nivel final, enseña la victoria.
    /// Lo llama Enemy_Health cuando un enemigo muere.
    /// </summary>
    public void EnemyDefeated()
    {
        enemiesLeft--;

        if (enemiesLeft == 0)
        {
            // Sin nivel siguiente es que era el nivel final: se ha ganado la partida
            if (nextLevel == "")
            {
                GameOverMenu.Instance.ShowVictory();
            }
            else
            {
                OpenExit();
            }
        }
    }

    /// <summary>
    /// Abre todas las salidas del nivel. Se buscan solas, asi basta con arrastrar el prefab a la escena.
    /// </summary>
    private void OpenExit()
    {
        // La salida se busca sola, asi basta con arrastrar el prefab a la escena
        foreach (LevelExit exit in FindObjectsByType<LevelExit>())
        {
            exit.Open();
        }
    }
}
