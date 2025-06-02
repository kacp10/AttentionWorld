using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentity;
using UnityEngine.SceneManagement;

public class CalendarSceneManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text monthYearText;
    public GameObject dayCellPrefab;
    public Transform gridParent;
    public Button backButton;
    private AmazonDynamoDBClient dbClient;
    private string playerId;
    private HashSet<int> playedDays = new HashSet<int>();

    void Start()
    {
        // Verificar referencias UI
        if (monthYearText == null || dayCellPrefab == null || gridParent == null)
        {
            Debug.LogError("[CalendarSceneManager] Falta asignar referencias en el Inspector.");
            return;
        }
        if (backButton == null)
        {
            Debug.LogWarning("[CalendarSceneManager] backButton no asignado en Inspector.");
        }
        else
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        playerId = UserSession.Instance != null
          ? UserSession.Instance.GetCurrentPlayerId()
          : "unknown";
        Debug.Log("🟢 Usuario actual: " + playerId);

        EnsureClient();
        DateTime today = DateTime.Now;
        monthYearText.text = today.ToString("MMMM yyyy").ToUpper();
        Debug.Log("📅 Cargando calendario para: " + today.ToString("yyyy-MM"));

        LoadPlayedDays(today.Year, today.Month);
    }
    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("ProgressScene");
    }

    void LoadPlayedDays(int year, int month)
    {
        Debug.Log($"📥 Iniciando carga de días jugados para {month}/{year}...");
        Task.Run(async () => await FetchPlayedDatesFromDynamo(year, month));
    }

    async Task FetchPlayedDatesFromDynamo(int year, int month)
    {
        playedDays.Clear();
        string prefix = $"{year:D4}-{month:D2}";

        try
        {
            var request = new QueryRequest
            {
                TableName = "GameResults",
                KeyConditionExpression = "PlayerID = :pid",
                FilterExpression = "begins_with(PlayDate, :pfx)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":pid", new AttributeValue { S = playerId }},
                    {":pfx", new AttributeValue { S = prefix }}
                },
                ProjectionExpression = "PlayDate"
            };

            Debug.Log($"🔍 Query PlayerID={playerId} con prefijo '{prefix}'…");
            var response = await dbClient.QueryAsync(request);
            Debug.Log($"📦 Recibidos {response.Count} documentos");

            foreach (var item in response.Items)
            {
                if (item.TryGetValue("PlayDate", out var av) && DateTime.TryParse(av.S, out var dt))
                {
                    if (dt.Month == month && dt.Year == year)
                    {
                        if (playedDays.Add(dt.Day))
                            Debug.Log($"✅ Día añadido: {dt.Day}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Error DynamoDB: " + ex.Message);
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() => GenerateCalendar(year, month));
    }

    void GenerateCalendar(int year, int month)
    {
        // Validar grilla y prefab
        if (gridParent == null || dayCellPrefab == null)
        {
            Debug.LogError("[GenerateCalendar] gridParent o dayCellPrefab es null.");
            return;
        }

        // Limpiar celdas previas
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        int daysInMonth = DateTime.DaysInMonth(year, month);
        int firstDay = ((int)new DateTime(year, month, 1).DayOfWeek + 6) % 7; // lunes = 0

        // Insertar espacios iniciales
        for (int i = 0; i < firstDay; i++)
        {
            GameObject emptyCell = Instantiate(dayCellPrefab, gridParent);
            var txt = emptyCell.GetComponentInChildren<TMP_Text>();
            if (txt != null) txt.text = string.Empty;
        }

        // Generar días
        for (int d = 1; d <= daysInMonth; d++)
        {
            GameObject cell = Instantiate(dayCellPrefab, gridParent);
            var label = cell.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = d.ToString();

            // Buscar componente Image en hijos
            var img = cell.GetComponent<Image>() ?? cell.GetComponentInChildren<Image>();
            if (img == null)
            {
                Debug.LogWarning($"[GenerateCalendar] Image no encontrado para el día {d}.");
                continue;
            }

            if (playedDays.Contains(d))
            {
                img.color = Color.green;
            }
        }

        Debug.Log($"✅ Calendario generado con {playedDays.Count} días jugados marcados.");
    }

    void EnsureClient()
    {
        if (dbClient != null) return;
        Debug.Log("🔐 Inicializando cliente DynamoDB con credenciales de AWS Cognito...");
        var credentials = new CognitoAWSCredentials(
            "",
            RegionEndpoint.USEast1);
        dbClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);
    }
}
