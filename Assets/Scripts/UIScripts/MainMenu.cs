using UnityEngine;
using UnityEngine.SceneManagement;

// Botones de la pantalla de inicio
public class MainMenu : MonoBehaviour
{
    // Escena que se carga al pulsar Empezar, tiene que estar en Build Settings
    public string firstLevel = "Zona1_01";
    public GameObject optionsPanel;
    public GameObject historyPanel;
    public GameObject controlsPanel;

    private void Start()
    {
        optionsPanel.SetActive(false);
        historyPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    private void Update()
    {
        // Esc cierra las opciones, la historia y los controles
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseOptions();
            CloseHistory();
        }
    }

    // Jugar abre la historia. Tambien sirve para el Volver de los controles
    public void Play()
    {
        controlsPanel.SetActive(false);
        historyPanel.SetActive(true);
    }

    // Boton Siguiente de la historia
    public void ShowControls()
    {
        historyPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    // Boton Volver de la historia, vuelve al menu
    public void CloseHistory()
    {
        historyPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    // Boton Empezar de los controles
    public void StartGame()
    {
        // El StatsManager pasa entre escenas, asi que si ya se jugo una partida hay que dejarlo como nuevo
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ResetRun();
        }
        SceneManager.LoadScene(firstLevel);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    public void Quit()
    {
        // En el editor no cierra nada, solo funciona en el juego compilado
        Debug.Log("Salir del juego");
        Application.Quit();
    }
}
