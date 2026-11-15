using UnityEngine;

public class Player_Bow : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject arrowPrefab;
    public PlayerMovement playerMovement;

    private Vector2 aimDirection = Vector2.right;

    public float launchDistance = .66f; // lo lejos del jugador que sale la flecha
    public float shootCooldown = .5f;
    private float shootTimer;
    private bool arrowReleased; // para que en cada disparo salga una sola flecha

    public Animator anim;
    public AudioClip shootSound;

    // Update se llama una vez por frame
    void Update()
    {
        // En pausa no se puede apuntar ni disparar
        if (PauseMenu.isPaused)
        {
            return;
        }

        shootTimer -= Time.deltaTime;

        // Mientras tensa el arco no cambia de direccion, asi la flecha sale hacia donde apunta la animacion
        if (playerMovement.isShooting == false)
        {
            HandleAiming();
        }

        // Solo se puede disparar si ya ha conseguido el arco (mejora permanente)
        if(Input.GetButtonDown("Shoot") && StatsManager.Instance.hasBow && shootTimer <= 0 && playerMovement.isShooting == false && playerMovement.isGuarding == false)
        {
            playerMovement.isShooting = true;
            arrowReleased = false;
            anim.SetBool("isShooting", true);
        }
    }

    // Lo llama un evento de la animacion en el frame en el que suelta la cuerda
    public void ReleaseArrow()
    {
        // En diagonal el blend tree mezcla varias animaciones y cada una lanza su evento, solo vale el primero
        if (arrowReleased)
        {
            return;
        }
        arrowReleased = true;
        AudioManager.Instance.PlaySFX(shootSound);

        // Coloca el punto de salida hacia donde apunta (lado, arriba, abajo o diagonal)
        // Con position y no localPosition, asi da igual que el jugador este girado con el Flip
        launchPoint.position = transform.position + (Vector3)(aimDirection * launchDistance);

        Arrow arrow = Instantiate(arrowPrefab, launchPoint.position, Quaternion.identity).GetComponent<Arrow>();
        arrow.direction = aimDirection;
        arrow.damage += StatsManager.Instance.ArrowDamageBonus(); // con la mejora Kunai la flecha hace mas daño
        shootTimer = shootCooldown * StatsManager.Instance.ShootCooldownMultiplier(); // con la mejora Cañón de agua se dispara mas rapido
    }

    // Evento al final de la animacion de disparo, vuelve a la pose normal y deja moverse
    public void FinishShooting()
    {
        anim.SetBool("isShooting", false);
        playerMovement.isShooting = false;
    }

    private void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if(horizontal != 0 || vertical != 0)
        {
            aimDirection = new Vector2(horizontal, vertical).normalized;
            anim.SetFloat("aimX", aimDirection.x);
            anim.SetFloat("aimY", aimDirection.y);
        }
    }
}
