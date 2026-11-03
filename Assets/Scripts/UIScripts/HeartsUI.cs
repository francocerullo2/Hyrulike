using UnityEngine;
using UnityEngine.UI;

// Pinta la vida del jugador como una fila de corazones
public class HeartsUI : MonoBehaviour
{
    // Imagenes de los corazones, de izquierda a derecha
    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    public void UpdateHearts(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // Solo se ven tantos corazones como vida maxima tenga el jugador
            hearts[i].gameObject.SetActive(i < maxHealth);

            // Los primeros estan llenos y el resto vacios
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }
}
