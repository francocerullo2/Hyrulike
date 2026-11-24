using UnityEngine;

/// <summary>
/// Ataque con espada del jugador, con combo de dos golpes y cooldown.
/// El daño lo hace un evento de la animacion (DealDamage) para que cuadre con el frame del espadazo.
/// Va en el Player.
/// </summary>
public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public float cooldown = .5f; // tiempo entre ataques
    public Transform attackPoint;
    public float attackDistance = .7f; // lo lejos que se pone el attackPoint del jugador
    public float feetHeight = -.5f; // ajuste para que el ataque salga desde los pies y no desde el centro
    public LayerMask enemyLayer;

    public AudioClip swordSound; // primer espadazo
    public AudioClip comboSound; // segundo espadazo del combo

    private float timer;
    private bool isSecondSlash; // para saber cual de los dos sonidos toca


    /// <summary>Va bajando el contador del cooldown.</summary>
    private void Update()
    {
        // Contador del cooldown
        if(timer > 0)
        {
            timer-= Time.deltaTime;
        }
    }

    /// <summary>
    /// Empieza un ataque o encadena el combo si ya esta atacando.
    /// Se llama desde PlayerMovement al pulsar el boton de ataque.
    /// </summary>
    public void Attack()
    {
        // Si vuelve a pulsar mientras ataca, se encadena el segundo golpe (combo)
        if(anim.GetBool("isAttacking"))
        {
            anim.SetBool("isCombo", true);
        }
        // Si el cooldown ha acabado empieza el ataque normal
        else if(timer<=0)
        {
            anim.SetBool("isAttacking", true);
        }

    }

    /// <summary>
    /// Hace daño a todos los enemigos que haya dentro del circulo del ataque y los empuja.
    /// Lo llama un evento de la animacion justo en el frame en el que pega la espada.
    /// </summary>
    public void DealDamage()
    {
          // El primer golpe del ataque suena de una forma y el del combo de otra
          if (isSecondSlash)
          {
              AudioManager.Instance.PlaySFX(comboSound);
          }
          else
          {
              AudioManager.Instance.PlaySFX(swordSound);
          }
          isSecondSlash = true;

          // Mueve el punto de ataque hacia donde mira el jugador, empezando desde sus pies
          attackPoint.localPosition = new Vector2(anim.GetFloat("lastHorizontal"), anim.GetFloat("lastVertical")) * attackDistance + new Vector2(0, feetHeight);

          // Pilla todos los enemigos que haya dentro del circulo del ataque
          Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.WeaponRange(), enemyLayer);

            // A cada uno le quitamos vida y lo empujamos
            foreach(Collider2D enemy in enemies)
            {
                enemy.GetComponent<Enemy_Health>().ChangeHealth(-StatsManager.Instance.Damage());
                enemy.GetComponent<Enemy_Knockback>().Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime, StatsManager.Instance.stunTime);
            }

    }

    /// <summary>
    /// Evento al final de la animacion, vuelve a dejar al jugador sin atacar.
    /// El cooldown empieza aqui para que el golpe y el combo cuenten como un solo ataque.
    /// </summary>
    public void FinishAttacking()
    {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isCombo", false);
            isSecondSlash = false;
            timer = cooldown * StatsManager.Instance.AttackCooldownMultiplier(); // con la mejora Corte se ataca mas rapido
    }


    /// <summary>
    /// Dibuja el rango del ataque en la escena para poder ajustarlo (solo en el editor).
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // En el editor sin darle a Play el StatsManager todavia no existe
        if (StatsManager.Instance == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, StatsManager.Instance.WeaponRange());
    }
}
