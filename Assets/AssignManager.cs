using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;

/// Control principal de la escena “Asignar Ejercicios”
public class AssignManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] TMP_Dropdown classroomDropdown;      // Salón A / B
    [SerializeField] Transform studentsContent;           // Content del ScrollView de alumnos
    [SerializeField] GameObject studentRowPrefab;          // Prefab con TMP_Text y Button
    [SerializeField] Button assignButton;
    [SerializeField] Button backButton;
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] AssignUI ui;                          // Referencia a la nueva UI de juegos

    /* AWS */
    AmazonDynamoDBClient db;
    string teacherId;
    string selectedClassroom;

    /*──────── ARRANQUE ────────*/
    async void Start()
    {
        // 0. Verificar sesión
        if (UserSession.Instance == null || string.IsNullOrEmpty(UserSession.Instance.GetLoggedInUser()))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
            return;
        }
        teacherId = UserSession.Instance.GetLoggedInUser();

        // 1. Configurar DynamoDB
        var cred = new Amazon.CognitoIdentity.CognitoAWSCredentials(
            "",
            Amazon.RegionEndpoint.USEast1);
        db = new AmazonDynamoDBClient(cred, Amazon.RegionEndpoint.USEast1);

        // 2. Cargar dropdown de salones
        classroomDropdown.ClearOptions();
        classroomDropdown.AddOptions(new List<string> { "Salon A", "Salon B" });
        selectedClassroom = classroomDropdown.options[0].text;
        classroomDropdown.onValueChanged.AddListener(delegate { _ = RefreshStudentList(); });  // ✔️ corrección aquí

        // 3. Botones
        assignButton.onClick.AddListener(SaveAssignments);
        backButton.onClick.AddListener(() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene("HomeTeacherScene"));

        // 4. Primera carga
        await RefreshStudentList();
    }

    /*──────── LISTA DE ALUMNOS ─────*/
    async Task RefreshStudentList()
    {
        selectedClassroom = classroomDropdown.options[classroomDropdown.value].text;

        foreach (Transform c in studentsContent) Destroy(c.gameObject);

        var req = new ScanRequest
        {
            TableName = "PlayerData",
            FilterExpression = "#r = :child AND Classroom = :cls",
            ExpressionAttributeNames = new() { { "#r", "Role" } },
            ExpressionAttributeValues = new()
            {
                { ":child", new AttributeValue { S = "Child" } },
                { ":cls",   new AttributeValue { S = selectedClassroom } }
            }
        };

        var resp = await db.ScanAsync(req);
        Debug.Log($"▶ Scan DynamoDB salón {selectedClassroom} → {resp.Count} alumno(s)");

        foreach (var it in resp.Items)
        {
            if (!it.ContainsKey("Name")) continue;

            var row = Instantiate(studentRowPrefab, studentsContent);
            row.transform.GetComponentInChildren<TMP_Text>().text = it["Name"].S;

            var meta = row.GetComponent<StudentRowMeta>();
            if (meta == null) meta = row.gameObject.AddComponent<StudentRowMeta>();   // ← seguro
            meta.playerId = it["PlayerID"].S;

            // Al hacer clic en el alumno, lo activamos en AssignUI
            row.GetComponentInChildren<Button>().onClick.AddListener(
    () => ui.SetCurrentStudent(meta));

        }
    }

    /*──────── GUARDAR ─────*/
    // Método que guarda las asignaciones de juegos
    async void SaveAssignments()
    {
        var playerId = ui.CurrentStudentId;
        var games = ui.GetSelectedGames();

        if (string.IsNullOrEmpty(playerId))
        {
            feedbackText.text = "Selecciona un alumno primero.";
            return;
        }
        if (games.Count == 0)
        {
            feedbackText.text = "No hay juegos seleccionados.";
            return;
        }

        // Debug para verificar los juegos seleccionados
        Debug.Log($"✔ Juegos seleccionados para asignar: {string.Join(", ", games)}");

        // Crear el item para DynamoDB
        var item = new Dictionary<string, AttributeValue>
    {
        { "PlayerID",  new AttributeValue { S = playerId } },
        { "Date",      new AttributeValue { S = System.DateTime.UtcNow.ToString("yyyy-MM-dd") } },
        { "TeacherID", new AttributeValue { S = teacherId } },
        { "Classroom", new AttributeValue { S = selectedClassroom } },
        { "Games",     new AttributeValue { SS = games } }
    };

        try
        {
            // ⏱️ Inicia medición
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            await db.PutItemAsync(new PutItemRequest
            {
                TableName = "DailyAssignments",
                Item = item
            });

            stopwatch.Stop();
            float tiempoPut = stopwatch.ElapsedMilliseconds / 1000f;

            // 🔍 Registro
            Debug.Log($"[R3] Tiempo de respuesta PUT DailyAssignments: {tiempoPut:F2} segundos");
            System.IO.File.AppendAllText("AssignTestResults.txt", $"{System.DateTime.Now} - {playerId} - {tiempoPut:F2}s\n");

            feedbackText.text = "¡Asignaciones guardadas!";
            Debug.Log($"✔ Asignación guardada para {playerId} con juegos: {string.Join(", ", games)}");
        }

        catch (System.Exception ex)
        {
            Debug.LogError($"❌ ERROR al guardar {playerId}: {ex.Message}");
            feedbackText.text = "Error al guardar. Revisa consola.";
        }
    }

}
