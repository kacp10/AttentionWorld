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

public class PerformanceChart : MonoBehaviour
{
    [Header("UI References")]
    public LineChart lineChart; // Asigna tu LineChart en el Inspector

    private AmazonDynamoDBClient dbClient;
    private string playerId;
    private int year, month;

    // Estructura para promediar scores
    struct AreaScores
    {
        public double Attention;
        public double Memory;
        public double Logic;
        public double Calculation;
    }

    async void Start()
    {
        // 1) Inicializar AWS y fecha
        playerId = UserSession.Instance?.GetLoggedInUser() ?? "unknown";
        var creds = new CognitoAWSCredentials(
            "",
            RegionEndpoint.USEast1);
        dbClient = new AmazonDynamoDBClient(creds, RegionEndpoint.USEast1);

        var today = DateTime.Now;
        year = today.Year;
        month = today.Month;

        // 2) Obtener datos y calcular promedios
        var data = await GetPerformanceDataAsync();

        // 3) Configurar gráfico
        SetupChartAppearance();

        // 4) Dibujar series
        PlotSeries(data);
    }

    async Task<Dictionary<int, AreaScores>> GetPerformanceDataAsync()
    {
        var temp = new Dictionary<int, Dictionary<string, List<int>>>();
        var prefix = $"{year:0000}-{month:00}";

        var request = new QueryRequest
        {
            TableName = "GameResults",
            KeyConditionExpression = "PlayerID = :pid",
            FilterExpression = "begins_with(PlayDate, :pfx)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {":pid", new AttributeValue { S = playerId }},
                {":pfx", new AttributeValue { S = prefix    }}
            }
        };

        QueryResponse resp;
        try { resp = await dbClient.QueryAsync(request); }
        catch (Exception e)
        {
            Debug.LogError("DynamoDB Query failed: " + e.Message);
            return Enumerable.Range(1, 7).ToDictionary(d => d, _ => new AreaScores());
        }

        foreach (var item in resp.Items)
        {
            if (!item.TryGetValue("PlayDate", out var pd) ||
                !DateTime.TryParse(pd.S, out var dt)) continue;
            int day = dt.Day;
            if (day < 1 || day > 7) continue;
            if (!temp.ContainsKey(day)) temp[day] = new Dictionary<string, List<int>>();
            if (item.TryGetValue("CognitiveArea", out var av) &&
                item.TryGetValue("Score", out var sv) &&
                int.TryParse(sv.N, out var sc))
            {
                var area = av.S.ToLower();
                if (!temp[day].ContainsKey(area)) temp[day][area] = new List<int>();
                temp[day][area].Add(sc);
            }
        }

        var result = new Dictionary<int, AreaScores>();
        for (int d = 1; d <= 7; d++)
        {
            var a = new AreaScores();
            if (temp.TryGetValue(d, out var map))
            {
                if (map.TryGetValue("atencion", out var la) && la.Any()) a.Attention = la.Average();
                if (map.TryGetValue("memoria", out var lm) && lm.Any()) a.Memory = lm.Average();
                if (map.TryGetValue("logica", out var ll) && ll.Any()) a.Logic = ll.Average();
                if (map.TryGetValue("calculo", out var lc) && lc.Any()) a.Calculation = lc.Average();
            }
            result[d] = a;
        }
        return result;
    }

    void SetupChartAppearance()
    {
        // Título
        var titleComp = lineChart.GetChartComponent<Title>();
        titleComp.text = $"MAYO {year}";
        titleComp.show = true;

        // Eje X
        var xAxis = lineChart.GetChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Category;
        xAxis.data.Clear();
        for (int i = 1; i <= 7; i++) xAxis.data.Add(i.ToString());
        xAxis.splitLine.show = false;

        // Eje Y
        var yAxis = lineChart.GetChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;
        yAxis.minMaxType = Axis.AxisMinMaxType.Default;
        yAxis.splitNumber = 5;

        // Limpiar series previas
        lineChart.ClearData();
    }

    void PlotSeries(Dictionary<int, AreaScores> data)
    {
        var listA = new List<double>();
        var listM = new List<double>();
        var listL = new List<double>();
        var listC = new List<double>();
        for (int d = 1; d <= 7; d++)
        {
            var s = data[d];
            listA.Add(s.Attention);
            listM.Add(s.Memory);
            listL.Add(s.Logic);
            listC.Add(s.Calculation);
        }

        AddLineSeries("Atención", Color.yellow, listA);
        AddLineSeries("Memoria", Color.cyan, listM);
        AddLineSeries("Lógica", Color.green, listL);
        AddLineSeries("Cálculo", Color.red, listC);

        lineChart.RefreshChart();
    }

    void AddLineSeries(string name, Color col, List<double> values)
    {
        var serie = lineChart.AddSerie<Line>(name);
        foreach (var v in values)
            serie.AddData(v);

        serie.lineStyle.color = col;
        serie.symbol.show = true;
        serie.symbol.type = SymbolType.Circle;
        serie.symbol.size = 10;
        serie.itemStyle.color = col;
    }
}
