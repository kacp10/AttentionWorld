using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentity;
using XCharts.Runtime;

public class PieChartPerformance : MonoBehaviour
{
    public PieChart pieChart;

    private AmazonDynamoDBClient dbClient;
    private string playerId;
    private int year, month;

    private Dictionary<string, int> totalScores = new();
    private Dictionary<string, int> countScores = new();

    async void Start()
    {
        playerId = UserSession.Instance != null
          ? UserSession.Instance.GetCurrentPlayerId()
          : "unknown";
        var creds = new CognitoAWSCredentials(
            "",
            RegionEndpoint.USEast1);
        dbClient = new AmazonDynamoDBClient(creds, RegionEndpoint.USEast1);

        var today = DateTime.Now;
        year = today.Year;
        month = today.Month;

        await LoadDataAsync();
        DrawPieChart();
    }

    async Task LoadDataAsync()
    {
        string prefix = $"{year:0000}-{month:00}";

        var request = new QueryRequest
        {
            TableName = "GameResults",
            KeyConditionExpression = "PlayerID = :pid",
            FilterExpression = "begins_with(PlayDate, :pfx)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {":pid", new AttributeValue { S = playerId }},
                {":pfx", new AttributeValue { S = prefix }}
            }
        };

        try
        {
            var response = await dbClient.QueryAsync(request);
            foreach (var item in response.Items)
            {
                if (!item.ContainsKey("CognitiveArea") || !item.ContainsKey("Score")) continue;
                string area = item["CognitiveArea"].S.ToLower();
                if (!int.TryParse(item["Score"].N, out int score)) continue;

                if (!totalScores.ContainsKey(area))
                {
                    totalScores[area] = 0;
                    countScores[area] = 0;
                }

                totalScores[area] += score;
                countScores[area]++;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al consultar datos: {ex.Message}");
        }
    }

    void DrawPieChart()
    {
        pieChart.ClearData();

        var areas = new[] { "logica", "memoria", "atencion", "calculo" };
        var labels = new[] { "Lógica", "Memoria", "Atención", "Cálculo" };

        for (int i = 0; i < areas.Length; i++)
        {
            string key = areas[i];
            float avg = (countScores.ContainsKey(key) && countScores[key] > 0)
                        ? (float)totalScores[key] / countScores[key]
                        : 0f;

            pieChart.AddData(0, avg, labels[i]);
        }

        pieChart.RefreshChart();
    }
}
