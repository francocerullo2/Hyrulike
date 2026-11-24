using TMPro;
using UnityEngine;

/// <summary>
/// Opciones de volumen y pantalla completa, se usa en la pausa y en el menu principal.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    public TMP_Text volumeText;

    /// <summary>Cada vez que se abre el panel se pinta el volumen actual.</summary>
    private void OnEnable()
    {
        UpdateVolumeText();
    }

    /// <summary>Boton +: sube el volumen general un 10% (como mucho 100%).</summary>
    public void VolumeUp()
    {
        AudioListener.volume = Mathf.Clamp(AudioListener.volume + 0.1f, 0, 1);
        UpdateVolumeText();
    }

    /// <summary>Boton -: baja el volumen general un 10% (como poco 0%).</summary>
    public void VolumeDown()
    {
        AudioListener.volume = Mathf.Clamp(AudioListener.volume - 0.1f, 0, 1);
        UpdateVolumeText();
    }

    /// <summary>Boton de pantalla completa: la pone o la quita.</summary>
    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    /// <summary>Escribe el volumen en porcentaje.</summary>
    private void UpdateVolumeText()
    {
        volumeText.text = "VOLUMEN: " + Mathf.RoundToInt(AudioListener.volume * 100) + "%";
    }
}
