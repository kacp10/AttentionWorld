using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentity;
public class TeacherStudentManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public Button ButtonList;       // Prefab de botón ButtonList
    public Transform Transform;     // Contenedor donde instanciar los botones

    private AmazonDynamoDBClient dbClient;
    private string teacherId;
    private string teacherClassroom;

    async void Start()
    {
        if (ButtonList == null || Transform == null)
        {
            Debug.LogError("[TeacherList] Asigna ButtonList y Transform en el Inspector.");
            return;
        }

        teacherId = UserSession.Instance?.GetLoggedInUser();
        if (string.IsNullOrEmpty(teacherId))
        {
            Debug.LogError("[TeacherList] No hay profesor logueado.");
            return;
        }

        EnsureClient();

        // 1) Obtener el salón del profesor
        teacherClassroom = await FetchTeacherClassroomAsync(teacherId);
        if (string.IsNullOrEmpty(teacherClassroom))
        {
            Debug.LogError("[TeacherList] No se encontró Classroom para el profesor.");
            return;
        }

        // 2) Obtener la lista de estudiantes en ese salón
        var students = await FetchStudentsByClassroomAsync(teacherClassroom);

        // 3) Mostrar la lista
        PopulateStudentButtons(students);
    }

    /// <summary>
    /// Instancia un cliente de DynamoDB con AWS Cognito.
    /// </summary>
    void EnsureClient()
    {
        if (dbClient != null) return;
        var creds = new CognitoAWSCredentials(
            "",
            RegionEndpoint.USEast1);
        dbClient = new AmazonDynamoDBClient(creds, RegionEndpoint.USEast1);
    }

    /// <summary>
    /// Consulta DynamoDB para obtener el Classroom del profesor.
    /// </summary>
    async Task<string> FetchTeacherClassroomAsync(string pid)
    {
        var request = new GetItemRequest
        {
            TableName = "PlayerData",
            Key = new Dictionary<string, AttributeValue>
            {
                { "PlayerID", new AttributeValue { S = pid } }
            },
            ProjectionExpression = "Classroom"
        };
        try
        {
            var resp = await dbClient.GetItemAsync(request);
            return resp.Item.ContainsKey("Classroom") ? resp.Item["Classroom"].S : null;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[TeacherList] Error obteniendo Classroom: " + e.Message);
            return null;
        }
    }

    /// <summary>
    /// Recupera todos los estudiantes (Role=Child) cuyo Classroom coincide.
    /// </summary>
    async Task<List<(string id, string name)>> FetchStudentsByClassroomAsync(string classroom)
    {
        var students = new List<(string, string)>();
        var scan = new ScanRequest
        {
            TableName = "PlayerData",
            FilterExpression = "#r = :roleChild AND Classroom = :cls",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#r", "Role" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":roleChild", new AttributeValue { S = "Child" } },
                { ":cls",       new AttributeValue { S = classroom  } }
            }
        };
        try
        {
            var resp = await dbClient.ScanAsync(scan);
            foreach (var item in resp.Items)
            {
                string id = item.ContainsKey("PlayerID") ? item["PlayerID"].S : null;
                string name = item.ContainsKey("Name") ? item["Name"].S : id;
                if (!string.IsNullOrEmpty(id))
                    students.Add((id, name));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("[TeacherList] Error recuperando estudiantes: " + e.Message);
        }
        return students;
    }

    /// <summary>
    /// Crea un botón por cada estudiante y asigna su acción.
    /// </summary>
    void PopulateStudentButtons(List<(string id, string name)> students)
    {
        foreach (var (id, name) in students)
        {
            var go = Instantiate(ButtonList, Transform);
            var label = go.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = name;

            go.onClick.AddListener(() => {
                // Al hacer click, mostrar progreso del estudiante
                UserSession.Instance.SetViewedChild(id);
                SceneManager.LoadScene("ProgressScene");
            });
        }
    }
}

