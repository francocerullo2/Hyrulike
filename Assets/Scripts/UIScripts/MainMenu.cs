using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Botones de la pantalla de inicio: jugar (historia y controles), opciones y salir.
/// </summary>
public class MainMenu : MonoBehaviour
{
    // Escena que se carga al pulsar Empezar, tiene que estar en Build Settings
    public string firstLevel = "Zona1_01";
    public GameObject optionsPanel;
    public GameObject historyPanel;
    public GameObject controlsPanel;

    /// <summary>Empieza con todos los paneles cerrados.</summary>
    private void Start()
    {
        optionsPanel.SetActive(false);
        historyPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    /// <summary>Esc cierra las opciones, la historia y los controles.</summary>
    private void Update()
    {
        // Esc cierra las opciones, la historia y los controles
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseOptions();
            CloseHistory();
        }
    }

    /// <summary>
    /// Boton Jugar: abre la historia. Tambien sirve para el Volver de los controles.
    /// </summary>
    public void Play()
    {
        controlsPanel.SetActive(false);
        historyPanel.SetActive(true);
    }

    /// <summary>Boton Siguiente de la historia: pasa a los controles.</summary>
    public void ShowControls()
    {
        historyPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    /// <summary>Boton Volver de la historia, vuelve al menu.</summary>
    public void CloseHistory()
    {
        historyPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    /// <summary>
    /// Boton Empezar de los controles: reinicia la partida y carga el primer nivel.
    /// </summary>
    public void StartGame()
    {
        // El StatsManager pasa entre escenas, asi que si ya se jugo una partida hay que dejarlo como nuevo
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ResetRun();
        }
        SceneManager.LoadScene(firstLevel);
    }

    /// <summary>Boton Opciones: abre el panel de ajustes.</summary>
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    /// <summary>Cierra el panel de ajustes.</summary>
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    /// <summary>
    /// Boton Salir: cierra el juego. En el editor no cierra nada, solo funciona en el juego compilado.
    /// </summary>
    public void Quit()
    {
        // En el editor no cierra nada, solo funciona en el juego compilado
        Debug.Log("Salir del juego");
        Application.Quit();
    }
}
