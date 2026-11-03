# Diagrama de clases — Hyrulike

Todas las clases heredan de `MonoBehaviour` salvo `EnemyState`, `Upgrade` y `RankingEntry` (no se dibuja la herencia para no cargar el diagrama).
Leyenda de relaciones: `-->` usa/tiene una referencia, `..>` depende de (llama o crea), `*--` composición.

## 1. Jugador

```mermaid
classDiagram
    direction LR

    class PlayerMovement {
        +int facingDirection
        +bool isShooting
        +bool isGuarding
        +bool isDashing
        +float dashCooldown
        -bool isKnockedBack
        -Rigidbody2D rb
        -Animator anim
        +Knockback(Transform enemy, float force, float stunTime)
        -Dash()
        -Flip()
    }

    class Player_Combat {
        +float cooldown
        +float attackDistance
        +LayerMask enemyLayer
        -float timer
        +Attack()
        +DealDamage()
        +FinishAttacking()
    }

    class Player_Bow {
        +Transform launchPoint
        +GameObject arrowPrefab
        +float launchDistance
        +float shootCooldown
        -Vector2 aimDirection
        +ReleaseArrow()
        +FinishShooting()
        -HandleAiming()
    }

    class Player_Guard {
        +float guardAngle
        +float damageReduction
        -bool isGuarding
        +StartGuard()
        +StopGuard()
        +BlocksHit(Transform attacker) bool
    }

    class PlayerHealth {
        +float invulnerabilityTime
        +GameObject deadPrefab
        -float invulnerabilityTimer
        +ChangeHealth(int amount)
    }

    class Arrow {
        +Vector2 direction
        +float speed
        +int damage
        +float lifeSpan
        +LayerMask enemyLayer
        +LayerMask playerLayer
        +LayerMask obstacleLayer
        -HitPlayer(GameObject player)
        -AttachToTarget(Transform target)
    }

    PlayerMovement --> Player_Combat : ataca
    Player_Bow --> PlayerMovement : bloquea movimiento
    Player_Guard --> PlayerMovement : bloquea movimiento
    Player_Bow ..> Arrow : crea
    PlayerHealth ..> Player_Guard : consulta si bloquea
```

## 2. Enemigos y jefe

```mermaid
classDiagram
    direction LR

    class EnemyState {
        <<enumeration>>
        Idle
        Chasing
        Attacking
        Knockback
    }

    class Enemy_Movement {
        +float speed
        +float attackRange
        +float attackCoolDown
        +float playerDetectRange
        +bool patrol
        +bool chasePlayer
        +Vector2[] patrolPoints
        +float pauseDuration
        -EnemyState enemyState
        -bool isPatrolling
        +ChangeState(EnemyState newState)
        -CheckForPlayer()
        -Chase()
        -Patrol()
        -PatrolOrIdle()
        -SetAttackDirection()
        -FaceTarget(Vector2 target)
        -Flip()
    }

    class Enemy_Combat {
        +int damage
        +Transform attackPoint
        +float weaponRange
        +float attackDistance
        +float knockbackForce
        +LayerMask playerLayer
        +Attack()
    }

    class Enemy_Bow {
        +Transform launchPoint
        +GameObject arrowPrefab
        +Attack()
    }

    class Enemy_Explosion {
        +int damage
        +float explosionRange
        +float fuseTime
        +GameObject explosionPrefab
        -bool isLit
        +LightFuse()
        -Explode()
    }

    class Enemy_Health {
        +int currentHealth
        +int maxHealth
        +GameObject deadPrefab
        -bool isDead
        +ChangeHealth(int amount)
    }

    class Enemy_Knockback {
        +Knockback(Transform attacker, float force, float knockbackTime, float stunTime)
        -StunTimer(float knockbackTime, float stunTime)
    }

    class Contact_Damage {
        +int damage
    }

    class Boss_Phases {
        +float samuraiHealthPercent
        +float monsterHealthPercent
        +float chargeSpeed
        +float healAmount
        -int currentPhase
        -CheckPhase()
        -ChangePhase(int phase)
        +ChargeAttack()
        +Heal()
    }

    Enemy_Movement --> EnemyState
    Enemy_Knockback --> Enemy_Movement : cambia estado
    Enemy_Health ..> Enemy_Explosion : enciende la mecha
    Enemy_Explosion ..> Enemy_Movement
    Enemy_Combat ..> Enemy_Movement : direccion del ataque
    Enemy_Bow ..> Arrow : crea
    Boss_Phases --> Enemy_Health : lee la vida y se cura
    Boss_Phases --> Enemy_Movement : ajusta velocidad y rango
    Boss_Phases --> Enemy_Combat : ajusta el daño
```

El jefe no tiene una clase "Boss" propia: es un prefab con los mismos componentes que los enemigos (`Enemy_Movement`, `Enemy_Combat`, `Enemy_Health`, `Enemy_Knockback`) más `Boss_Phases`, que lo hace cambiar de forma (Monje → Samurái → Monstruo) al bajar de vida y añade la curación y el ataque cargado.

## 3. Interacción jugador ↔ enemigos

