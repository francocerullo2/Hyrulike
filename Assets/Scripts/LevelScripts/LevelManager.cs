using UnityEngine;

// Uno en cada nivel: cuenta los enemigos y cuando no queda ninguno saca la galleta de la fortuna
// Al recogerla se elige mejora y se pasa a nextLevel
public class LevelManager : MonoBehaviour
{
    // Igual que StatsManager.Instance, pero este no pasa de una escena a otra: cada nivel tiene el suyo
    public static LevelManager Instance;

    public string nextLevel; // nombre de la escena siguiente, tiene que estar en Build Settings
    public GameObject cookiePrefab;

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

        // Si el nivel no tiene enemigos la galleta sale ya, en la posicion de este objeto
        if (enemiesLeft == 0)
        {
            SpawnCookie(transform.position);
        }
    }

    // Lo llaman Enemy_Health y Enemy_Explosion cuando un enemigo muere
    public void EnemyDefeated(Vector3 position)
    {
        enemiesLeft--;

        // El ultimo enemigo deja la galleta donde ha muerto
        if (enemiesLeft == 0)
        {
            SpawnCookie(position);
        }
    }

    private void SpawnCookie(Vector3 position)
    {
        Instantiate(cookiePrefab, position, Quaternion.identity);
    }
}
