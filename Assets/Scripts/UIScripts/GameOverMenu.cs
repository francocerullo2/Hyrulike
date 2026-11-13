using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Pantalla de Game Over: aparece al morir el jugador, enseña los datos de la partida y deja reintentar
// Tambien enseña la pantalla de victoria al matar al jefe: es un duplicado del panel con otros colores
// Va en un objeto que siempre este activo (por ejemplo MenuCanvas), no en el propio panel
public class GameOverMenu : MonoBehaviour
{
    // Es static para que PauseMenu sepa que no tiene que abrirse con Esc estando muerto
    public static bool isGameOver;

    // Igual que StatsManager.Instance: asi el jugador lo encuentra solo en cualquier nivel
    public static GameOverMenu Instance;

    public GameObject gameOverPanel;
    public float delay = 1.5f; // tiempo para que se vea la animacion de muerte antes de salir el menu

    public GameObject victoryPanel; // copia de gameOverPanel con su propio titulo y colores

    // Para saber si la partida acabo ganando o perdiendo (lo usa tambien el ranking)
    public bool isVictory;

    // Textos del panel de Game Over
    public TMP_Text screenText;
    public TMP_Text timeText;
    public TMP_Text enemiesText;
    public TMP_Text upgradesText;

    // Los mismos textos, pero los del panel de victoria
    public TMP_Text victoryScreenText;
    public TMP_Text victoryTimeText;
    public TMP_Text victoryEnemiesText;
    public TMP_Text victoryUpgradesText;

    // Nombres de las escenas, tienen que estar en Build Settings
    public string mainMenuScene = "MainMenu";
    public string firstLevel = "Zona1_01";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isGameOver = false;
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
    }

    // Lo llama PlayerHealth cuando el jugador se queda sin vida
    public void Show()
    {
        // Si ya esta saliendo el panel (por ejemplo, morir justo despues de ganar) no se vuelve a abrir
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        isVictory = false;
        StartCoroutine(ShowAfterDelay());
    }

    // Lo llama LevelManager cuando se derrota al ultimo enemigo del nivel final
    public void ShowVictory()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        isVictory = true;
        StartCoroutine(ShowAfterDelay());
    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        // Igual que en la pausa: con isPaused el jugador no se mueve y con timeScale 0 se para todo
        PauseMenu.isPaused = true;
        Time.timeScale = 0;

        // Se rellenan los textos del panel que toca y se enseña ese panel
        if (isVictory)
        {
            UpdateTexts(victoryScreenText, victoryTimeText, victoryEnemiesText, victoryUpgradesText);
            victoryPanel.SetActive(true);
        }
        else
        {
            UpdateTexts(screenText, timeText, enemiesText, upgradesText);
            gameOverPanel.SetActive(true);
        }

        // El ranking online se coloca en el recuadro del panel que ha salido
        RankingMenu.Instance.Open(isVictory);
    }

    // Recibe los textos para que sirva igual para el panel de Game Over que para el de victoria
    private void UpdateTexts(TMP_Text screen, TMP_Text time, TMP_Text enemies, TMP_Text upgrades)
    {
        StatsManager stats = StatsManager.Instance;

        // El tiempo se enseña en minutos:segundos
        int minutes = (int)(stats.runTime / 60);
        int seconds = (int)(stats.runTime % 60);

        screen.text = "Nivel: " + stats.screenReached;
        time.text = "Tiempo total: " + minutes.ToString("00") + ":" + seconds.ToString("00");
        enemies.text = "Enemigos derrotados: " + stats.enemiesKilled;
        upgrades.text = "Mejoras: " + stats.UpgradesObtained();
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
        isVictory = false;
        PauseMenu.isPaused = false;
        Time.timeScale = 1;
    }
}
