using UnityEngine;

// Flecha que dispara el jugador, va recta y se borra sola al rato
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
        else if ((obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            AttachToTarget(collision.gameObject.transform);
        }
    }

    private void AttachToTarget(Transform target)
    {
        sr.sprite = buriedSprite;

        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;

        transform.SetParent(target);
    }
}
