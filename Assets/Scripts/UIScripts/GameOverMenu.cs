using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Pantalla de Game Over: aparece al morir el jugador, enseña el ranking online y deja reintentar
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

        // Se enseña el panel que toca y se cambia la musica del nivel por la suya
        if (isVictory)
        {
            victoryPanel.SetActive(true);
            AudioManager.Instance.PlayMusic(AudioManager.Instance.victoryMusic);
        }
        else
        {
            gameOverPanel.SetActive(true);
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameOverMusic);
        }

        // El ranking online se coloca en el recuadro del panel que ha salido
        RankingMenu.Instance.Open(isVictory);
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
