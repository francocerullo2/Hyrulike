using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Lo que hace falta tener para que una mejora pueda salir: nada, el arco o la guardia.
/// </summary>
public enum UpgradeRequirement
{
    None,
    Bow,
    Guard
}

/// <summary>
/// Datos de una mejora temporal: nombre, descripcion, icono, nivel maximo y requisito.
/// Se rellenan en el Inspector de UpgradeChoiceMenu.
/// </summary>
[System.Serializable]
public class Upgrade
{
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;
    public int maxLevel = 3; // 0 = no tiene nivel y puede salir siempre
    public UpgradeRequirement requirement;

    /// <summary>
    /// Crea una mejora con sus datos (el icono se pone luego en el Inspector).
    /// </summary>
    /// <param name="upgradeName">Nombre que sale en el menu.</param>
    /// <param name="description">Lo que hace la mejora.</param>
    /// <param name="maxLevel">Nivel maximo (0 = sin niveles).</param>
    /// <param name="requirement">Lo que hace falta tener para que salga.</param>
    public Upgrade(string upgradeName, string description, int maxLevel, UpgradeRequirement requirement)
    {
        this.upgradeName = upgradeName;
        this.description = description;
        this.maxLevel = maxLevel;
        this.requirement = requirement;
    }
}

/// <summary>
/// Menu que sale al terminar un nivel para elegir una mejora.
/// Al acabar cada zona se elige una permanente (arco o guardia) y en el resto una temporal entre tres al azar.
/// Va en un objeto que siempre este activo (MenuCanvas), no en los paneles.
/// </summary>
public class UpgradeChoiceMenu : MonoBehaviour
{
    /// <summary>
    /// Igual que StatsManager.Instance: asi se puede abrir desde cualquier sitio.
    /// </summary>
    public static UpgradeChoiceMenu Instance;

    /// <summary>
    /// Es static para que PauseMenu sepa que no tiene que abrirse con Esc mientras se elige.
    /// </summary>
    public static bool isChoosing;

    [Header("Mejoras temporales")]
    public GameObject upgradePanel;
    // En el mismo orden que las constantes de StatsManager. Salen ya rellenas al añadir el componente, solo faltan los iconos
    public Upgrade[] upgrades =
    {
        new Upgrade("Corazón", "+1 corazón máximo", 3, UpgradeRequirement.None),
        new Upgrade("Impulso", "El dash llega más lejos", 3, UpgradeRequirement.None),
        new Upgrade("Armadura", "Más tiempo invulnerable después de recibir un golpe", 3, UpgradeRequirement.None),
        new Upgrade("Fuerza", "+1 daño de espada", 3, UpgradeRequirement.None),
        new Upgrade("Arma mágica", "Más alcance de la espada", 3, UpgradeRequirement.None),
        new Upgrade("Corte", "Atacas más rápido", 3, UpgradeRequirement.None),
        new Upgrade("Permuta", "Más velocidad de movimiento", 3, UpgradeRequirement.None),
        new Upgrade("Kunai", "+1 daño de flecha", 3, UpgradeRequirement.Bow),
        new Upgrade("Cañón de agua", "Disparas más rápido", 3, UpgradeRequirement.Bow),
        new Upgrade("Niebla", "El dash se recarga antes", 3, UpgradeRequirement.None),
        new Upgrade("Defensa", "La guardia cubre más ángulo y te empujan menos al bloquear", 3, UpgradeRequirement.Guard),
        new Upgrade("Amuleto", "Si mueres, revives una vez con 2 corazones", 1, UpgradeRequirement.None),
    };
    public Image[] optionIcons; // Icon1 e Icon2, tienen que tener un componente Button
    public TMP_Text[] optionNames; // Upgrade1 y Upgrade2
    public TMP_Text upgradeDescription;
    public Button upgradeContinueButton;

    [Header("Mejoras permanentes")]
    public GameObject permanentPanel;
    public int permanentEvery = 3; // cada cuantos niveles toca elegir permanente (al acabar cada zona)
    // Las listas en el orden 0 arco, 1 guardia
    public Image[] permanentIcons; // IconBow e IconGuard, tienen que tener un componente Button
    public Sprite[] permanentIconsOn;
    public Sprite[] permanentIconsOff;
    [TextArea] public string[] permanentDescriptions;
    public TMP_Text permanentDescription;
    public Button permanentContinueButton;

    public Color notSelectedColor = new Color(.5f, .5f, .5f); // la opcion que no esta elegida se ve mas oscura
    public string mainMenuScene = "MainMenu";

    private List<int> options = new List<int>(); // mejoras que salen ahora (posicion en upgrades)
    private int selected = -1; // opcion elegida, -1 = ninguna todavia
    private bool isPermanentChoice;
    private string nextLevel; // escena que se carga al darle a Continuar

    /// <summary>Se guarda como Instance.</summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>Empieza con los dos paneles cerrados.</summary>
    private void Start()
    {
        isChoosing = false;
        upgradePanel.SetActive(false);
        permanentPanel.SetActive(false);
    }

