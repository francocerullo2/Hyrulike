using UnityEngine;

// Hace daño al jugador cuando lo toca (espinas, fuego, rocas...)
public class Contact_Damage : MonoBehaviour
{
    public int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().ChangeHealth(-damage);
        }
    }
}
