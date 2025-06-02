using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentity;
using UnityEngine.SceneManagement;

public class SummaryBubble : MonoBehaviour
{
    [Header("Textos TMP")]
    public TMP_Text txtDaysPlayed;
    public TMP_Text txtExercisesDone;
    public TMP_Text txtTopGame;
    public TMP_Text txtBestMemory;
    public TMP_Text txtBestAttention;
    public TMP_Text txtBestLogic;
    public TMP_Text txtBestCalc;
    [Header("Botón Back")]
    public Button backButton;


    /* AWS */
    AmazonDynamoDBClient db;
    string playerId;

    async void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
        else
            Debug.LogWarning("Back Button no asignado en el inspector");

        playerId = UserSession.Instance != null
         ? UserSession.Instance.GetCurrentPlayerId()
         : "unknown";
        db = new AmazonDynamoDBClient(
                new CognitoAWSCredentials(
                    "",
                    RegionEndpoint.USEast1),
                RegionEndpoint.USEast1);

        var records = await LoadPlayerRecords();

        if (records.Count == 0)
        {
            txtDaysPlayed.text = "Sin datos aún";
            txtExercisesDone.text = "";
            return;
        }

        /* 1) Días jugados y ejercicios totales */
        int days = records.Select(r => r.playDate).Distinct().Count();
        int total = records.Count;
        txtDaysPlayed.text = $"Días jugados: {days}";
        txtExercisesDone.text = $"Ejercicios completados: {total}";

        /* 2) Juego con mayor puntaje global */
        var top = records.OrderByDescending(r => r.score).First();
        txtTopGame.text = $"Mejor juego: {top.game} ({top.score} pts)";

        /* 3) Máximo por cada área */
        SetAreaTop(txtBestMemory, records, "memoria", "Memoria");
        SetAreaTop(txtBestAttention, records, "atencion", "Atención");
        SetAreaTop(txtBestLogic, records, "logica", "Lógica");
        SetAreaTop(txtBestCalc, records, "calculo", "Cálculo");
    }
    void OnBackButtonClicked()
    {
        SceneManager.LoadScene("ProgressScene");
    }

    /* ---------- helpers ---------- */

    struct Rec
    {
        public string playDate;   // yyyy-MM-dd
        public string area;       // atencion, memoria…
        public string game;
        public int score;
    }

    async Task<List<Rec>> LoadPlayerRecords()
    {
        var list = new List<Rec>();

        var req = new QueryRequest
        {
            TableName = "GameResults",
            KeyConditionExpression = "PlayerID = :pid",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {":pid", new AttributeValue{ S = playerId }}
            }
        };

        var resp = await db.QueryAsync(req);
        foreach (var it in resp.Items)
        {
            if (!it.ContainsKey("Score") || !it.ContainsKey("GameName") ||
                !it.ContainsKey("CognitiveArea") || !it.ContainsKey("PlayDate"))
                continue;

            list.Add(new Rec
            {
                playDate = it["PlayDate"].S,
                game = it["GameName"].S,
                area = it["CognitiveArea"].S.ToLower(),
                score = int.Parse(it["Score"].N)
            });
        }
        return list;
    }

    void SetAreaTop(TMP_Text txt, List<Rec> recs, string areaKey, string areaLabel)
    {
        var best = recs.Where(r => r.area == areaKey)
                       .OrderByDescending(r => r.score)
                       .FirstOrDefault();
        if (best.score > 0)
            txt.text = $"{areaLabel}: {best.game}  {best.score} pts";
        else
            txt.text = $"{areaLabel}: —";
    }
}
