// ProfileManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class ProfileManager : MonoBehaviour
{
    // ─────────  Inspector  ─────────
    [SerializeField] TMP_Text usernameText;
    [SerializeField] Image[] medalImages;      // 4 slots
    [SerializeField] TMP_Text[] lastGameTexts;  // 4 slots
    [SerializeField] Button backButton;

    // ─────────  AWS  ─────────
    AmazonDynamoDBClient db;
    string playerId;

    // Bucket y claves de las medallas
    const string Bucket = "attention-world-assets";
    readonly string[] medalKeys = {
        "achievements/achievement1.png",
        "achievements/achievement2.png",
        "achievements/achievement3.png",
        "achievements/achievement4.png"
    };
    const int MedalThreshold = 1000;   // Score mínimo

    // ===================  START  ===================
    async void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(GoBackHomeChildScene);
        else
            Debug.LogWarning("BackButton no asignado en inspector.");

        playerId = UserSession.Instance?.GetLoggedInUser() ?? "unknown";
        usernameText.text = playerId;

        EnsureDynamo();
        var results = await QueryResults();
        if (results == null) return;

        await LoadMedals(results);
        LoadActivities(results);
    }

    // =================  MEDALLAS  ==================
    async Task LoadMedals(List<GameResult> results)
    {
        int gamesWith1000 = results
            .Where(r => r.Score >= MedalThreshold)
            .Select(r => r.GameName)
            .Distinct()
            .Count();

        for (int i = 0; i < medalImages.Length; i++)
        {
            bool earned = i < gamesWith1000;
            Image img = medalImages[i];

            if (earned)
            {
                await S3Manager.Instance.LoadImage(Bucket, medalKeys[i], img);
                img.rectTransform.sizeDelta = new Vector2(84, 75);
            }
            else
            {
                img.sprite = null;
                img.color = new Color(1, 1, 1, 0); // transparente
            }
        }
    }

    // ===========  ÚLTIMAS ACTIVIDADES  =============
    void LoadActivities(List<GameResult> results)
    {
        var list = results
            .OrderByDescending(r => r.Date)
            .GroupBy(r => r.GameName)      // sin duplicar
            .Select(g => g.First())
            .Take(4)
            .ToArray();

        for (int i = 0; i < lastGameTexts.Length; i++)
            lastGameTexts[i].text = i < list.Length ? list[i].GameName : "-";
    }
    void GoBackHomeChildScene()
    {
        SceneManager.LoadScene("HomeChildScene");
    }

    // ================  DYNAMODB =====================
    async Task<List<GameResult>> QueryResults()
    {
        try
        {
            var req = new QueryRequest
            {
                TableName = "GameResults",
                KeyConditionExpression = "PlayerID = :pid",
                ExpressionAttributeValues = new() {
                    {":pid", new AttributeValue { S = playerId }}
                }
            };

            var resp = await db.QueryAsync(req);
            var list = new List<GameResult>();

            foreach (var item in resp.Items)
            {
                if (item.TryGetValue("Score", out var sVal) &&
                     item.TryGetValue("GameName", out var gVal) &&
                     item.TryGetValue("PlayDate", out var dVal) &&
                     int.TryParse(sVal.N, out int score) &&
                     System.DateTime.TryParse(dVal.S, out var date))
                {
                    list.Add(new GameResult { GameName = gVal.S, Score = score, Date = date });
                }
            }
            return list;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ DynamoDB: {ex.Message}");
            return null;
        }
    }

    // ===============  AWS CLIENTS  =====================
    void EnsureDynamo()
    {
        if (db != null) return;

        var creds = new CognitoAWSCredentials(
            "",   // mismo Identity Pool
            RegionEndpoint.USEast1);

        db = new AmazonDynamoDBClient(creds, RegionEndpoint.USEast1);
    }

    // ================  POCO ============================
    private class GameResult
    {
        public string GameName;
        public int Score;
        public System.DateTime Date;
    }
}
