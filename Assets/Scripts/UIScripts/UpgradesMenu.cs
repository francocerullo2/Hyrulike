using UnityEngine;
using UnityEngine.UI;

// Muestra en el menu las mejoras permanentes (arco, dash y guardia) y las temporales
public class UpgradesMenu : MonoBehaviour
{
    [Header("Arco")]
    public Image bowSlot;
    public Sprite bowIcon;
    public Sprite bowDisabledIcon;

    [Header("Dash")]
    public Image dashSlot;
    public Sprite dashIcon;
    public Sprite dashDisabledIcon;

    [Header("Guardia")]
    public Image guardSlot;
    public Sprite guardIcon;
    public Sprite guardDisabledIcon;

    [Header("Mejoras temporales")]
    // En el mismo orden que upgradeLevels del StatsManager
    public UpgradeSlot[] tempSlots;

    private void Start()
    {
        UpdateSlots();
    }

    // Se llama cada vez que se activa el menu, asi siempre sale actualizado
    private void OnEnable()
    {
        // Al cargar la escena puede que el StatsManager aun no exista
        if (StatsManager.Instance != null)
        {
            UpdateSlots();
        }
    }

    public void UpdateSlots()
    {
        SetSlot(bowSlot, StatsManager.Instance.hasBow, bowIcon, bowDisabledIcon);
        SetSlot(dashSlot, StatsManager.Instance.hasDash, dashIcon, dashDisabledIcon);
        SetSlot(guardSlot, StatsManager.Instance.hasGuard, guardIcon, guardDisabledIcon);

        for (int i = 0; i < tempSlots.Length; i++)
        {
            tempSlots[i].SetLevel(StatsManager.Instance.upgradeLevels[i]);
        }
    }

    // Pone el icono en color si la mejora esta desbloqueada y en gris si no
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
