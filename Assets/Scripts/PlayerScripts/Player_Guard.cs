using UnityEngine;

/// <summary>
/// Guardia del jugador: mientras se mantiene pulsado se queda quieto y bloquea los golpes que vienen del lado al que mira.
/// El escudo solo cubre de lado, asi que los ataques que vienen de arriba o de abajo le siguen dando.
/// Solo funciona si ya tiene la guardia (mejora permanente).
/// </summary>
public class Player_Guard : MonoBehaviour
{
    public Animator anim;
    public PlayerMovement playerMovement;

    public float blockAngle = 60; // grados por encima y por debajo del frente que cubre el escudo (90 seria todo el lado)
    public float blockedKnockbackMultiplier = .3f; // un golpe bloqueado empuja menos (0 = nada, 1 = igual que sin guardia)

    /// <summary>Activa o quita la guardia segun si se mantiene pulsado el boton.</summary>
    void Update()
    {
        // En pausa no se puede cambiar de guardia
        if (PauseMenu.isPaused)
        {
            return;
        }

        // Solo se puede poner en guardia si tiene la mejora y no esta atacando ni disparando
        bool wantsToGuard = Input.GetButton("Guard")
            && StatsManager.Instance.hasGuard
            && playerMovement.isShooting == false
            && anim.GetBool("isAttacking") == false;

        playerMovement.isGuarding = wantsToGuard;
        anim.SetBool("isGuarding", wantsToGuard);
    }

    /// <summary>
    /// Dice si el escudo para un golpe, mirando el angulo entre hacia donde mira el jugador y donde esta el atacante.
    /// </summary>
    /// <param name="attackerPosition">Donde esta quien pega.</param>
    /// <returns>True si el golpe se bloquea.</returns>
    public bool BlocksHit(Vector2 attackerPosition)
    {
        if (playerMovement.isGuarding == false)
        {
            return false;
        }

        // Hacia donde mira el jugador: derecha si la escala es positiva, izquierda si es negativa
        Vector2 facing = new Vector2(Mathf.Sign(transform.localScale.x), 0);
        Vector2 toAttacker = attackerPosition - (Vector2)transform.position;

        // Con la mejora Defensa cubre mas angulo, pero como mucho 85 grados para que arriba o abajo (90) nunca se bloquee
        float angle = Mathf.Min(blockAngle + StatsManager.Instance.GuardAngleBonus(), 85);

        // Si el atacante esta dentro del angulo del escudo, bloquea
        return Vector2.Angle(facing, toAttacker) <= angle;
    }

    /// <summary>
    /// Lo que se multiplica el empujon de un golpe bloqueado (con la mejora Defensa empuja todavia menos).
    /// </summary>
    /// <returns>Un numero entre 0 y 1.</returns>
    public float BlockedKnockbackMultiplier()
    {
        return blockedKnockbackMultiplier * StatsManager.Instance.GuardKnockbackMultiplier();
    }
}
