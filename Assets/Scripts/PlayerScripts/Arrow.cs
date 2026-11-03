using UnityEngine;

// Flecha que va recta y se borra sola al rato
// La usan el jugador (enemyLayer puesto) y el arquero enemigo (playerLayer puesto)
public class Arrow : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 direction = Vector2.right;
    public float lifeSpan = 2; // segundos que dura la flecha antes de desaparecer
    public float speed;
    public int damage;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;
    public LayerMask enemyLayer;
    public LayerMask playerLayer; // en la flecha del jugador se deja en Nothing para que no se de a si mismo
    public LayerMask obstacleLayer;

    public SpriteRenderer sr;
    public Sprite buriedSprite;

    // Al crearse le damos la velocidad y programamos que se destruya pasado el lifeSpan
    void Start()
    {
        rb.linearVelocity = direction* speed;
        RotateArrow();
        Destroy(gameObject, lifeSpan);

    }

    private void RotateArrow()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if((enemyLayer.value & (1 << collision.gameObject.layer)) >0)
        {
            collision.gameObject.GetComponent<Enemy_Health>().ChangeHealth(-damage);
            collision.gameObject.GetComponent<Enemy_Knockback>().Knockback(transform, knockbackForce, knockbackTime, stunTime);

        }
        else if ((playerLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            HitPlayer(collision.gameObject);
        }
        else if ((obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            AttachToTarget(collision.gameObject.transform);
        }
    }

    // El collider del enemigo es trigger, asi que el golpe a enemigos llega por aqui y no por OnCollisionEnter2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((enemyLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            collision.gameObject.GetComponent<Enemy_Health>().ChangeHealth(-damage);
            collision.gameObject.GetComponent<Enemy_Knockback>().Knockback(transform, knockbackForce, knockbackTime, stunTime);
            Destroy(gameObject);
        }
        else if ((playerLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            HitPlayer(collision.gameObject);
        }
    }

    // Igual que el ataque de Enemy_Combat: le quita vida al jugador, lo empuja y la flecha desaparece
    private void HitPlayer(GameObject player)
    {
        player.GetComponent<PlayerHealth>().ChangeHealth(-damage, transform);
        player.GetComponent<PlayerMovement>().Knockback(transform, knockbackForce, stunTime);
        Destroy(gameObject);
    }

    private void AttachToTarget(Transform target)
    {
        sr.sprite = buriedSprite;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Para que la flecha clavada no estorbe al jugador ni a otras flechas
        GetComponent<Collider2D>().enabled = false;

        transform.SetParent(target);
    }
}
