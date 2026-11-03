using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;
    public float attackDistance = 1; // lo lejos que se pone el attackPoint del enemigo

    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Lo llama un evento de la animacion de ataque del enemigo
    public void Attack()
    {
        // Pone el punto de ataque hacia donde ataca: lado (1,0), arriba (0,1) o abajo (0,-1)
        attackPoint.localPosition = new Vector2(anim.GetFloat("attackX"), anim.GetFloat("attackY")) * attackDistance;

        // Barre un circulo desde el enemigo hasta el attackPoint: pega a lo largo de todo el arma, no solo en la punta
        // Asi si el jugador esta pegado al enemigo la lanza no le atraviesa
        Vector2 direction = attackPoint.position - transform.position;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, weaponRange, direction.normalized, direction.magnitude, playerLayer);

        // Si le ha dado, lo empuja y le quita vida
        if(hits.Length > 0)
        {
            hits[0].collider.GetComponent<PlayerMovement>().Knockback(transform, knockbackForce,stunTime);
            hits[0].collider.GetComponent<PlayerHealth>().ChangeHealth(-damage);


        }
    }
        
    
    // Zona del ataque en amarillo en el editor: un circulo en el enemigo, otro en la punta y la linea que los une
    private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, weaponRange);
    Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    Gizmos.DrawLine(transform.position, attackPoint.position);
}
}


