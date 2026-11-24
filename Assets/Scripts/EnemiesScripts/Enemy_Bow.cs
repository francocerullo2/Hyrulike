using UnityEngine;

/// <summary>
/// Ataque del arquero: en vez de pegar con un arma dispara una flecha hacia el jugador.
/// Va en lugar de Enemy_Combat, el resto (moverse, patrullar, cooldown) lo hace Enemy_Movement.
/// </summary>
public class Enemy_Bow : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject arrowPrefab;
    public AudioClip shootSound;

    private Transform player;

    /// <summary>Busca al jugador por su Tag para saber hacia donde disparar.</summary>
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    /// <summary>
    /// Crea una flecha que va hacia donde esta el jugador.
    /// Lo llama el evento de la animacion de disparo, se llama Attack igual que en Enemy_Combat para usar el mismo evento.
    /// </summary>
    public void Attack()
    {
        AudioManager.Instance.PlaySFX(shootSound);
        Arrow arrow = Instantiate(arrowPrefab, launchPoint.position, Quaternion.identity).GetComponent<Arrow>();
        arrow.direction = (player.position - launchPoint.position).normalized;
    }
}
