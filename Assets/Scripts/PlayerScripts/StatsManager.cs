using UnityEngine;

public class StatsManager : MonoBehaviour
{

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
    public bool hasBow;
    public bool hasDash;
    public bool hasGuard;

    [Header("Temporary Upgrades")]
    // Nivel de cada mejora temporal (0 = no la tiene). La posicion de cada una es la de las constantes de abajo
    public int[] upgradeLevels = new int[12];

    // Posicion de cada mejora en upgradeLevels (y en la lista de UpgradeChoiceMenu y en el menu de la pausa)
    public const int HEART = 0;         // +1 corazon maximo
    public const int HEAL = 1;          // recupera 2 corazones (no tiene nivel, se puede coger siempre)
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

    // Las permanentes se guardan en disco (PlayerPrefs) para que sigan al reintentar y al cerrar el juego
    // Con || las casillas marcadas a mano en el Inspector para probar siguen valiendo
    private void LoadPermanents()
    {
        hasBow = hasBow || PlayerPrefs.GetInt("hasBow") == 1;
        hasGuard = hasGuard || PlayerPrefs.GetInt("hasGuard") == 1;
        hasDash = hasDash || PlayerPrefs.GetInt("hasDash") == 1;
    }

    private void SavePermanents()
    {
        PlayerPrefs.SetInt("hasBow", hasBow ? 1 : 0);
        PlayerPrefs.SetInt("hasGuard", hasGuard ? 1 : 0);
        PlayerPrefs.SetInt("hasDash", hasDash ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Para probar desde cero: en Play, tres puntos del componente -> Borrar progreso
    [ContextMenu("Borrar progreso (permanentes)")]
    public void DeletePermanents()
    {
        hasBow = false;
        hasGuard = false;
        hasDash = false;
        SavePermanents();
    }

    // Con Time.deltaTime el contador se para solo en pausa y en el Game Over (timeScale = 0)
    private void Update()
    {
        runTime += Time.deltaTime;
    }

    // Cuenta las mejoras que lleva: las permanentes y cada nivel de las temporales
    public int UpgradesObtained()
    {
        int total = 0;
        if (hasBow) total++;
        if (hasDash) total++;
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

    public int MaxHealth()
    {
        return maxHealth + upgradeLevels[HEART];
    }

    public int Damage()
    {
        return damage + upgradeLevels[STRENGTH];
    }

    public float WeaponRange()
    {
        return weaponRange + .2f * upgradeLevels[RANGE];
    }

    public float Speed()
    {
        return speed + .5f * upgradeLevels[SPEED];
    }

    // Por lo que se multiplica el tiempo entre ataques: cada nivel lo baja un 15%
    public float AttackCooldownMultiplier()
    {
        return 1 - .15f * upgradeLevels[ATTACK_SPEED];
    }

    // Segundos de invulnerabilidad que se suman despues de un golpe
    public float InvulnerabilityBonus()
    {
        return .3f * upgradeLevels[ARMOR];
    }

    public int ArrowDamageBonus()
    {
        return upgradeLevels[ARROW_DAMAGE];
    }

    public float ShootCooldownMultiplier()
    {
        return 1 - .15f * upgradeLevels[FIRE_RATE];
    }

    public float DashCooldownMultiplier()
    {
        return 1 - .2f * upgradeLevels[DASH_RECHARGE];
    }

    // Grados que se suman al angulo que cubre el escudo
    public float GuardAngleBonus()
    {
        return 10 * upgradeLevels[GUARD];
    }

    // Por lo que se multiplica el empujon de un golpe bloqueado
    public float GuardKnockbackMultiplier()
    {
        return 1 - .3f * upgradeLevels[GUARD];
    }

    public bool HasAmulet()
    {
        return upgradeLevels[AMULET] > 0;
    }

    // Aplica una mejora temporal elegida en el menu
    public void ApplyUpgrade(int index)
    {
        // La curacion no sube de nivel, se aplica en el momento
        if (index == HEAL)
        {
            currentHealth = Mathf.Min(currentHealth + 2, MaxHealth());
            return;
        }

        upgradeLevels[index]++;

        // El corazon nuevo aparece ya lleno
        if (index == HEART)
        {
            currentHealth++;
        }
    }

    // Desbloquea una mejora permanente: 0 arco, 1 guardia, 2 dash
    public void UnlockPermanent(int index)
    {
        if (index == 0) hasBow = true;
        else if (index == 1) hasGuard = true;
        else if (index == 2) hasDash = true;
        SavePermanents();
    }

    public bool HasPermanent(int index)
    {
        if (index == 0) return hasBow;
        if (index == 1) return hasGuard;
        return hasDash;
    }

    // Deja todo como al empezar una partida nueva (las mejoras permanentes se mantienen)
    public void ResetRun()
    {
        currentHealth = maxHealth;
        screenReached = 1;
        runTime = 0;
        enemiesKilled = 0;
        upgradeLevels = new int[12];
    }


}