```mermaid
classDiagram
    direction LR
    class Player_Combat
    class Arrow
    class Enemy_Combat
    class Enemy_Explosion
    class Contact_Damage
    class Enemy_Health
    class Enemy_Knockback
    class PlayerHealth
    class PlayerMovement

    Player_Combat ..> Enemy_Health : quita vida
    Player_Combat ..> Enemy_Knockback : empuja
    Arrow ..> Enemy_Health : quita vida
    Arrow ..> Enemy_Knockback : empuja
    Arrow ..> PlayerHealth : quita vida
    Arrow ..> PlayerMovement : empuja
    Enemy_Combat ..> PlayerHealth : quita vida
    Enemy_Combat ..> PlayerMovement : empuja
    Enemy_Explosion ..> PlayerHealth : quita vida
    Enemy_Explosion ..> PlayerMovement : empuja
    Contact_Damage ..> PlayerHealth : quita vida
```

## 4. Partida, niveles, ranking y sonido

```mermaid
classDiagram
    direction LR

    class StatsManager {
        <<singleton>>
        +StatsManager Instance$
        +int damage
        +int speed
        +int dashSpeed
        +int maxHealth
        +int currentHealth
        +bool hasBow
        +bool hasDash
        +bool hasGuard
        +int[] upgradeLevels
        +int screenReached
        +float runTime
        +int enemiesKilled
        +UpgradesObtained() int
        +ApplyUpgrade(int index)
        +ResetRun()
    }

    class LevelManager {
        <<singleton>>
        +LevelManager Instance$
        +string[] levelScenes
        +LoadNextLevel()
    }

    class LevelExit {
        -OnTriggerEnter2D(Collider2D other)
    }

    class Upgrade {
        +string upgradeName
        +string description
        +Sprite icon
        +int index
        +int maxLevel
    }

    class UpgradeChoiceMenu {
        <<singleton>>
        +UpgradeChoiceMenu Instance$
        +Upgrade[] allUpgrades
        +int optionsToShow
        +Show()
        +ChooseUpgrade(int option)
        +GetRandomOptions(int amount) List~Upgrade~
    }

    class RankingEntry {
        +string playerName
        +int screenReached
        +float runTime
        +int enemiesKilled
        +int upgrades
        +CompareTo(RankingEntry other) int
    }

    class RankingManager {
        <<singleton>>
        +RankingManager Instance$
        +int maxEntries
        +SubmitEntry(RankingEntry entry)
        +LoadTopEntries(int amount)
        +IsValidName(string name) bool
    }

    class AudioManager {
        <<singleton>>
        +AudioManager Instance$
        +AudioSource musicSource
        +AudioSource sfxSource
        +float volume
        +PlaySFX(AudioClip clip)
        +PlayMusic(AudioClip clip)
        +SetVolume(float value)
    }

    LevelExit ..> UpgradeChoiceMenu : abre al terminar el nivel
    UpgradeChoiceMenu *-- Upgrade
    UpgradeChoiceMenu ..> StatsManager : aplica la mejora
    UpgradeChoiceMenu ..> LevelManager : pasa de nivel
    LevelManager ..> StatsManager : suma screenReached
    RankingManager *-- RankingEntry
    RankingManager ..> StatsManager : lee los datos de la partida
```

El ranking ordena por `screenReached` (de más a menos) y, si empatan, por `runTime` (de menos a más). `enemiesKilled` y `upgrades` son solo informativos. Esa comparación es `RankingEntry.CompareTo`.

## 5. Interfaz

```mermaid
classDiagram
    direction LR

    class MainMenu {
        +string firstLevel
        +Play()
        +OpenOptions()
        +CloseOptions()
        +Quit()
    }

    class PauseMenu {
        +bool isPaused$
        +Pause()
        +Resume()
        +OpenSettings()
        +CloseSettings()
        +ExitToMenu()
    }

    class SettingsMenu {
        +VolumeUp()
        +VolumeDown()
        +ToggleFullscreen()
    }

    class GameOverMenu {
        <<singleton>>
        +GameOverMenu Instance$
        +bool isGameOver$
        +float delay
        +Show()
        +Retry()
        +ExitToMenu()
        -UpdateTexts()
    }

    class RankingUI {
        +TMP_InputField nameInput
        +Transform entriesParent
        +SendScore()
        +ShowEntries(List~RankingEntry~ entries)
    }

    class HeartsUI {
        <<singleton>>
        +HeartsUI Instance$
        +Image[] hearts
        +UpdateHearts(int currentHealth, int maxHealth)
    }

    class UpgradesMenu {
        +UpgradeSlot[] tempSlots
        +UpdateSlots()
    }

    class UpgradeSlot {
        +Image icon
        +GameObject[] levelIcons
        +SetLevel(int level)
    }

    MainMenu --> SettingsMenu
    PauseMenu --> SettingsMenu
    PauseMenu --> UpgradesMenu
    UpgradesMenu *-- UpgradeSlot
    GameOverMenu --> RankingUI
    GameOverMenu ..> PauseMenu : bloquea la pausa
    GameOverMenu ..> StatsManager
    RankingUI ..> RankingManager
    UpgradesMenu ..> StatsManager
    PlayerHealth ..> HeartsUI
    PlayerHealth ..> GameOverMenu : al morir
    PlayerHealth ..> StatsManager
    SettingsMenu ..> AudioManager
```
