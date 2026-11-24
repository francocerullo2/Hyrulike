using UnityEngine;

/// <summary>
/// Salida del nivel: se arrastra el prefab a la escena y se coloca donde se quiera.
/// Esta cerrada (mas oscura) hasta que mueren los enemigos. Al cruzarla abierta se elige mejora y se pasa a la siguiente pantalla.
/// </summary>
public class LevelExit : MonoBehaviour
{
    public Color closedColor = new Color(.5f, .5f, .5f);

    private SpriteRenderer spriteRenderer;
    private Collider2D exitCollider;

    /// <summary>
    /// Cierra la salida: la oscurece y apaga su collider.
    /// En Awake y no en Start: LevelManager la abre en su Start si el nivel no tiene enemigos, y tiene que estar ya cerrada.
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        exitCollider = GetComponent<Collider2D>();

        spriteRenderer.color = closedColor;
        exitCollider.enabled = false;
    }

    /// <summary>
    /// Abre la salida: vuelve a su color y enciende el collider.
    /// Lo llama LevelManager cuando no quedan enemigos.
    /// Al activar el collider, si el jugador ya estaba encima salta OnTriggerEnter2D igualmente.
    /// </summary>
    public void Open()
    {
        spriteRenderer.color = Color.white;
        exitCollider.enabled = true;
    }

    /// <summary>
    /// Si el jugador cruza la salida abierta, sale el menu de elegir mejora.
    /// </summary>
    /// <param name="collision">El collider que ha entrado.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            exitCollider.enabled = false; // para que no se abra el menu dos veces
            UpgradeChoiceMenu.Instance.Show(LevelManager.Instance.nextLevel);
        }
    }
}
