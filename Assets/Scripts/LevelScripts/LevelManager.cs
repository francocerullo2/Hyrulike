using UnityEngine;

// Uno en cada nivel: cuenta los enemigos y cuando no queda ninguno abre la salida del nivel
// Al cruzarla se elige mejora y se pasa a nextLevel
// En el nivel final nextLevel se deja vacio: al matar al jefe sale la pantalla de victoria en vez de la salida
public class LevelManager : MonoBehaviour
{
    // Igual que StatsManager.Instance, pero este no pasa de una escena a otra: cada nivel tiene el suyo
    public static LevelManager Instance;

    public string nextLevel; // nombre de la escena siguiente, tiene que estar en Build Settings (vacio en el nivel final)

    private int enemiesLeft;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Se cuentan los enemigos por su Enemy_Health, menos el TNT: es pasivo y no hace falta explotarlo para acabar el nivel
        foreach (Enemy_Health enemy in FindObjectsByType<Enemy_Health>(FindObjectsSortMode.None))
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

    // Lo llama Enemy_Health cuando un enemigo muere
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

    private void OpenExit()
    {
        // La salida se busca sola, asi basta con arrastrar el prefab a la escena
        foreach (LevelExit exit in FindObjectsByType<LevelExit>(FindObjectsSortMode.None))
        {
            exit.Open();
        }
    }
}
