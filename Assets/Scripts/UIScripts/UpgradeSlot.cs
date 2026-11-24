using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hueco del menu de pausa para una mejora temporal: su icono y las flechas de nivel.
/// </summary>
public class UpgradeSlot : MonoBehaviour
{
    public Image icon;
    public Sprite enabledIcon;
    public Sprite disabledIcon;

    // Flechas de subir de nivel que salen al lado del icono (nivel 2 y nivel 3)
    public GameObject[] levelIcons;

    /// <summary>
    /// Pone el icono en color o en gris y enseña una flecha por cada nivel por encima del primero.
    /// </summary>
    /// <param name="level">Nivel de la mejora (0 = no la tiene).</param>
    public void SetLevel(int level)
    {
        // A nivel 0 todavia no se ha elegido y sale en gris
        if (level > 0)
        {
            icon.sprite = enabledIcon;
        }
        else
        {
            icon.sprite = disabledIcon;
        }

        // Una flecha por cada nivel por encima del primero
        for (int i = 0; i < levelIcons.Length; i++)
        {
            levelIcons[i].SetActive(level >= i + 2);
        }
    }
}
