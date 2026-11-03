using UnityEngine;
using UnityEngine.UI;

// Hueco del menu para una mejora temporal
public class UpgradeSlot : MonoBehaviour
{
    public Image icon;
    public Sprite enabledIcon;
    public Sprite disabledIcon;

    // Flechas de subir de nivel que salen al lado del icono (nivel 2 y nivel 3)
    public GameObject[] levelIcons;

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
