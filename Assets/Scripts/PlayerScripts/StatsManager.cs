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
    // Nivel de cada mejora (0 = no la tiene, maximo 3). Orden:
    // 0 Heal, 1 Armor, 2 Amulet, 3 AttackUpgrade, 4 MagicWeapon, 5 Cut,
    // 6 Kunai, 7 WaterCanon, 8 Explosion, 9 Permutation, 10 Mist, 11 Camouflage
    public int[] upgradeLevels = new int[12];

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
    }


}
