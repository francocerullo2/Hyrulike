using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Uno solo para todo el juego: pasa de una escena a otra y lleva la musica y los efectos.
/// Usa dos AudioSource: uno para la musica en bucle y otro para los efectos.
/// </summary>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// La unica copia, para que cualquier script haga sonar algo con AudioManager.Instance.
    /// </summary>
    public static AudioManager Instance;

    public AudioSource musicSource; // musica de fondo, en bucle
    public AudioSource sfxSource;   // efectos cortos (golpes, dash...)

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip zone1Music;
    public AudioClip zone2Music;
    public AudioClip zone3Music;
    public AudioClip bossMusic; // nivel final
    public AudioClip gameOverMusic; // las pone GameOverMenu al salir su panel
    public AudioClip victoryMusic;

    [Header("SFX")]
    public AudioClip hitSound; // golpe recibido, el mismo para el jugador y los enemigos

    /// <summary>
    /// Igual que el StatsManager: solo vale el primero, los demas se borran al cargar.
    /// Tambien se apunta a sceneLoaded para cambiar la musica en cada escena.
    /// </summary>
    private void Awake()
    {
        // Igual que el StatsManager: solo vale el primero, los demas se borran al cargar
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Cada vez que se cargue una escena Unity llama a OnSceneLoaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Segun la escena que se acaba de cargar se elige la musica.
    /// Los niveles de una zona empiezan todos igual (Zona1_01, Zona1_02...), asi comparten cancion.
    /// </summary>
    /// <param name="scene">La escena que se ha cargado.</param>
    /// <param name="mode">Como se ha cargado (no se usa, pero Unity lo pide).</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Los niveles de una zona empiezan todos igual (Zona1_01, Zona1_02...), asi comparten cancion
        if (scene.name == "MainMenu")
        {
            PlayMusic(menuMusic);
        }
        else if (scene.name.StartsWith("Zona1"))
        {
            PlayMusic(zone1Music);
        }
        else if (scene.name.StartsWith("Zona2"))
        {
            PlayMusic(zone2Music);
        }
        else if (scene.name.StartsWith("Zona3"))
        {
            PlayMusic(zone3Music);
        }
        else if (scene.name == "FinalLevel")
        {
            PlayMusic(bossMusic);
        }
    }

    /// <summary>
    /// Pone una cancion de fondo. Si ya esta sonando esa no la toca, asi no vuelve a empezar al cambiar de escena.
    /// </summary>
    /// <param name="clip">La cancion.</param>
    public void PlayMusic(AudioClip clip)
    {
        // Si ya esta sonando esa cancion no se toca, asi no vuelve a empezar al cambiar de escena
        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }
        musicSource.clip = clip;
        musicSource.Play();
    }

    /// <summary>
    /// Lo llaman los demas scripts para que suene un efecto.
    /// PlayOneShot deja que suenen varios efectos a la vez sin cortarse.
    /// </summary>
    /// <param name="clip">El sonido.</param>
    public void PlaySFX(AudioClip clip)
    {
        // PlayOneShot deja que suenen varios efectos a la vez sin cortarse
        sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Igual pero mas bajito, para sonidos que se repiten mucho como los pasos.
    /// </summary>
    /// <param name="clip">El sonido.</param>
    /// <param name="volume">Volumen de 0 a 1.</param>
    public void PlaySFX(AudioClip clip, float volume)
    {
        sfxSource.PlayOneShot(clip, volume);
    }
}
