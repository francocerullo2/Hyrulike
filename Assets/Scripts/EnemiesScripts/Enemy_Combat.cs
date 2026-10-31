using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;

    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;

    // Lo llama un evento de la animacion de ataque del enemigo
    public void Attack()
    {
        // Miramos si el jugador esta dentro del rango del arma
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        // Si le ha dado, lo empuja y le quita vida
        if(hits.Length > 0)
        {
            hits[0].GetComponent<PlayerMovement>().Knockback(transform, knockbackForce,stunTime);
            hits[0].GetComponent<PlayerHealth>().ChangeHealth(-damage);


        }
    }
        
    
    // Circulo amarillo en el editor para ver hasta donde llega el ataque
    private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
}
}


