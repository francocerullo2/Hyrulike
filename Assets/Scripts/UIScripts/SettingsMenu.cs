using TMPro;
using UnityEngine;

// Opciones de volumen y pantalla completa, se usa en la pausa y en el menu principal
public class SettingsMenu : MonoBehaviour
{
    public TMP_Text volumeText;

    // Cada vez que se abre el panel se pinta el volumen actual
    private void OnEnable()
    {
        UpdateVolumeText();
    }

    public void VolumeUp()
    {
        AudioListener.volume = Mathf.Clamp(AudioListener.volume + 0.1f, 0, 1);
        UpdateVolumeText();
    }

    public void VolumeDown()
    {
        AudioListener.volume = Mathf.Clamp(AudioListener.volume - 0.1f, 0, 1);
        UpdateVolumeText();
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    private void UpdateVolumeText()
    {
        volumeText.text = "VOLUMEN: " + Mathf.RoundToInt(AudioListener.volume * 100) + "%";
    }
}
