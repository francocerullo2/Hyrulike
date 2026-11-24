using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pinta la vida del jefe arriba en la pantalla: 10 corazones por fase y su retrato.
/// Va en el canvas del nivel final y empieza oculto.
/// </summary>
public class BossUI : MonoBehaviour
{
    /// <summary>La unica copia, para que Boss_Phases la llame con BossUI.Instance.</summary>
    public static BossUI Instance;

    // Imagenes de los corazones, de izquierda a derecha
    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Image portrait;

    /// <summary>
    /// Se guarda como Instance y se oculta: mientras el jefe es el monje no se ve su vida.
    /// </summary>
    private void Awake()
    {
        Instance = this;

        // Empieza oculto: mientras el jefe es el monje no se ve su vida
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Enseña la vida del jefe con el retrato de la fase.
    /// Lo llama Boss_Phases al empezar cada fase.
    /// </summary>
    /// <param name="bossPortrait">El retrato del samurai o de la bestia.</param>
    public void Show(Sprite bossPortrait)
    {
        portrait.sprite = bossPortrait;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Pinta los corazones: los primeros llenos y el resto vacios.
    /// </summary>
    /// <param name="health">Cuantos corazones llenos hay que pintar.</param>
    public void UpdateHearts(int health)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // Los primeros estan llenos y el resto vacios
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    /// <summary>Oculta la vida del jefe. Lo llama Boss_Phases cuando el jefe muere.</summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
