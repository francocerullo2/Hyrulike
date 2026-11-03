using UnityEngine;
using UnityEngine.SceneManagement;

// Botones de la pantalla de inicio
public class MainMenu : MonoBehaviour
{
    // Escena que se carga al pulsar Jugar, tiene que estar en Build Settings
    public string firstLevel = "RioPelt_01_Base";
    public GameObject optionsPanel;

    private void Start()
    {
        optionsPanel.SetActive(false);
    }

    private void Update()
    {
        // Esc cierra las opciones
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseOptions();
        }
    }

    public void Play()
    {
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
