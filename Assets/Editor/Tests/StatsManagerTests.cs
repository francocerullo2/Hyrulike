using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Pruebas de unidad de StatsManager: que las mejoras cambien bien las estadisticas
/// y que al empezar una partida nueva se borre lo temporal pero no lo permanente.
/// Se lanzan desde Window -> General -> Test Runner, en la pestaña EditMode.
/// </summary>
public class StatsManagerTests
{
    private GameObject statsObject;
    private StatsManager stats;

    /// <summary>
    /// Antes de cada prueba se crea un StatsManager nuevo con unos valores base fijos,
    /// asi las pruebas no dependen de lo que haya puesto en el Inspector.
    /// No se usa UnlockPermanent para no tocar el progreso guardado (PlayerPrefs).
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        statsObject = new GameObject("StatsManagerTest");
        stats = statsObject.AddComponent<StatsManager>();

        stats.maxHealth = 5;
        stats.currentHealth = 5;
        stats.damage = 1;
        stats.upgradeLevels = new int[12];
    }

    /// <summary>
    /// Despues de cada prueba se borra el objeto, para que no se quede en la escena.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(statsObject);
    }

    /// <summary>Prueba 9: la mejora Corazon da un corazon mas de vida maxima, y aparece ya lleno.</summary>
    [Test]
    public void MejoraCorazon_SubeLaVidaMaxima()
    {
        stats.ApplyUpgrade(StatsManager.HEART);

        Assert.AreEqual(6, stats.MaxHealth());
        Assert.AreEqual(6, stats.currentHealth);
    }

    /// <summary>Prueba 10: cada nivel de Fuerza suma 1 de daño (1 base + 2 niveles = 3).</summary>
    [Test]
    public void MejoraFuerza_SubeElDaño()
    {
        stats.ApplyUpgrade(StatsManager.STRENGTH);
        stats.ApplyUpgrade(StatsManager.STRENGTH);

        Assert.AreEqual(3, stats.Damage());
    }

    /// <summary>Prueba 11: sin el Amuleto no hay segunda oportunidad, y al cogerlo si.</summary>
    [Test]
    public void Amuleto_SoloSiSeHaCogido()
    {
        Assert.IsFalse(stats.HasAmulet());

        stats.ApplyUpgrade(StatsManager.AMULET);

        Assert.IsTrue(stats.HasAmulet());
    }

    /// <summary>
    /// Prueba 12: ResetRun deja la partida como nueva (vida llena, nivel 1, tiempo 0 y sin mejoras temporales)
    /// pero las mejoras permanentes se mantienen.
    /// </summary>
    [Test]
    public void ResetRun_BorraLoTemporalPeroNoLoPermanente()
    {
        // Una partida a medias: con arco, varias mejoras temporales, en el nivel 7 y con poca vida
        stats.hasBow = true;
        stats.ApplyUpgrade(StatsManager.HEART);
        stats.ApplyUpgrade(StatsManager.SPEED);
        stats.screenReached = 7;
        stats.runTime = 420;
        stats.currentHealth = 1;

        stats.ResetRun();

        Assert.AreEqual(5, stats.currentHealth);
        Assert.AreEqual(1, stats.screenReached);
        Assert.AreEqual(0, stats.runTime);
        Assert.AreEqual(5, stats.MaxHealth()); // el corazon extra se ha ido

        // Todas las mejoras temporales vuelven a 0
        foreach (int level in stats.upgradeLevels)
        {
            Assert.AreEqual(0, level);
        }

        // El arco sigue ahi
        Assert.IsTrue(stats.hasBow);
    }
}
