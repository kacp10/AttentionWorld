using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;          // Stopwatch
using System.IO;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

public class LoginManager : MonoBehaviour
{
    /*────────── Referencias UI ──────────*/
    [Header("Inputs")]
    [SerializeField] TMP_InputField loginField;
    [SerializeField] TMP_InputField passwordField;

    [Header("Botones")]
    [SerializeField] Button loginButton;
    [SerializeField] Button childButton;
    [SerializeField] Button parentsButton;
    [SerializeField] Button teacherButton;

    [Header("Feedback")]
    [SerializeField] TMP_Text feedbackText;

    /*────────── AWS & tracking ──────────*/
    AmazonDynamoDBClient db;
    string selectedRole = "";                // Child | Parents | Teacher
    UsabilityTracker tracker;                // Métrica T2

    /*─────────────────────────────────────*/
    void Start()
    {
        // Credenciales demo
        var cred = new BasicAWSCredentials(
            "",
            "");
        db = new AmazonDynamoDBClient(cred, Amazon.RegionEndpoint.USEast1);

        if (UserSession.Instance == null)
            new GameObject("UserSessionManager").AddComponent<UserSession>();

        tracker = FindObjectOfType<UsabilityTracker>();

        /* Rol elegido */
        childButton.onClick.AddListener(() => SetRole("Child"));
        parentsButton.onClick.AddListener(() => SetRole("Parents"));
        teacherButton.onClick.AddListener(() => SetRole("Teacher"));

        loginButton.onClick.AddListener(HandleLogin);
    }

    void SetRole(string role)
    {
        selectedRole = role;
        UnityEngine.Debug.Log($"Rol seleccionado: {selectedRole}");
    }

    /*───────────────────────── LOGIN ─────────────────────────*/
    void HandleLogin()
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
            /*──────── Métrica R1 (rendimiento de login) ────────*/
            var sw = Stopwatch.StartNew();

            var response = db.GetItemAsync(request).Result;

            sw.Stop();
            float r1 = sw.ElapsedMilliseconds / 1000f;
            UnityEngine.Debug.Log($"[R1] GET PlayerData: {r1:F2}s");
            File.AppendAllText("LoginTestResults.txt",
                $"{System.DateTime.Now} - {username} - {r1:F2}s\n");

            /*──────── Validaciones ────────*/
            if (response.Item == null) { Fail("Usuario no encontrado."); return; }
            if (response.Item["Password"].S != password) { Fail("Contraseña incorrecta."); return; }

            /*──────── Login OK ────────*/
            float elapsed = Time.realtimeSinceStartup - t0;
            UnityEngine.Debug.Log($"Login exitoso en {elapsed:F2}s");

            feedbackText.text = "¡Login exitoso!";
            feedbackText.color = Color.green;

            /* Guarda usuario y ROL correcto */
            UserSession.Instance.SetLoggedInUser(username, selectedRole);

            tracker?.EndTask("T2", true);

            /*──────── Navegación ────────*/
            switch (selectedRole)
            {
                case "Child": SceneManager.LoadScene("HomeChildScene"); break;
                case "Parents": SceneManager.LoadScene("HomeParentsScene"); break;
                case "Teacher": SceneManager.LoadScene("HomeTeacherScene"); break;
            }
        }
        catch (System.Exception ex)
        {
            Fail($"Error de servidor: {ex.Message}");
        }

        /*──────── Local helper ────────*/
        void Fail(string msg)
        {
            UnityEngine.Debug.LogWarning(msg);
            feedbackText.text = msg;
            feedbackText.color = Color.red;
            tracker?.EndTask("T2", false);
        }
    }
}
