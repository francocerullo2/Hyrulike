using UnityEngine;
using UnityEngine.UI;

/// <summary>Pinta la vida del jugador como una fila de corazones.</summary>
public class HeartsUI : MonoBehaviour
{
    /// <summary>
    /// Igual que StatsManager.Instance: asi el jugador lo encuentra solo en cualquier nivel.
    /// </summary>
    public static HeartsUI Instance;

    // Imagenes de los corazones, de izquierda a derecha
    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    /// <summary>
    /// Se guarda como Instance.
    /// Awake va antes que cualquier Start, asi ya esta listo cuando PlayerHealth pinta los corazones.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Enseña tantos corazones como vida maxima, los primeros llenos y el resto vacios.
    /// </summary>
    /// <param name="currentHealth">Vida que tiene ahora.</param>
    /// <param name="maxHealth">Vida maxima (con la mejora Corazon).</param>
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
