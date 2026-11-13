using UnityEngine;

// Salida del nivel: se arrastra el prefab a la escena y se coloca donde se quiera
// Esta cerrada (mas oscura) hasta que mueren los enemigos. Al cruzarla abierta se elige mejora y se pasa a la siguiente pantalla
public class LevelExit : MonoBehaviour
{
    public Color closedColor = new Color(.5f, .5f, .5f);

    private SpriteRenderer spriteRenderer;
    private Collider2D exitCollider;

    // En Awake y no en Start: LevelManager la abre en su Start si el nivel no tiene enemigos, y tiene que estar ya cerrada
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        exitCollider = GetComponent<Collider2D>();

        spriteRenderer.color = closedColor;
        exitCollider.enabled = false;
    }

    // Lo llama LevelManager cuando no quedan enemigos
    // Al activar el collider, si el jugador ya estaba encima salta OnTriggerEnter2D igualmente
    public void Open()
    {
        spriteRenderer.color = Color.white;
        exitCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            exitCollider.enabled = false; // para que no se abra el menu dos veces
            UpgradeChoiceMenu.Instance.Show(LevelManager.Instance.nextLevel);
        }
    }
}
