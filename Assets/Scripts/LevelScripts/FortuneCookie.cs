using UnityEngine;

// Galleta de la fortuna que sale al matar a todos los enemigos del nivel
// Al tocarla se abre el menu de mejoras, y al darle a Continuar se pasa al siguiente nivel
public class FortuneCookie : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UpgradeChoiceMenu.Instance.Show(LevelManager.Instance.nextLevel);
            Destroy(gameObject);
        }
    }
}
