using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Abre y cierra el menu de pausa con Esc y tiene las funciones de sus botones.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// Es static para que los scripts del jugador puedan mirar si el juego esta en pausa.
    /// </summary>
    public static bool isPaused;

    public GameObject pauseMenu;
    public GameObject settingsPanel;

    // Nombre de la escena del menu principal, tiene que estar en Build Settings
    public string mainMenuScene = "MainMenu";

    /// <summary>La partida empieza sin pausa y con el menu cerrado.</summary>
    private void Start()
    {
        // La partida empieza sin pausa y con el menu cerrado
        Resume();
    }

    /// <summary>Con Esc pausa, reanuda o sale de los ajustes, segun donde este.</summary>
    private void Update()
    {
        // Muerto o eligiendo mejora no se puede pausar ni reanudar, ya hay otro menu abierto
        if (GameOverMenu.isGameOver || UpgradeChoiceMenu.isChoosing)
        {
            return;
        }

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

    /// <summary>
    /// Para el juego y abre el menu. Con timeScale 0 se para todo lo que use el tiempo (fisicas, animaciones, corrutinas).
    /// </summary>
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0; // con 0 se para todo lo que use el tiempo (fisicas, animaciones, corrutinas)
        pauseMenu.SetActive(true);
    }

    /// <summary>Boton Continuar: quita la pausa y cierra los menus.</summary>
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        settingsPanel.SetActive(false);
    }

    /// <summary>Boton Ajustes: abre el panel de ajustes.</summary>
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    /// <summary>Cierra el panel de ajustes y vuelve al menu de pausa.</summary>
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Boton Salir al menu: quita la pausa y carga el menu principal.
    /// Si no quitamos la pausa, la siguiente escena empezaria congelada.
    /// </summary>
    public void ExitToMenu()
    {
        // Si no quitamos la pausa, la siguiente escena empezaria congelada
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene);
    }
}
