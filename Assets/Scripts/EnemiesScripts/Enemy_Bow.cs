using UnityEngine;

// Ataque del arquero: en vez de pegar con un arma dispara una flecha hacia el jugador
// Va en lugar de Enemy_Combat, el resto (moverse, patrullar, cooldown) lo hace Enemy_Movement
public class Enemy_Bow : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject arrowPrefab;

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Lo llama el evento de la animacion de disparo, se llama Attack igual que en Enemy_Combat
    public void Attack()
    {
        Arrow arrow = Instantiate(arrowPrefab, launchPoint.position, Quaternion.identity).GetComponent<Arrow>();
        arrow.direction = (player.position - launchPoint.position).normalized;
    }
}
