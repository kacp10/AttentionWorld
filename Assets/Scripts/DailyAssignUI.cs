using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentity;
using System;
using System.Globalization;
using System.Threading.Tasks;

public class DailyAssignUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text dateLabel;
    [SerializeField] private Transform listContainer;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button backBtn;
    [SerializeField] private List<Sprite> iconSprites;  // Asegúrate de que estén en orden: Atención, Memoria, Math, Lógica

    private List<string> todayGames = new();

    private AmazonDynamoDBClient db;

    async void Start()
    {
        // Fecha bonita
        string today = DateTime.Now.ToString("ddd. d MMMM", new CultureInfo("es-ES"));
        dateLabel.text = today;

        // DynamoDB
        var credentials = new CognitoAWSCredentials("", Amazon.RegionEndpoint.USEast1);
        db = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast1);

        await LoadAssignmentsFromDB();

        // Crear ítems visuales
        for (int i = 0; i < todayGames.Count; i++)
        {
            GameObject line = Instantiate(linePrefab, listContainer);
            Debug.Log($"🧩 Instanciado: {todayGames[i]}");

            Transform gameText = line.transform.Find("GameText");
            Transform icon = line.transform.Find("Icon");

            if (gameText == null || icon == null)
            {
                Debug.LogError("❌ GameText o Icon no encontrados en el prefab.");
                continue;
            }
            gameText.GetComponent<TMP_Text>().text = GetDisplayName(todayGames[i]);
            icon.GetComponent<Image>().sprite = GetIconForScene(todayGames[i]);
        }

        // BOTÓN INICIAR ► lanza BriefScene
        startBtn.onClick.AddListener(() =>
        {
            var assignments = new List<GameAssignment>();

            foreach (var scene in todayGames)
            {
                assignments.Add(new GameAssignment
                {
                    SceneName = scene,
                    DisplayName = GetDisplayName(scene),
                    Area = GetAreaForScene(scene),
                    Instruction = GetInstructionForScene(scene)
                });
            }

            GameSessionData.Instance.SetAssignments(assignments);
            PlayerPrefs.SetInt("CurrentGameIndex", 0);

            UnityEngine.SceneManagement.SceneManager.LoadScene("BriefScene");
        });

        // BOTÓN ATRÁS
        backBtn.onClick.AddListener(() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene"));
    }

    async Task LoadAssignmentsFromDB()
    {
        string playerId = UserSession.Instance?.GetLoggedInUser();
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogWarning("⚠️ No hay sesión iniciada.");
            return;
        }

        string today = DateTime.Now.ToString("yyyy-MM-dd");
        Debug.Log($"📅 Hoy es {today} - Jugador: {playerId}");

        var request = new QueryRequest
        {
            TableName = "DailyAssignments",
            IndexName = "PlayerID-Date-index",
            KeyConditionExpression = "PlayerID = :pid and #d = :today",
            ExpressionAttributeNames = new Dictionary<string, string> { { "#d", "Date" } },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":pid", new AttributeValue { S = playerId } },
                { ":today", new AttributeValue { S = today } }
            }
        };

        try
        {
            var response = await db.QueryAsync(request);
            Debug.Log($"📦 Resultado: {response.Count} ítems.");

            if (response.Items.Count > 0 && response.Items[0].ContainsKey("Games"))
            {
                todayGames = response.Items[0]["Games"].SS;
                Debug.Log("✔ Juegos cargados: " + string.Join(", ", todayGames));
            }
            else
            {
                Debug.LogWarning("❌ No se encontraron juegos asignados para hoy.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Error al consultar DynamoDB: " + ex.Message);
        }
    }

    private string GetDisplayName(string sceneName)
    {
        return sceneName switch
        {
            "BallScene" => "Pelotas saltarinas",
            "PairsScene" => "Parejas",
            "GameSceneMath" => "Sumas",
            "PuzzleScene" => "Rompecabezas",
            _ => sceneName
        };
    }

    private Sprite GetIconForScene(string sceneName)
    {
        return sceneName switch
        {
            "BallScene" => iconSprites[0],
            "PairsScene" => iconSprites[1],
            "GameSceneMath" => iconSprites[2],
            "PuzzleScene" => iconSprites[3],
            _ => null
        };
    }

    private string GetAreaForScene(string sceneName)
    {
        return sceneName switch
        {
            "BallScene" => "Atención",
            "PairsScene" => "Memoria",
            "GameSceneMath" => "Cálculo",
            "PuzzleScene" => "Lógica",
            _ => "General"
        };
    }

    private string GetInstructionForScene(string sceneName)
    {
        return sceneName switch
        {
            "BallScene" => "Elige cuál pelota salta más alto observando sus movimientos.",
            "PairsScene" => "Encuentra las parejas de imágenes volteando dos cartas a la vez.",
            "GameSceneMath" => "Resuelve las operaciones matemáticas antes de que acabe el tiempo.",
            "PuzzleScene" => "Ordena correctamente las piezas para completar la figura.",
            _ => "Sigue las instrucciones del juego con atención."
        };
    }
}
