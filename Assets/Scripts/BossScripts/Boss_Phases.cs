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

    private bool isTransforming;

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
    }

    // Lo llama Enemy_Health cada vez que el jefe recibe daño
    public void CheckPhase(int currentHealth, int maxHealth)
    {
        if (phase == 1 && !isTransforming && currentHealth <= maxHealth / 2)
        {
            // Primero se vuelve a Idle para apagar isAttacking e isHit, si no la bestia saldria atacando
            enemy_Movement.ChangeState(EnemyState.Idle);
            enemy_Movement.enabled = false;
            rb.linearVelocity = Vector2.zero;

            isTransforming = true;
            anim.SetTrigger("ToMonster");
        }
    }

    // Lo llama un evento al final de la animacion de transformacion de la bestia
    public void StartPhase2()
    {
        phase = 2;

        // La bestia es mas rapida, pega mas fuerte y en mas area
        enemy_Movement.speed = monsterSpeed;
        enemy_Movement.attackRange = monsterAttackRange;
        enemy_Movement.attackCoolDown = monsterAttackCoolDown;
        enemy_Combat.damage = monsterDamage;
        enemy_Combat.weaponRange = monsterWeaponRange;

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
