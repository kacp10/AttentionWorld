using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentity;
using System.Collections.Generic;   // ←  Dictionary<,>


public class ParentHomeController : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] Button progressButtonParents;

    AmazonDynamoDBClient db;

    /* -------------------------------------------------- */
    void Start()
    {
        if (progressButtonParents == null)
        {
            Debug.LogError("[ParentHome] Arrastra el botón PROGRESO HIJO al Inspector");
            return;
        }

        db = new AmazonDynamoDBClient(
                new CognitoAWSCredentials(
                    "",
                    RegionEndpoint.USEast1),
                RegionEndpoint.USEast1);

        progressButtonParents.onClick.AddListener(() => _ = GoToProgressAsync());
        Debug.Log("[ParentHome] Start() listo");
    }

    async Task GoToProgressAsync()
    {
        Debug.Log("[ParentHome] Click en PROGRESO HIJO");

        string parentId = UserSession.Instance.GetLoggedInUser();
        if (string.IsNullOrEmpty(parentId))
        {
            SceneManager.LoadScene("LoginScene");
            return;
        }

        string childId =
              await FindChildByParentIdAsync(parentId)      // A
           ?? await FindChildByExactNameAsync(parentId)     // B
           ?? await FindChildByLooseNameAsync(parentId);    // C

        if (!string.IsNullOrEmpty(childId))
        {
            Debug.Log($"[ParentHome] ➜ Hijo encontrado ({childId})");
            UserSession.Instance.SetViewedChild(childId);
        }
        else
        {
            Debug.LogWarning("[ParentHome] Padre sin hijo asociado, se mostrará progreso vacío");
            UserSession.Instance.ClearViewedChild();
        }

        Debug.Log("[ParentHome] Cargando ProgressScene…");
        SceneManager.LoadScene("ProgressScene");
    }

    /* ----------  PASO  A  ------------------------------------------------ */
    async Task<string> FindChildByParentIdAsync(string parentId)
    {
        var scan = new ScanRequest
        {
            TableName = "PlayerData",
            FilterExpression = "#r = :child AND ParentID = :pid",
            ExpressionAttributeNames = new() { { "#r", "Role" } },
            ExpressionAttributeValues = new()
            {
                {":child", new AttributeValue{ S = "Child" }},
                {":pid",   new AttributeValue{ S = parentId }}
            },
            Limit = 1
        };

        var resp = await db.ScanAsync(scan);
        return resp.Items.Count > 0 ? resp.Items[0]["PlayerID"].S : null;
    }

    /* ----------  PASO  B  ------------------------------------------------ */
    async Task<string> FindChildByExactNameAsync(string parentId)
    {
        /* Obtiene el nombre completo almacenado en ParentID */
        var parent = await db.GetItemAsync("PlayerData",
                      new Dictionary<string, AttributeValue>{
                          {"PlayerID", new AttributeValue{ S = parentId }}
                      });

        if (!parent.Item.TryGetValue("ParentID", out var childNameAttr) ||
            string.IsNullOrWhiteSpace(childNameAttr.S))
            return null;

        string childName = childNameAttr.S.Trim();

        var scan = new ScanRequest
        {
            TableName = "PlayerData",
            FilterExpression = "#r = :child AND #n = :cname",
            ExpressionAttributeNames = new()
            {
                {"#r", "Role"},
                {"#n", "Name"}
            },
            ExpressionAttributeValues = new()
            {
                {":child", new AttributeValue{ S = "Child" }},
                {":cname", new AttributeValue{ S = childName }}
            },
            Limit = 1
        };

        var resp = await db.ScanAsync(scan);
        return resp.Items.Count > 0 ? resp.Items[0]["PlayerID"].S : null;
    }

    /* ----------  PASO  C  ------------------------------------------------ */
    async Task<string> FindChildByLooseNameAsync(string parentId)
    {
        /* Nombre que escribió el padre */
        var parent = await db.GetItemAsync("PlayerData",
                      new Dictionary<string, AttributeValue>{
                          {"PlayerID", new AttributeValue{ S = parentId }}
                      });
        if (!parent.Item.TryGetValue("ParentID", out var childNameAttr) ||
            string.IsNullOrWhiteSpace(childNameAttr.S))
            return null;

        string wanted = Normalize(childNameAttr.S);

        /* Trae todos los niños y compara ignorando mayúsculas, tildes, etc. */
        var scan = new ScanRequest
        {
            TableName = "PlayerData",
            FilterExpression = "#r = :child",
            ExpressionAttributeNames = new() { { "#r", "Role" } },
            ExpressionAttributeValues = new()
            {
                {":child", new AttributeValue{ S = "Child" }}
            }
        };

        var resp = await db.ScanAsync(scan);
        foreach (var item in resp.Items)
        {
            if (!item.ContainsKey("Name")) continue;
            if (Normalize(item["Name"].S) == wanted)
                return item["PlayerID"].S;
        }
        return null;
    }

    /* Quita tildes y pasa todo a minúsculas */
    static string Normalize(string s)
    {
        string formD = s.ToLowerInvariant()
                       .Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (char c in formD)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        return sb.ToString().Replace(" ", ""); // sin espacios
    }
}
