using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.CognitoIdentity;

public class SummarySceneManager : MonoBehaviour
{
    /*─────────────────────── UI ───────────────────────*/
    [Header("UI")]
    public TMP_Text titleText, totalScoreText, feedbackText;
    public Image brainImage;

    [Header("Brain Sprites")]
    public Sprite emptyBrain, quarterBrain, halfBrain, threeQuarterBrain, fullBrain;

    /*──────────────── AWS ────────────────*/
    private AmazonDynamoDBClient db;

    /*──────────────────────── Start ───────────────────────*/
    void Start()
    {
        /* 0. Datos de sesión */
        string playerId = UserSession.Instance != null
                        ? UserSession.Instance.GetLoggedInUser()
                        : "unknown";

        string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        int juegos = GameSessionData.Instance.Assignments.Count;

        /* 1. Recorremos las partidas guardadas y subimos cada una */
        int totalScore = 0;

        for (int i = 0; i < juegos; i++)
        {
            int score = PlayerPrefs.GetInt($"Score{i}", 0);
            string area = PlayerPrefs.GetString($"CognitiveArea{i}", "atencion");
            string game = PlayerPrefs.GetString($"GameName{i}", $"Game{i}");
            bool hasCI = PlayerPrefs.GetInt($"HasCI{i}", 0) == 1;
            int correct = PlayerPrefs.GetInt($"Correct{i}", 0);
            int incor = PlayerPrefs.GetInt($"Incorrect{i}", 0);

            totalScore += score;

            /* subimos CADA partida (idx as part of GameStamp) */
            StartCoroutine(
                PutSingleGame(
                    playerId, today, game, area, score,
                    hasCI, correct, incor, i       /* idx */
                )
            );
        }

        /* 2. UI local */
        int promedio = juegos > 0 ? totalScore / juegos : 0;
        totalScoreText.text = promedio.ToString();
        UpdateBrain(promedio);
        UpdateTexts(promedio);

        /* 3. Resumen diario */
        StartCoroutine(PutDailySummary(playerId, today, promedio, juegos));
    }

    /*──────────────── Dynamo helpers ───────────────────*/
    IEnumerator PutSingleGame(string pid, string date, string game, string area,
                              int score, bool hasCI, int corr, int inc, int idx)
    {
        EnsureClient();
        var table = Table.LoadTable(db, "GameResults");

        var doc = new Document
        {
            ["PlayerID"] = pid,                     // PK
            ["GameStamp"] = $"{date}#{idx:D2}",      // SK único por partida
            ["PlayDate"] = date,                    // atributo auxiliar (string)
            ["GameName"] = game,
            ["CognitiveArea"] = area,
            ["Score"] = score,
            ["ItemType"] = "SingleGame"
        };

        if (hasCI)
        {
            doc["CorrectCount"] = corr;
            doc["IncorrectCount"] = inc;
        }

        yield return table.PutItemAsync(doc);
        Debug.Log($"⏫ Subido juego {idx + 1}: {game} ({score} pts)");
    }

    IEnumerator PutDailySummary(string pid, string date, int total, int games)
    {
        EnsureClient();
        var table = Table.LoadTable(db, "GameResults");

        var doc = new Document
        {
            ["PlayerID"] = pid,                  // PK
            ["GameStamp"] = $"{date}#SUMMARY",    // SK resumen
            ["PlayDate"] = date,
            ["TotalScore"] = total,
            ["CompletedGames"] = games,
            ["ItemType"] = "DailySummary"
        };

        yield return table.PutItemAsync(doc);
        Debug.Log("📦 Resumen diario subido.");
    }

    void EnsureClient()
    {
        if (db != null) return;

        var cred = new CognitoAWSCredentials(
            "",
            RegionEndpoint.USEast1);

        db = new AmazonDynamoDBClient(cred, RegionEndpoint.USEast1);
    }

    /*────────────────── UI helpers ─────────────────────*/
    void UpdateBrain(int s)
    {
        brainImage.sprite = s >= 900 ? fullBrain
                        : s >= 700 ? threeQuarterBrain
                        : s >= 500 ? halfBrain
                        : s >= 300 ? quarterBrain
                                   : emptyBrain;
    }

    void UpdateTexts(int s)
    {
        titleText.text = s >= 900 ? "¡Día Excelente!"
                        : s >= 700 ? "¡Muy buen trabajo!"
                        : s >= 500 ? "¡Buen esfuerzo!"
                                   : "¡Sigue practicando!";

        feedbackText.text = s >= 900 ? "Tu desempeño fue sobresaliente."
                         : s >= 700 ? "¡Gran progreso, bien hecho!"
                         : s >= 500 ? "¡Vas mejorando! Mañana más."
                                    : "Hoy fue duro, mañana será mejor.";
    }
}
