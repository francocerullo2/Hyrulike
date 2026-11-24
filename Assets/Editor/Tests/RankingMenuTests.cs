using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Pruebas de unidad del ranking: la validacion del nombre y el calculo de la puntuacion.
/// Se lanzan desde Window -> General -> Test Runner, en la pestaña EditMode.
/// </summary>
public class RankingMenuTests
{
    private GameObject menuObject;
    private RankingMenu ranking;

    /// <summary>
    /// Antes de cada prueba se crea un RankingMenu nuevo en un objeto vacio.
    /// En EditMode no se llama a Start, asi que no intenta conectarse a Firebase.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        menuObject = new GameObject("RankingMenuTest");
        ranking = menuObject.AddComponent<RankingMenu>();
    }

    /// <summary>
    /// Despues de cada prueba se borra el objeto, para que no se quede en la escena.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(menuObject);
    }

    // ---- Validacion del nombre ----

    /// <summary>Prueba 1: un nombre normal de letras vale.</summary>
    [Test]
    public void NombreCorrecto_EsValido()
    {
        Assert.IsTrue(ranking.IsValidName("Franco"));
    }

    /// <summary>Prueba 2: con menos de 3 letras no vale.</summary>
    [Test]
    public void NombreCorto_NoEsValido()
    {
        Assert.IsFalse(ranking.IsValidName("ab"));
    }

    /// <summary>Prueba 3: con mas de 10 letras no vale.</summary>
    [Test]
    public void NombreLargo_NoEsValido()
    {
        Assert.IsFalse(ranking.IsValidName("nombremuylargo"));
    }

    /// <summary>Prueba 4: con simbolos o espacios en medio no vale.</summary>
    [Test]
    public void NombreConSimbolos_NoEsValido()
    {
        Assert.IsFalse(ranking.IsValidName("Fran$"));
        Assert.IsFalse(ranking.IsValidName("Fran co"));
    }

    /// <summary>Prueba 5: justo 3 y justo 10 letras valen (los limites estan incluidos), y con numeros tambien.</summary>
    [Test]
    public void NombreEnLosLimites_EsValido()
    {
        Assert.IsTrue(ranking.IsValidName("abc"));
        Assert.IsTrue(ranking.IsValidName("abcdefghij"));
        Assert.IsTrue(ranking.IsValidName("Jugador1"));
    }

    // ---- Puntuacion ----

    /// <summary>Prueba 6: muere en el nivel 6 a los 209 segundos -> 6 * 100000 - 209.</summary>
    [Test]
    public void PuntuacionAlPerder()
    {
        Assert.AreEqual(599791, ranking.CalculateScore(6, 209, false));
    }

    /// <summary>Prueba 7: gana en el nivel 10 a los 1500 segundos -> 10 * 100000 - 1500 + 100000.</summary>
    [Test]
    public void PuntuacionAlGanar()
    {
        Assert.AreEqual(1098500, ranking.CalculateScore(10, 1500, true));
    }

    /// <summary>
    /// Prueba 8: el orden del ranking. Ganar queda por encima de morir contra el jefe aunque se tarde mas,
    /// y llegar mas lejos queda por encima de ser mas rapido.
    /// </summary>
    [Test]
    public void OrdenDelRanking()
    {
        int ganaLento = ranking.CalculateScore(10, 3000, true);
        int pierdeRapido = ranking.CalculateScore(10, 600, false);
        Assert.Greater(ganaLento, pierdeRapido);

        int nivel5Lento = ranking.CalculateScore(5, 3000, false);
        int nivel4Rapido = ranking.CalculateScore(4, 100, false);
        Assert.Greater(nivel5Lento, nivel4Rapido);
    }
}
