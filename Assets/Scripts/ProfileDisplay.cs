using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon;
using Amazon.CognitoIdentity;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProfileDisplay : MonoBehaviour
{
    [Header("Referencias UI (TextMeshPro)")]
    public TMP_Text childNameText;
    public TMP_Text birthYearText;
    public TMP_Text classroomText;
    public TMP_Text parentNameText;

    [Header("Botón Back")]
    public Button backButton;

    private AmazonDynamoDBClient dbClient;
    private string playerId;

    async void Start()
    {
        playerId = UserSession.Instance?.GetLoggedInUser() ?? "unknown";
        EnsureClient();

        if (backButton != null)
            backButton.onClick.AddListener(() => SceneManager.LoadScene("SettingsScene"));

        var childData = await GetUserDataAsync(playerId);
        if (childData == null)
        {
            Debug.LogError("❌ No se encontraron datos del niño.");
            return;
        }

        string childName = childData.ContainsKey("Name") ? childData["Name"].S : "Desconocido";
        string classroom = childData.ContainsKey("Classroom") ? childData["Classroom"].S : "—";
        string year = childData.ContainsKey("YearOfBirth") ? childData["YearOfBirth"].S : "—";

        childNameText.text = childName;
        classroomText.text = classroom;
        birthYearText.text = year;

        var parentData = await GetParentByChildNameAsync(childName);
        if (parentData != null && parentData.ContainsKey("Name"))
            parentNameText.text = parentData["Name"].S;
        else
            parentNameText.text = "No registrado";
    }

    async Task<Dictionary<string, AttributeValue>> GetUserDataAsync(string playerId)
    {
        var request = new GetItemRequest
        {
            TableName = "PlayerData",
            Key = new Dictionary<string, AttributeValue> {
                { "PlayerID", new AttributeValue { S = playerId } }
            }
        };

        var response = await dbClient.GetItemAsync(request);
        return response.Item.Count > 0 ? response.Item : null;
    }

    async Task<Dictionary<string, AttributeValue>> GetParentByChildNameAsync(string childName)
    {
        var request = new ScanRequest
        {
            TableName = "PlayerData",
            FilterExpression = "#r = :parentRole",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#r", "Role" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":parentRole", new AttributeValue { S = "Parents" } }
            }
        };

        var response = await dbClient.ScanAsync(request);

        string targetName = childName.Trim().ToLowerInvariant();

        foreach (var item in response.Items)
        {
            if (item.TryGetValue("ParentID", out var pidVal))
            {
                string parentIdValue = pidVal.S?.Trim().ToLowerInvariant();
                if (parentIdValue == targetName)
                {
                    Debug.Log("✅ Padre encontrado: " + item["Name"].S);
                    return item;
                }
            }
        }

        Debug.LogWarning("⚠️ No se encontró padre con ParentID igual a " + childName);
        return null;
    }

    void EnsureClient()
    {
        if (dbClient != null) return;

        var credentials = new CognitoAWSCredentials(
            "",
            RegionEndpoint.USEast1
        );
        dbClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);
    }
}
