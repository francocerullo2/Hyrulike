using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Pantalla de Game Over: aparece al morir el jugador, enseña los datos de la partida y deja reintentar
// Va en un objeto que siempre este activo (por ejemplo MenuCanvas), no en el propio panel
public class GameOverMenu : MonoBehaviour
{
    // Es static para que PauseMenu sepa que no tiene que abrirse con Esc estando muerto
    public static bool isGameOver;

    // Igual que StatsManager.Instance: asi el jugador lo encuentra solo en cualquier nivel
    public static GameOverMenu Instance;

    public GameObject gameOverPanel;
    public GameObject rankingPanel; // hueco para el ranking online, lo rellenara el script de Firebase
    public float delay = 1.5f; // tiempo para que se vea la animacion de muerte antes de salir el menu

    public TMP_Text screenText;
    public TMP_Text timeText;
    public TMP_Text enemiesText;
    public TMP_Text upgradesText;

    // Nombres de las escenas, tienen que estar en Build Settings
    public string mainMenuScene = "MainMenu";
    public string firstLevel = "RioPelt_01_Base";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isGameOver = false;
        gameOverPanel.SetActive(false);
    }

    // Lo llama PlayerHealth cuando el jugador se queda sin vida
    public void Show()
    {
        isGameOver = true;
        StartCoroutine(ShowAfterDelay());
    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        // Igual que en la pausa: con isPaused el jugador no se mueve y con timeScale 0 se para todo
        PauseMenu.isPaused = true;
        Time.timeScale = 0;

        UpdateTexts();
        gameOverPanel.SetActive(true);
    }

    private void UpdateTexts()
    {
        StatsManager stats = StatsManager.Instance;

        // El tiempo se enseña en minutos:segundos
        int minutes = (int)(stats.runTime / 60);
        int seconds = (int)(stats.runTime % 60);

        screenText.text = "Nivel: " + stats.screenReached;
        timeText.text = "Tiempo total: " + minutes.ToString("00") + ":" + seconds.ToString("00");
        enemiesText.text = "Enemigos derrotados: " + stats.enemiesKilled;
        upgradesText.text = "Mejoras: " + stats.UpgradesObtained();
    }

    // Empieza una partida nueva desde el primer nivel con todo reiniciado (las permanentes se mantienen)
    public void Retry()
    {
        StatsManager.Instance.ResetRun();
        Unpause();
        SceneManager.LoadScene(firstLevel);
    }

    public void ExitToMenu()
    {
        StatsManager.Instance.ResetRun();
        Unpause();
        SceneManager.LoadScene(mainMenuScene);
    }

    // Si no quitamos la pausa, la siguiente escena empezaria congelada
    private void Unpause()
    {
        isGameOver = false;
        PauseMenu.isPaused = false;
        Time.timeScale = 1;
    }
}