    /// <summary>
    /// Abre el menu y se guarda a que escena hay que ir despues.
    /// Lo llama la salida del nivel al cruzarla; al darle a Continuar se carga nextLevel.
    /// </summary>
    /// <param name="nextLevel">Nombre de la escena siguiente.</param>
    public void Show(string nextLevel)
    {
        this.nextLevel = nextLevel;
        Show();
    }

    /// <summary>
    /// Para el juego y abre el panel que toque: el de permanentes o el de temporales.
    /// Sin nivel siguiente sirve para probar en Play desde los tres puntos del componente (no cambia de escena).
    /// </summary>
    [ContextMenu("Probar: abrir menu de mejoras")]
    public void Show()
    {
        isChoosing = true;
        selected = -1;

        // Igual que en la pausa: con isPaused el jugador no se mueve y con timeScale 0 se para todo
        PauseMenu.isPaused = true;
        Time.timeScale = 0;

        isPermanentChoice = IsPermanentLevel(StatsManager.Instance.screenReached);

        if (isPermanentChoice)
        {
            ShowPermanent();
        }
        else
        {
            ShowTemporary();
        }
    }

    // ---- Mejoras temporales ----

    /// <summary>Saca mejoras temporales al azar y las pone en los huecos del panel.</summary>
    private void ShowTemporary()
    {
        options = GetRandomOptions(optionIcons.Length);

        for (int i = 0; i < optionIcons.Length; i++)
        {
            // Si quedan menos mejoras que huecos, el hueco que sobra se esconde
            bool hasOption = i < options.Count;
            optionIcons[i].gameObject.SetActive(hasOption);
            optionNames[i].gameObject.SetActive(hasOption);

            if (hasOption)
            {
                Upgrade upgrade = upgrades[options[i]];
                optionIcons[i].sprite = upgrade.icon;
                optionIcons[i].color = Color.white;
                optionNames[i].text = upgrade.upgradeName;
            }
        }

        upgradePanel.SetActive(true);

        // La primera sale ya elegida, asi se ve su descripcion nada mas abrir y se puede continuar directamente
        if (options.Count > 0)
        {
            SelectOption(0);
        }
    }

    /// <summary>
    /// Marca una de las mejoras temporales y enseña su descripcion.
    /// Lo llaman los botones de los iconos.
    /// </summary>
    /// <param name="option">Hueco pulsado (0 el primero, 1 el segundo...).</param>
    public void SelectOption(int option)
    {
        selected = option;

        // La elegida se ve normal y la otra mas oscura
        for (int i = 0; i < optionIcons.Length; i++)
        {
            optionIcons[i].color = i == option ? Color.white : notSelectedColor;
        }

        int index = options[option];
        Upgrade upgrade = upgrades[index];
        upgradeDescription.text = upgrade.upgradeName + LevelText(index, upgrade) + "\n" + upgrade.description;
        upgradeContinueButton.interactable = true;
    }

    /// <summary>
    /// Texto con el nivel que tiene y al que pasa, por ejemplo "(nivel 1 -> 2)", o nada si la mejora no tiene niveles.
    /// </summary>
    /// <param name="index">Posicion de la mejora en upgrades.</param>
    /// <param name="upgrade">La mejora.</param>
    /// <returns>El texto del nivel.</returns>
    private string LevelText(int index, Upgrade upgrade)
    {
        if (upgrade.maxLevel == 0)
        {
            return "";
        }
        int level = StatsManager.Instance.upgradeLevels[index];
        return "  (nivel " + level + " -> " + (level + 1) + ")";
    }

    /// <summary>
    /// Elige al azar varias mejoras distintas entre las que se pueden coger ahora.
    /// Se va sacando una al azar y se quita de la lista, asi nunca salen dos iguales.
    /// </summary>
    /// <param name="amount">Cuantas mejoras sacar.</param>
    /// <returns>Las posiciones de las mejoras elegidas (pueden ser menos si no quedan).</returns>
    public List<int> GetRandomOptions(int amount)
    {
        List<int> available = new List<int>();
        for (int i = 0; i < upgrades.Length; i++)
        {
            if (IsAvailable(i))
            {
                available.Add(i);
            }
        }

        // Se va sacando una al azar y se quita de la lista, asi nunca salen dos iguales
        List<int> result = new List<int>();
        while (result.Count < amount && available.Count > 0)
        {
            int random = Random.Range(0, available.Count);
            result.Add(available[random]);
            available.RemoveAt(random);
        }
        return result;
    }

    /// <summary>
    /// Una mejora puede salir si no esta al maximo y el jugador tiene lo que necesita (arco o guardia).
    /// </summary>
    /// <param name="index">Posicion de la mejora en upgrades.</param>
    /// <returns>True si puede salir.</returns>
    public bool IsAvailable(int index)
    {
        StatsManager stats = StatsManager.Instance;
        Upgrade upgrade = upgrades[index];

        if (upgrade.maxLevel > 0 && stats.upgradeLevels[index] >= upgrade.maxLevel)
        {
            return false;
        }

        if (upgrade.requirement == UpgradeRequirement.Bow) return stats.hasBow;
        if (upgrade.requirement == UpgradeRequirement.Guard) return stats.hasGuard;
        return true;
    }

