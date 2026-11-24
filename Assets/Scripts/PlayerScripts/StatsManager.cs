using UnityEngine;

/// <summary>
/// Guarda todas las estadisticas del jugador: daño, vida, velocidad, las mejoras y los datos de la partida.
/// Pasa de una escena a otra (DontDestroyOnLoad) para no perder nada al cambiar de nivel.
/// Los valores base no se tocan; las mejoras se suman en los metodos de abajo (MaxHealth, Damage...).
/// </summary>
public class StatsManager : MonoBehaviour
{

    /// <summary>
    /// La unica copia, para leer las estadisticas desde cualquier script con StatsManager.Instance.
    /// </summary>
    public static StatsManager Instance;

    [Header("Combat Stats")]
    public int damage;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;

    [Header ("Movement Stats")]
    public int speed;
    public int dashSpeed;

    [Header("Health Stats")]
    public int maxHealth;
    public int currentHealth;

    [Header("Permanent Upgrades")]
    // El dash no esta aqui porque se tiene desde el principio
    public bool hasBow;
    public bool hasGuard;

    [Header("Temporary Upgrades")]
    // Nivel de cada mejora temporal (0 = no la tiene). La posicion de cada una es la de las constantes de abajo
    public int[] upgradeLevels = new int[12];

    // Posicion de cada mejora en upgradeLevels (y en la lista de UpgradeChoiceMenu y en el menu de la pausa)
    public const int HEART = 0;         // +1 corazon maximo
    public const int DASH_DISTANCE = 1; // el dash llega mas lejos
    public const int ARMOR = 2;         // mas tiempo invulnerable tras un golpe
    public const int STRENGTH = 3;      // +1 daño de espada
    public const int RANGE = 4;         // mas alcance de espada
    public const int ATTACK_SPEED = 5;  // atacas mas rapido
    public const int SPEED = 6;         // mas velocidad de movimiento
    public const int ARROW_DAMAGE = 7;  // +1 daño de flecha
    public const int FIRE_RATE = 8;     // disparas mas rapido
    public const int DASH_RECHARGE = 9; // el dash se recarga antes
    public const int GUARD = 10;        // la guardia cubre mas angulo y te empujan menos
    public const int AMULET = 11;       // segunda oportunidad al morir

    [Header("Run Stats")]
    // Datos de la partida para el Game Over y el ranking
    // El ranking ordena por pantalla alcanzada (mas es mejor) y luego por tiempo (menos es mejor)
    public int screenReached = 1;
    public float runTime;
    public int enemiesKilled;

