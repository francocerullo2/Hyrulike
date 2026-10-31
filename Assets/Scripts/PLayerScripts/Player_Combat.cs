using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public float cooldown = .5f; // tiempo entre ataques
    public float knockbackForce = 50;
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange; // radio del circulo donde pega la espada
    public float attackDistance = .7f; // lo lejos que se pone el attackPoint del jugador
    public float feetHeight = -.5f; // ajuste para que el ataque salga desde los pies y no desde el centro
    public float stunTime = .3f;
    public float knockbackTime = .15f;
    public LayerMask enemyLayer;

    private float timer;


    private void Update()
    {
        // Contador del cooldown
        if(timer > 0)
        {
            timer-= Time.deltaTime;
        }
    }

    // Se llama desde PlayerMovement al pulsar el boton de ataque
    public void Attack()
    {
        // Si el cooldown ha acabado empieza el ataque normal
        if(timer<=0)
        {
            anim.SetBool("isAttacking", true);

            timer = cooldown;

        }
        // Si vuelve a pulsar mientras ataca, se encadena el segundo golpe (combo)
        else if(anim.GetBool("isAttacking"))
        {
            anim.SetBool("isCombo", true);
        }

    }

    // Esto lo llama un evento de la animacion justo en el frame en el que pega la espada
    public void DealDamage()
    {
          // Mueve el punto de ataque hacia donde mira el jugador, empezando desde sus pies
          attackPoint.localPosition = new Vector2(anim.GetFloat("lastHorizontal"), anim.GetFloat("lastVertical")) * attackDistance + new Vector2(0, feetHeight);

          // Pilla todos los enemigos que haya dentro del circulo del ataque
          Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);

            // A cada uno le quitamos vida y lo empujamos
            foreach(Collider2D enemy in enemies)
            {
                enemy.GetComponent<Enemy_Health>().ChangeHealth(-damage);
                enemy.GetComponent<Enemy_Knockback>().Knockback(transform, knockbackForce, knockbackTime,stunTime);
            }

    }

    // Evento al final de la animacion, vuelve a dejar al jugador sin atacar
    public void FinishAttacking()
    {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isCombo", false);
    }


    // Dibuja el rango del ataque en la escena para poder ajustarlo (solo en el editor)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}
