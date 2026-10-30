using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public float cooldown = 2;
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;

    public float knockbackForce;
    public float stunTime;
    public LayerMask enemyLayer;

    private float timer;


    private void Update()
    {
        if(timer > 0)
        {
            timer-= Time.deltaTime;
        }
    }
    public void Attack()
    {
        if(timer<=0)
        {
            anim.SetBool("isAttacking", true);

          

            timer = cooldown;

        }
        
    }

    public void DealDamage()
    {
          Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);

            if(enemies.Length > 0)
            {
                enemies[0].GetComponent<Enemy_Health>().ChangeHealth(-damage);
            }

    }

    public void FinishAttacking()
    {
            anim.SetBool("isAttacking", false);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}
