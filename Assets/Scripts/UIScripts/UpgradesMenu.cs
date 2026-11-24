using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra en el menu de pausa las mejoras permanentes (arco y guardia) y las temporales.
/// </summary>
public class UpgradesMenu : MonoBehaviour
{
    [Header("Arco")]
    public Image bowSlot;
    public Sprite bowIcon;
    public Sprite bowDisabledIcon;

    [Header("Guardia")]
    public Image guardSlot;
    public Sprite guardIcon;
    public Sprite guardDisabledIcon;

    [Header("Mejoras temporales")]
    // En el mismo orden que upgradeLevels del StatsManager
    public UpgradeSlot[] tempSlots;

    /// <summary>Pinta las mejoras al empezar.</summary>
    private void Start()
    {
        UpdateSlots();
    }

    /// <summary>
    /// Se llama cada vez que se activa el menu, asi siempre sale actualizado.
    /// </summary>
    private void OnEnable()
    {
        // Al cargar la escena puede que el StatsManager aun no exista
        if (StatsManager.Instance != null)
        {
            UpdateSlots();
        }
    }

    /// <summary>
    /// Pinta todas las mejoras segun lo que tenga el jugador en StatsManager.
    /// </summary>
    public void UpdateSlots()
    {
        SetSlot(bowSlot, StatsManager.Instance.hasBow, bowIcon, bowDisabledIcon);
        SetSlot(guardSlot, StatsManager.Instance.hasGuard, guardIcon, guardDisabledIcon);

        for (int i = 0; i < tempSlots.Length; i++)
        {
            tempSlots[i].SetLevel(StatsManager.Instance.upgradeLevels[i]);
        }
    }

    /// <summary>
    /// Pone el icono en color si la mejora esta desbloqueada y en gris si no.
    /// </summary>
    /// <param name="slot">La imagen del hueco.</param>
    /// <param name="unlocked">Si se tiene la mejora.</param>
    /// <param name="icon">Icono en color.</param>
    /// <param name="disabledIcon">Icono en gris.</param>
    private void SetSlot(Image slot, bool unlocked, Sprite icon, Sprite disabledIcon)
    {
        if (unlocked)
        {
            slot.sprite = icon;
        }
        else
        {
            slot.sprite = disabledIcon;
        }
    }
}
