using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ranking online: guarda las partidas en Firestore (Firebase) y enseña las 10 mejores.
/// Cada partida es un documento de la coleccion "scores" con nombre, nivel, tiempo, si ha ganado y puntuacion.
/// Va en un objeto que siempre este activo (MenuCanvas), igual que GameOverMenu.
/// </summary>
public class RankingMenu : MonoBehaviour
{
    /// <summary>
    /// La unica copia del ranking, para llamarlo desde otros scripts con RankingMenu.Instance.
    /// </summary>
    public static RankingMenu Instance;

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

    /// <summary>Letras minimas que tiene que tener el nombre.</summary>
    public int minNameLength = 3;
    /// <summary>Letras maximas que puede tener el nombre (las mismas que dejan las reglas de Firestore).</summary>
    public int maxNameLength = 10;
    /// <summary>Cuantas partidas se enseñan en el ranking.</summary>
    public int rows = 10;

    private FirebaseFirestore db;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // La conexion con la base de datos. Los datos del proyecto los lee solo de google-services.json
        db = FirebaseFirestore.DefaultInstance;
    }

    /// <summary>
    /// Enseña el ranking dentro del panel que toque y carga las 10 mejores partidas.
    /// Lo llama GameOverMenu cuando sale el panel de Game Over o el de victoria.
    /// </summary>
    /// <param name="isVictory">True si se abre en la pantalla de victoria, false si es el Game Over.</param>
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

        LoadRanking();
    }

    /// <summary>
    /// Boton Enviar: comprueba el nombre y sube la partida a Firestore.
    /// Pasos: valida el nombre, desactiva el boton, junta los datos de la partida,
    /// los envia y cuando llega la respuesta enseña el mensaje y recarga el ranking.
    /// </summary>
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
        messageText.text = "Enviando...";

        int level = StatsManager.Instance.screenReached;
        int time = (int)StatsManager.Instance.runTime;
        bool won = GameOverMenu.Instance.isVictory;

        // Los datos de la partida, cada uno con su nombre de campo
        Dictionary<string, object> entry = new Dictionary<string, object>();
        entry["name"] = playerName;
        entry["level"] = level;
        entry["time"] = time;
        entry["won"] = won;
        entry["score"] = CalculateScore(level, time, won);

        // Add crea un documento nuevo en la coleccion. La respuesta llega mas tarde, por eso va en ContinueWith
        db.Collection("scores").AddAsync(entry).ContinueWithOnMainThread(task =>
        {
            // Si mientras tanto se ha cambiado de escena, este menu ya no existe
            if (this == null)
            {
                return;
            }

            if (task.IsFaulted)
            {
                // Si falla se deja volver a intentarlo
                messageText.text = "No se ha podido enviar";
                sendButton.interactable = true;
            }
            else
            {
                messageText.text = "¡Puntuacion enviada!";
                LoadRanking();
            }
        });
    }

    /// <summary>
    /// Mira si el nombre vale para el ranking: solo letras y numeros, y ni muy corto ni muy largo.
    /// </summary>
    /// <param name="playerName">El nombre ya sin espacios al principio ni al final.</param>
    /// <returns>True si el nombre vale, false si no.</returns>
    public bool IsValidName(string playerName)
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

    /// <summary>
    /// Saca un solo numero para poder ordenar el ranking: cuanto mas alto, mejor.
    /// Cada nivel vale 100000 puntos y cada segundo resta 1, asi manda el nivel y el tiempo solo desempata.
    /// Ganar cuenta como un nivel mas, para quedar por encima de quien murio contra el jefe.
    /// Se guarda ya calculado para que Firestore ordene por un solo campo.
    /// </summary>
    /// <param name="level">Nivel al que ha llegado (1 a 10).</param>
    /// <param name="time">Lo que ha durado la partida, en segundos.</param>
    /// <param name="won">True si ha ganado al jefe final.</param>
    /// <returns>La puntuacion de la partida.</returns>
    public int CalculateScore(int level, int time, bool won)
    {
        int score = level * 100000 - time;

        if (won)
        {
            score += 100000;
        }

        return score;
    }

    /// <summary>
    /// Pide a Firestore las 10 mejores partidas y las escribe en las tres columnas (nombre, nivel y tiempo).
    /// Si no hay conexion lo dice en vez de quedarse cargando.
    /// </summary>
    private void LoadRanking()
    {
        namesText.text = "Cargando...";
        levelsText.text = "";
        timesText.text = "";

        // Firestore ya devuelve las partidas ordenadas de mayor a menor puntuacion, y solo las 10 primeras
        db.Collection("scores").OrderByDescending("score").Limit(rows).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (this == null)
            {
                return;
            }

            if (task.IsFaulted)
            {
                namesText.text = "Sin conexion";
                return;
            }

            namesText.text = "";
            int position = 1;

            // Cada documento es una partida
            foreach (DocumentSnapshot document in task.Result.Documents)
            {
                string playerName = document.GetValue<string>("name");
                int level = document.GetValue<int>("level");
                int time = document.GetValue<int>("time");
                bool won = document.GetValue<bool>("won");

                int minutes = time / 60;
                int seconds = time % 60;

                namesText.text += position + ". " + playerName + "\n";
                timesText.text += minutes.ToString("00") + ":" + seconds.ToString("00") + "\n";

                if (won)
                {
                    levelsText.text += "Victoria\n";
                }
                else
                {
                    levelsText.text += "Nivel " + level + "\n";
                }

                position++;
            }

            if (position == 1)
            {
                namesText.text = "Todavia no hay partidas";
            }
        });
    }
}
