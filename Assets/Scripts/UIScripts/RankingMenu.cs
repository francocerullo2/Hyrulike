using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// Una fila del ranking. Tiene que ser Serializable para que JsonUtility la pase a JSON y al reves
[System.Serializable]
public class RankingEntry
{
    public string name;
    public int level;   // nivel alcanzado
    public int time;    // segundos que ha durado la partida
    public bool won;    // true si se derroto al jefe
}

// Ranking online: guarda las partidas en la Realtime Database de Firebase y enseña las 10 mejores
// Se habla con Firebase por su API REST (peticiones web normales), asi no hace falta instalar su SDK
// Va en un objeto que siempre este activo (MenuCanvas), igual que GameOverMenu
public class RankingMenu : MonoBehaviour
{
    public static RankingMenu Instance;

    // La URL de la base de datos, sin la barra del final. Se copia de la consola de Firebase
    public string databaseUrl;

    // El contenido del ranking se monta una sola vez y se mete en el recuadro del panel que se abra
    public Transform rankingContent;
    public Transform gameOverBox;
    public Transform victoryBox;

    public TMP_InputField nameInput;
    public Button sendButton;
    public TMP_Text messageText; // avisos: nombre no valido, enviando, error...

    // El top 10 se escribe en tres columnas, cada una es un texto con una linea por jugador
    public TMP_Text namesText;
    public TMP_Text levelsText;
    public TMP_Text timesText;

    public int minNameLength = 3;
    public int maxNameLength = 10;
    public int rows = 10;

    private void Awake()
    {
        Instance = this;
    }

    // Lo llama GameOverMenu cuando sale el panel de Game Over o el de victoria
    public void Open(bool isVictory)
    {
        // El segundo parametro en false hace que se coloque dentro del recuadro en vez de quedarse donde estaba
        if (isVictory)
        {
            rankingContent.SetParent(victoryBox, false);
        }
        else
        {
            rankingContent.SetParent(gameOverBox, false);
        }

        sendButton.interactable = true;
        messageText.text = "";

        StartCoroutine(LoadRanking());
    }

    // Boton Enviar
    public void Send()
    {
        string playerName = nameInput.text.Trim();

        if (!IsValidName(playerName))
        {
            messageText.text = "El nombre debe tener de " + minNameLength + " a " + maxNameLength + " letras o numeros";
            return;
        }

        // Se desactiva el boton para que no se pueda enviar la misma partida dos veces
        sendButton.interactable = false;
        StartCoroutine(SendScore(playerName));
    }

    // Solo letras y numeros: asi no se cuelan simbolos raros que rompan el JSON
    private bool IsValidName(string playerName)
    {
        if (playerName.Length < minNameLength || playerName.Length > maxNameLength)
        {
            return false;
        }

        foreach (char c in playerName)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator SendScore(string playerName)
    {
        messageText.text = "Enviando...";

        RankingEntry entry = new RankingEntry();
        entry.name = playerName;
        entry.level = StatsManager.Instance.screenReached;
        entry.time = (int)StatsManager.Instance.runTime;
        entry.won = GameOverMenu.Instance.isVictory;

        // Con POST Firebase añade la partida a la lista "scores" con una clave unica
        string json = JsonUtility.ToJson(entry);
        UnityWebRequest request = UnityWebRequest.Post(databaseUrl + "/scores.json", json, "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            messageText.text = "¡Puntuacion enviada!";
            StartCoroutine(LoadRanking());
        }
        else
        {
            // Si falla (por ejemplo, sin internet) se deja volver a intentarlo
            messageText.text = "No se ha podido enviar";
            sendButton.interactable = true;
        }
    }

    IEnumerator LoadRanking()
    {
        namesText.text = "Cargando...";
        levelsText.text = "";
        timesText.text = "";

        UnityWebRequest request = UnityWebRequest.Get(databaseUrl + "/scores.json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            namesText.text = "Sin conexion";
            yield break;
        }

        List<RankingEntry> entries = ReadEntries(request.downloadHandler.text);

        // Se ordenan de mejor a peor con las reglas de CompareEntries
        entries.Sort(CompareEntries);

        ShowEntries(entries);
    }

    // Firebase devuelve todas las partidas asi: {"clave1":{...partida...},"clave2":{...partida...}}
    // JsonUtility no sabe leer esas claves, asi que se va cortando cada {...partida...} y se lee una a una
    private List<RankingEntry> ReadEntries(string json)
    {
        List<RankingEntry> entries = new List<RankingEntry>();

        // Se empieza a buscar en la posicion 1 para saltarse la llave que lo abre todo
        int start = json.IndexOf('{', 1);

        while (start != -1)
        {
            int end = json.IndexOf('}', start);
            string entryJson = json.Substring(start, end - start + 1);

            entries.Add(JsonUtility.FromJson<RankingEntry>(entryJson));

            start = json.IndexOf('{', end);
        }

        return entries;
    }

    // Devuelve un numero negativo si a va antes que b, y positivo si va despues
    // Orden: primero los que han ganado, luego el nivel mas alto y, si empatan, el menor tiempo
    private int CompareEntries(RankingEntry a, RankingEntry b)
    {
        if (a.won != b.won)
        {
            if (a.won)
            {
                return -1;
            }
            return 1;
        }

        if (a.level != b.level)
        {
            return b.level - a.level;
        }

        return a.time - b.time;
    }

    private void ShowEntries(List<RankingEntry> entries)
    {
        namesText.text = "";

        for (int i = 0; i < rows && i < entries.Count; i++)
        {
            RankingEntry entry = entries[i];

            int minutes = entry.time / 60;
            int seconds = entry.time % 60;

            namesText.text += (i + 1) + ". " + entry.name + "\n";
            timesText.text += minutes.ToString("00") + ":" + seconds.ToString("00") + "\n";

            if (entry.won)
            {
                levelsText.text += "Victoria\n";
            }
            else
            {
                levelsText.text += "Nivel " + entry.level + "\n";
            }
        }

        if (entries.Count == 0)
        {
            namesText.text = "Todavia no hay partidas";
        }
    }
}
