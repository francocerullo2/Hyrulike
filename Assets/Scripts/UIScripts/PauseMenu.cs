using UnityEngine;
using UnityEngine.SceneManagement;

// Abre y cierra el menu con Esc y tiene las funciones de sus botones
public class PauseMenu : MonoBehaviour
{
    // Es static para que los scripts del jugador puedan mirar si el juego esta en pausa
    public static bool isPaused;

    public GameObject pauseMenu;
    public GameObject settingsPanel;

    // Nombre de la escena del menu principal, tiene que estar en Build Settings
    public string mainMenuScene = "MainMenu";

    private void Start()
    {
        // La partida empieza sin pausa y con el menu cerrado
        Resume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Si estamos en ajustes, Esc vuelve al menu de pausa
            if (settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0; // con 0 se para todo lo que use el tiempo (fisicas, animaciones, corrutinas)
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void ExitToMenu()
    {
        // Si no quitamos la pausa, la siguiente escena empezaria congelada
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene);
    }
}