    /// <summary>
    /// Solo vale el primero que se crea y pasa de una escena a otra, asi no se pierden la vida, las mejoras ni el tiempo.
    /// Los StatsManager que haya en los demas niveles se borran solos al cargar.
    /// </summary>
    private void Awake()
    {
        // Solo vale el primero que se crea y pasa de una escena a otra, asi no se pierden la vida, las mejoras ni el tiempo.
        // Los StatsManager que haya en los demas niveles se borran solos al cargar
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPermanents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Carga las mejoras permanentes guardadas en disco (PlayerPrefs), para que sigan al reintentar y al cerrar el juego.
    /// Con || las casillas marcadas a mano en el Inspector para probar siguen valiendo.
    /// </summary>
    private void LoadPermanents()
    {
        hasBow = hasBow || PlayerPrefs.GetInt("hasBow") == 1;
        hasGuard = hasGuard || PlayerPrefs.GetInt("hasGuard") == 1;
    }

    /// <summary>
    /// Guarda en disco (PlayerPrefs) si se tienen el arco y la guardia. 1 = si, 0 = no.
    /// </summary>
    private void SavePermanents()
    {
        PlayerPrefs.SetInt("hasBow", hasBow ? 1 : 0);
        PlayerPrefs.SetInt("hasGuard", hasGuard ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Borra el arco y la guardia guardados, para probar desde cero.
    /// Se usa en Play: tres puntos del componente -> Borrar progreso.
    /// </summary>
    [ContextMenu("Borrar progreso (permanentes)")]
    public void DeletePermanents()
    {
        hasBow = false;
        hasGuard = false;
        SavePermanents();
    }

    /// <summary>
    /// Va sumando el tiempo de la partida.
    /// Con Time.deltaTime el contador se para solo en pausa y en el Game Over (timeScale = 0).
    /// </summary>
    private void Update()
    {
        runTime += Time.deltaTime;
    }

    /// <summary>
    /// Cuenta las mejoras que lleva: las permanentes y cada nivel de las temporales.
    /// </summary>
    /// <returns>El total de mejoras.</returns>
    public int UpgradesObtained()
    {
        int total = 0;
        if (hasBow) total++;
        if (hasGuard) total++;

        foreach (int level in upgradeLevels)
        {
            total += level;
        }
        return total;
    }

    // ---- Valores con las mejoras aplicadas ----
    // Los valores base no se tocan nunca, las mejoras se suman aqui segun su nivel.
    // Asi para empezar una partida nueva basta con poner los niveles a 0

    /// <summary>
    /// Vida maxima con la mejora Corazon: un corazon mas por nivel.
    /// </summary>
    /// <returns>Los corazones maximos.</returns>
    public int MaxHealth()
    {
        return maxHealth + upgradeLevels[HEART];
    }

    /// <summary>
    /// Daño de la espada con la mejora Fuerza: +1 por nivel.
    /// </summary>
    /// <returns>El daño de cada golpe.</returns>
    public int Damage()
    {
        return damage + upgradeLevels[STRENGTH];
    }

    /// <summary>
    /// Alcance de la espada con la mejora Arma magica: +0.2 por nivel.
    /// </summary>
    /// <returns>El radio del ataque.</returns>
    public float WeaponRange()
    {
        return weaponRange + .2f * upgradeLevels[RANGE];
    }

    /// <summary>
    /// Velocidad de movimiento con la mejora Permuta: +0.5 por nivel.
    /// </summary>
    /// <returns>La velocidad del jugador.</returns>
    public float Speed()
    {
        return speed + .5f * upgradeLevels[SPEED];
    }

    /// <summary>
    /// Por lo que se multiplica el tiempo entre ataques (mejora Corte): cada nivel lo baja un 15%.
    /// </summary>
    /// <returns>1 sin mejora, menos de 1 con ella.</returns>
    public float AttackCooldownMultiplier()
    {
        return 1 - .15f * upgradeLevels[ATTACK_SPEED];
    }

    /// <summary>
    /// Segundos de invulnerabilidad que se suman despues de un golpe (mejora Armadura).
    /// </summary>
    /// <returns>Los segundos extra.</returns>
    public float InvulnerabilityBonus()
    {
        return .3f * upgradeLevels[ARMOR];
    }

    /// <summary>
    /// Daño extra de las flechas con la mejora Kunai: +1 por nivel.
    /// </summary>
    /// <returns>El daño que se suma a cada flecha.</returns>
    public int ArrowDamageBonus()
    {
        return upgradeLevels[ARROW_DAMAGE];
    }

    /// <summary>
    /// Por lo que se multiplica el tiempo entre disparos (mejora Cañon de agua): cada nivel lo baja un 15%.
    /// </summary>
    /// <returns>1 sin mejora, menos de 1 con ella.</returns>
    public float ShootCooldownMultiplier()
    {
        return 1 - .15f * upgradeLevels[FIRE_RATE];
    }

    /// <summary>
    /// Velocidad del dash con la mejora Impulso. Como el dash dura lo mismo, si va mas rapido llega mas lejos.
    /// </summary>
    /// <returns>La velocidad durante el dash.</returns>
    public float DashSpeed()
    {
        return dashSpeed + 2 * upgradeLevels[DASH_DISTANCE];
    }

    /// <summary>
    /// Por lo que se multiplica la recarga del dash (mejora Niebla): cada nivel la baja un 20%.
    /// </summary>
    /// <returns>1 sin mejora, menos de 1 con ella.</returns>
    public float DashCooldownMultiplier()
    {
        return 1 - .2f * upgradeLevels[DASH_RECHARGE];
    }

    /// <summary>
    /// Grados que se suman al angulo que cubre el escudo (mejora Defensa).
    /// </summary>
    /// <returns>Los grados extra.</returns>
    public float GuardAngleBonus()
    {
        return 10 * upgradeLevels[GUARD];
    }

    /// <summary>
    /// Por lo que se multiplica el empujon de un golpe bloqueado (mejora Defensa): cada nivel lo baja un 30%.
    /// </summary>
    /// <returns>1 sin mejora, menos de 1 con ella.</returns>
    public float GuardKnockbackMultiplier()
    {
        return 1 - .3f * upgradeLevels[GUARD];
    }

    /// <summary>
    /// Mira si tiene el Amuleto (segunda oportunidad al morir).
    /// </summary>
    /// <returns>True si lo tiene.</returns>
    public bool HasAmulet()
    {
        return upgradeLevels[AMULET] > 0;
    }

    /// <summary>
    /// Aplica una mejora temporal elegida en el menu: le sube un nivel.
    /// </summary>
    /// <param name="index">Posicion de la mejora (las constantes HEART, ARMOR...).</param>
    public void ApplyUpgrade(int index)
    {
        upgradeLevels[index]++;

        // El corazon nuevo aparece ya lleno
        if (index == HEART)
        {
            currentHealth++;
        }
    }

    /// <summary>Al terminar un nivel se recupera toda la vida.</summary>
    public void HealFull()
    {
        currentHealth = MaxHealth();
    }

    /// <summary>
    /// Desbloquea una mejora permanente y la guarda en disco.
    /// </summary>
    /// <param name="index">0 arco, 1 guardia.</param>
    public void UnlockPermanent(int index)
    {
        if (index == 0) hasBow = true;
        else if (index == 1) hasGuard = true;
        SavePermanents();
    }

    /// <summary>
    /// Mira si ya se tiene una mejora permanente.
    /// </summary>
    /// <param name="index">0 arco, 1 guardia.</param>
    /// <returns>True si ya la tiene.</returns>
    public bool HasPermanent(int index)
    {
        if (index == 0) return hasBow;
        return hasGuard;
    }

    /// <summary>
    /// Deja todo como al empezar una partida nueva (las mejoras permanentes se mantienen).
    /// </summary>
    public void ResetRun()
    {
        currentHealth = maxHealth;
        screenReached = 1;
        runTime = 0;
        enemiesKilled = 0;
        upgradeLevels = new int[12];
    }


}