    // ---- Mejoras permanentes ----

    /// <summary>
    /// Abre el panel de permanentes: las que ya tiene salen encendidas y no se pueden pulsar.
    /// </summary>
    private void ShowPermanent()
    {
        for (int i = 0; i < permanentIcons.Length; i++)
        {
            // Las que ya tiene salen encendidas y no se pueden pulsar, las que no tiene salen apagadas
            bool owned = StatsManager.Instance.HasPermanent(i);
            permanentIcons[i].sprite = owned ? permanentIconsOn[i] : permanentIconsOff[i];
            permanentIcons[i].GetComponent<Button>().interactable = !owned;
        }

        permanentPanel.SetActive(true);

        // La primera que no tiene sale ya elegida, igual que en las temporales
        for (int i = 0; i < permanentIcons.Length; i++)
        {
            if (!StatsManager.Instance.HasPermanent(i))
            {
                SelectPermanent(i);
                break;
            }
        }
    }

    /// <summary>
    /// Marca una mejora permanente y enseña su descripcion.
    /// Lo llaman los botones de los iconos.
    /// </summary>
    /// <param name="index">0 arco, 1 guardia.</param>
    public void SelectPermanent(int index)
    {
        selected = index;

        // La elegida se enciende y las otras que no tiene se quedan apagadas
        for (int i = 0; i < permanentIcons.Length; i++)
        {
            bool on = i == index || StatsManager.Instance.HasPermanent(i);
            permanentIcons[i].sprite = on ? permanentIconsOn[i] : permanentIconsOff[i];
        }

        permanentDescription.text = permanentDescriptions[index];
        permanentContinueButton.interactable = true;
    }

    /// <summary>
    /// Dice si al acabar este nivel toca elegir permanente.
    /// Toca en los niveles 3 y 6, pero solo si todavia no tiene las que corresponden a ese nivel: en el 3 se puede tener como mucho 1 y en el 6 las 2.
    /// Asi si ya consiguio una en el 3 de otra partida, al volver al 3 elige temporal y la segunda llega en el 6.
    /// En el 9 (o si ya las tiene todas) sale una temporal normal.
    /// </summary>
    /// <param name="level">Nivel que se acaba de terminar.</param>
    /// <returns>True si toca permanente.</returns>
    public bool IsPermanentLevel(int level)
    {
        if (level % permanentEvery != 0)
        {
            return false;
        }
        if (OwnedPermanents() >= permanentIcons.Length)
        {
            return false;
        }
        int allowed = level / permanentEvery;
        return OwnedPermanents() < allowed;
    }

    /// <summary>
    /// Cuenta cuantas mejoras permanentes tiene ya.
    /// </summary>
    /// <returns>0, 1 o 2.</returns>
    private int OwnedPermanents()
    {
        int owned = 0;
        for (int i = 0; i < permanentIcons.Length; i++)
        {
            if (StatsManager.Instance.HasPermanent(i))
            {
                owned++;
            }
        }
        return owned;
    }

    // ---- Botones Continuar y Salir ----

    /// <summary>
    /// Boton Continuar de los dos paneles: aplica la mejora elegida, cura entera la vida y pasa al siguiente nivel.
    /// </summary>
    public void Confirm()
    {
        if (selected < 0)
        {
            return;
        }

        if (isPermanentChoice)
        {
            StatsManager.Instance.UnlockPermanent(selected);
        }
        else
        {
            StatsManager.Instance.ApplyUpgrade(options[selected]);
        }

        // Al pasar de nivel se recupera toda la vida (despues de la mejora, asi cuenta el corazon nuevo)
        StatsManager.Instance.HealFull();

        // Que se vean los corazones llenos y los nuevos si la mejora era de vida
        HeartsUI.Instance.UpdateHearts(StatsManager.Instance.currentHealth, StatsManager.Instance.MaxHealth());

        upgradePanel.SetActive(false);
        permanentPanel.SetActive(false);
        Unpause();

        // Siguiente pantalla
        StatsManager.Instance.screenReached++;
        if (!string.IsNullOrEmpty(nextLevel))
        {
            SceneManager.LoadScene(nextLevel);
        }
    }

    /// <summary>Boton Salir al menu: reinicia la partida y vuelve al menu principal.</summary>
    public void ExitToMenu()
    {
        StatsManager.Instance.ResetRun();
        Unpause();
        SceneManager.LoadScene(mainMenuScene);
    }

    /// <summary>
    /// Quita la pausa. Si no, el juego (o la siguiente escena) se quedaria congelado.
    /// </summary>
    private void Unpause()
    {
        isChoosing = false;
        PauseMenu.isPaused = false;
        Time.timeScale = 1;
    }
}
