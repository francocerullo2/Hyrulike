using UnityEngine;
using UnityEngine.UI;

// Pinta la vida del jefe: 10 corazones por fase y su retrato
public class BossUI : MonoBehaviour
{
    public static BossUI Instance;

    // Imagenes de los corazones, de izquierda a derecha
    public Image[] hearts;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Image portrait;

    private void Awake()
    {
        Instance = this;

        // Empieza oculto: mientras el jefe es el monje no se ve su vida
        gameObject.SetActive(false);
    }

    // Lo llama Boss_Phases al empezar cada fase, con el retrato del samurai o de la bestia
    public void Show(Sprite bossPortrait)
    {
        portrait.sprite = bossPortrait;
        gameObject.SetActive(true);
    }

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

    // Lo llama Boss_Phases cuando el jefe muere
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
