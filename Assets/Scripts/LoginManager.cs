using UnityEngine;
using UnityEngine.UI;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System.Diagnostics; // ← necesario para Stopwatch
using System.IO;

/// <summary>
/// Controla el flujo de inicio de sesión y registra métricas de usabilidad (T2) + R1 (rendimiento).
/// </summary>
public class LoginManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField loginField;
    [SerializeField] private TMP_InputField passwordField;

    [Header("Botones")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button childButton;
    [SerializeField] private Button parentsButton;
    [SerializeField] private Button teacherButton;

    [Header("Feedback")]
    [SerializeField] private TMP_Text feedbackText;

    private AmazonDynamoDBClient db;
    private string selectedRole = "";
    private UsabilityTracker tracker;

    private void Start()
    {
        var cred = new BasicAWSCredentials(
            "",
            "");
        db = new AmazonDynamoDBClient(cred, Amazon.RegionEndpoint.USEast1);

        if (UserSession.Instance == null)
        {
            var go = new GameObject("UserSessionManager");
            go.AddComponent<UserSession>();
        }

        tracker = FindObjectOfType<UsabilityTracker>();

        childButton.onClick.AddListener(() => SetRole("Child"));
        parentsButton.onClick.AddListener(() => SetRole("Parent"));
        teacherButton.onClick.AddListener(() => SetRole("Teacher"));
        loginButton.onClick.AddListener(HandleLogin);
    }

    private void SetRole(string role)
    {
        selectedRole = role;
        UnityEngine.Debug.Log($"Rol seleccionado: {selectedRole}");
    }

    private void HandleLogin()
    {
        tracker?.StartTask("T2");
        float t0 = Time.realtimeSinceStartup;

        if (string.IsNullOrEmpty(selectedRole))
        {
            Fail("Por favor, selecciona un rol.");
            return;
        }

        string username = loginField.text.Trim();
        string password = passwordField.text;

        var request = new GetItemRequest
        {
            TableName = "PlayerData",
            Key = new Dictionary<string, AttributeValue>
            {
                { "PlayerID", new AttributeValue { S = username } }
            }
        };

        try
        {
            // ── MÉTRICA DE RENDIMIENTO R1 ──
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = db.GetItemAsync(request).Result;

            stopwatch.Stop();
            float tiempoR1 = stopwatch.ElapsedMilliseconds / 1000f;
            UnityEngine.Debug.Log($"[R1] Tiempo de respuesta GET PlayerData: {tiempoR1:F2} segundos");

            // Guardar en archivo
            File.AppendAllText("LoginTestResults.txt", $"{System.DateTime.Now} - {username} - {tiempoR1:F2}s\n");

            if (response.Item == null)
            {
                Fail("Usuario no encontrado.");
                return;
            }

            string storedPass = response.Item["Password"].S;
            if (storedPass != password)
            {
                Fail("Contraseña incorrecta.");
                return;
            }

            float elapsed = Time.realtimeSinceStartup - t0;
            UnityEngine.Debug.Log($"Login exitoso en {elapsed:F2}s");

            feedbackText.text = "¡Login exitoso!";
            feedbackText.color = Color.green;

            UserSession.Instance.SetLoggedInUser(username);
            tracker?.EndTask("T2", true);

            switch (selectedRole)
            {
                case "Child": SceneManager.LoadScene("HomeChildScene"); break;
                case "Parent": SceneManager.LoadScene("HomeParentsScene"); break;
                case "Teacher": SceneManager.LoadScene("HomeTeacherScene"); break;
            }
        }
        catch (System.Exception ex)
        {
            Fail($"Error de servidor: {ex.Message}");
        }

        void Fail(string msg)
        {
            UnityEngine.Debug.LogWarning(msg);
            feedbackText.text = msg;
            feedbackText.color = Color.red;
            tracker?.EndTask("T2", false);
        }
    }
}










