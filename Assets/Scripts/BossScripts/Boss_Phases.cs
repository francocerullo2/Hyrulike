using UnityEngine;

// Controla las formas del jefe: empieza como monje, al acercarse el jugador se convierte en samurai (fase 1)
// y al bajar de la mitad de vida se transforma en la bestia (fase 2)
// El movimiento y los ataques los siguen haciendo Enemy_Movement y Enemy_Combat, este script solo los enciende, apaga y cambia sus valores
public class Boss_Phases : MonoBehaviour
{
    public int phase = 0; // 0 = monje, 1 = samurai, 2 = bestia

    [Header("Monje")]
    public float wakeUpRange = 3; // si el jugador se acerca a esta distancia, el monje se transforma
    public LayerMask playerLayer;

    [Header("Bestia (fase 2)")]
    public float monsterSpeed = 1.5f;
    public int monsterDamage = 3;
    public float monsterAttackRange = 1;
    public float monsterWeaponRange = 1.5f;
    public float monsterAttackCoolDown = 2.2f;
    public CapsuleCollider2D bodyCollider; // opcional: si la bestia es mas grande se cambia el tamaño del collider
    public Vector2 monsterBodySize;
    public AudioClip monsterAttackSound; // el del samurai se pone en Enemy_Combat, este lo sustituye en la fase 2

    [Header("Sonido")]
    public AudioClip transformSound; // suena en las dos transformaciones

    [Header("Iconos")]
    public Sprite samuraiPortrait;
    public Sprite monsterPortrait;

    private bool isTransforming;

    private Enemy_Health enemy_Health;
    private Animator anim;
    private Rigidbody2D rb;
    private Enemy_Movement enemy_Movement;
    private Enemy_Combat enemy_Combat;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemy_Movement = GetComponent<Enemy_Movement>();
        enemy_Combat = GetComponent<Enemy_Combat>();
        enemy_Health = GetComponent<Enemy_Health>();

        // El monje se queda quieto hasta que llega el jugador
        enemy_Movement.enabled = false;
    }

    void Update()
    {
        // Solo el monje busca al jugador, luego ya se encarga Enemy_Movement
        if (phase == 0 && !isTransforming)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, wakeUpRange, playerLayer);
            if (hits.Length > 0)
            {
                isTransforming = true;
                anim.SetTrigger("ToSamurai");
                AudioManager.Instance.PlaySFX(transformSound);
            }
        }
    }

    // Lo usan Enemy_Health y Enemy_Knockback: el monje y las transformaciones no reciben daño
    public bool CanTakeDamage()
    {
        return phase != 0 && !isTransforming;
    }

    // Lo llama un evento al final de la animacion de curarse del monje
    public void StartPhase1()
    {
        // La animacion de curarse tambien se usa al pasar a bestia, ahi no tiene que hacer nada
        if (phase != 0)
        {
            return;
        }

        phase = 1;
        isTransforming = false;
        enemy_Movement.enabled = true;

        // Aparece la vida del jefe con los 10 corazones del samurai
        BossUI.Instance.Show(samuraiPortrait);
        BossUI.Instance.UpdateHearts(enemy_Health.maxHealth / 2);
    }

    // Lo llama Enemy_Health cada vez que el jefe recibe daño
    public void CheckPhase(int currentHealth, int maxHealth)
    {
        // El samurai gasta la primera mitad de la vida y la bestia la segunda
        if (phase == 1)
        {
            BossUI.Instance.UpdateHearts(currentHealth - maxHealth / 2);
        }
        else if (phase == 2)
        {
            BossUI.Instance.UpdateHearts(currentHealth);
        }

        // Al morir el jefe se quita su vida de la pantalla
        if (currentHealth <= 0)
        {
            BossUI.Instance.Hide();
        }

        if (phase == 1 && !isTransforming && currentHealth <= maxHealth / 2)
        {
            // Primero se vuelve a Idle para apagar isAttacking e isHit, si no la bestia saldria atacando
            enemy_Movement.ChangeState(EnemyState.Idle);
            enemy_Movement.enabled = false;
            rb.linearVelocity = Vector2.zero;

            isTransforming = true;
            anim.SetTrigger("ToMonster");
            AudioManager.Instance.PlaySFX(transformSound);
        }
    }

    // Lo llama un evento al final de la animacion de transformacion de la bestia
    public void StartPhase2()
    {
        phase = 2;

        // La bestia empieza siempre con sus 10 corazones, aunque el ultimo golpe al samurai se pasara
        enemy_Health.currentHealth = enemy_Health.maxHealth / 2;
        BossUI.Instance.Show(monsterPortrait);
        BossUI.Instance.UpdateHearts(enemy_Health.currentHealth);

        // La bestia es mas rapida, pega mas fuerte y en mas area
        enemy_Movement.speed = monsterSpeed;
        enemy_Movement.attackRange = monsterAttackRange;
        enemy_Movement.attackCoolDown = monsterAttackCoolDown;
        enemy_Combat.damage = monsterDamage;
        enemy_Combat.weaponRange = monsterWeaponRange;
        enemy_Combat.attackSound = monsterAttackSound;

        if (bodyCollider != null)
        {
            bodyCollider.size = monsterBodySize;
        }

        isTransforming = false;
        enemy_Movement.enabled = true;
    }

    // Dibuja en el editor la distancia a la que despierta el monje
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, wakeUpRange);
    }
}
