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
    [Header("TextMeshPro fields")]
    public TMP_Text childNameText;    // Siempre = nombre del usuario logeado
    public TMP_Text birthYearText;    // Año (niño)  | Correo (padre)
    public TMP_Text classroomText;    // Aula
    public TMP_Text parentNameText;   // Nombre del padre (niño) | Nombre del hijo (padre)

    [Header("Text Predeterminados")]
    public TMP_Text dateText;         // "Año de nacimiento" o "Correo" según el rol
    public TMP_Text parentText;       // "Padre" o "Nombre del hijo" según el rol


    [Header("Botón Back")]
    public Button backButton;

    AmazonDynamoDBClient dbClient;
    string playerId;

    /*──────────────────────────────*/
    async void Start()
    {
        playerId = UserSession.Instance != null
                 ? UserSession.Instance.GetCurrentPlayerId()
                 : "unknown";

        EnsureClient();

        if (backButton != null)
            backButton.onClick.AddListener(() => SceneManager.LoadScene("SettingsScene"));

        var myData = await GetUserDataAsync(playerId);
        if (myData == null)
        {
            Debug.LogError("❌ No se encontraron datos de " + playerId);
            return;
        }

        string role = UserSession.Instance != null ? UserSession.Instance.GetCurrentRole()
                                                        : "Child";
        string fullName = myData.GetValueOrDefault("Name")?.S ?? "Desconocido";
        string classroom = myData.GetValueOrDefault("Classroom")?.S ?? "—";
        string year = myData.GetValueOrDefault("YearOfBirth")?.S ?? "—";

        /*──────── Siempre mostrar mi propio nombre en childNameText ───────*/
        childNameText.text = fullName;
        classroomText.text = classroom;

        /*──────── Rol CHILD ──────────────*/
        if (role == "Child")
        {
            birthYearText.text = year;

            var parentData = await GetParentByChildNameAsync(fullName);
            parentNameText.text = parentData != null ? parentData["Name"].S
                                                     : "No registrado";
        }
        /*──────── Rol PARENTS ────────────*/
        else if (role == "Parents")
        {
            dateText.text = "Correo";               // Padre
            parentText.text = "Nombre del hijo";

            /* ParentID contiene el nombre del hijo */
            string childFullName = myData.GetValueOrDefault("ParentID")?.S ?? "—";
            string parentEmail = myData.GetValueOrDefault("Email")?.S ?? "—";

            birthYearText.text = parentEmail;      // mostrar correo del padre
            parentNameText.text = childFullName;    // mostrar nombre del hijo
        }
        /*──────── Otros roles ────────────*/
        else
        {
            birthYearText.text = year;
            parentNameText.text = "—";
        }
    }

    /*──────────── Helpers DynamoDB ────────────*/

    async Task<Dictionary<string, AttributeValue>> GetUserDataAsync(string id)
    {
        var req = new GetItemRequest
        {
            TableName = "PlayerData",
            Key = new Dictionary<string, AttributeValue>{
                {"PlayerID", new AttributeValue{ S = id }}
            }
        };
        var resp = await dbClient.GetItemAsync(req);
        return resp.Item.Count > 0 ? resp.Item : null;
    }

    async Task<Dictionary<string, AttributeValue>> GetParentByChildNameAsync(string childName)
    {
        var scan = new ScanRequest
        {
            TableName = "PlayerData",
            FilterExpression = "#r = :parentRole",
            ExpressionAttributeNames = new() { { "#r", "Role" } },
            ExpressionAttributeValues = new()
            {
                {":parentRole", new AttributeValue{ S = "Parents" }}
            }
        };

        var resp = await dbClient.ScanAsync(scan);
        string target = childName.Trim().ToLowerInvariant();

        foreach (var item in resp.Items)
        {
            if (item.TryGetValue("ParentID", out var pid) &&
                pid.S?.Trim().ToLowerInvariant() == target)
                return item;
        }
        return null;
    }

    void EnsureClient()
    {
        if (dbClient != null) return;
        var creds = new CognitoAWSCredentials(
                        "",
                        RegionEndpoint.USEast1);
        dbClient = new AmazonDynamoDBClient(creds, RegionEndpoint.USEast1);
    }
}
