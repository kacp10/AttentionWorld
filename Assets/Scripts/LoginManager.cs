using UnityEngine;
using UnityEngine.UI;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;  // Para poder cambiar de escena

public class LoginManager : MonoBehaviour
{
    public TMP_InputField loginField;  // Campo de texto para el nombre de usuario
    public TMP_InputField passwordField;  // Campo de texto para la contraseña
    public Button loginButton;  // Botón de login
    public TMP_Text feedbackText;  // Texto para mostrar los mensajes

    public Button childButton;  // Botón para seleccionar el rol "Child"
    public Button parentsButton; // Botón para seleccionar el rol "Parent"
    public Button teacherButton; // Botón para seleccionar el rol "Teacher"
    private string selectedRole = ""; // Variable para almacenar el rol seleccionado

    private AmazonDynamoDBClient dynamoDBClient;

    void Start()
    {
        var credentials = new BasicAWSCredentials("", ""); // Inicializa el cliente de DynamoDB con las credenciales de AWS

        dynamoDBClient = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast1);

        // Asegurarse de que UserSession exista
        if (UserSession.Instance == null)
        {
            GameObject userSessionObject = new GameObject("UserSessionManager");
            userSessionObject.AddComponent<UserSession>();
        }

        // Asignar eventos de clic a los botones de rol
        childButton.onClick.AddListener(() => SetRole("Child"));
        parentsButton.onClick.AddListener(() => SetRole("Parent"));
        teacherButton.onClick.AddListener(() => SetRole("Teacher"));

        // Asigna el evento de clic al botón de login
        loginButton.onClick.AddListener(HandleLogin);
    }

    // Método para asignar el rol seleccionado
    void SetRole(string role)
    {
        selectedRole = role;
        Debug.Log($"Rol seleccionado: {selectedRole}");  // Imprimir en consola el rol seleccionado
    }

    void HandleLogin()
    {
        // Verificar si se ha seleccionado un rol antes de continuar
        if (string.IsNullOrEmpty(selectedRole))
        {
            feedbackText.text = "Por favor, selecciona un rol.";
            feedbackText.color = Color.red;
            return; // Evitar continuar si no se ha seleccionado un rol
        }

        string username = loginField.text;
        string password = passwordField.text;

        // Realizar una consulta en DynamoDB para verificar las credenciales
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
            // Consulta sincrónica para evitar problemas con callbacks
            var response = dynamoDBClient.GetItemAsync(request).Result;

            if (response.Item != null)
            {
                var storedPassword = response.Item["Password"].S;

                if (storedPassword == password)
                {
                    Debug.Log("Login exitoso");
                    feedbackText.text = "Login Exitoso!";
                    feedbackText.color = Color.green;

                    if (!string.IsNullOrEmpty(username))
                    {
                        Debug.Log($"✅ Guardando usuario en sesión: {username}");
                        UserSession.Instance.SetLoggedInUser(username);

                        // Redirigir a la escena correspondiente según el rol
                        if (selectedRole == "Child")
                        {
                            SceneManager.LoadScene("HomeChildScene");
                        }
                        else if (selectedRole == "Parent")
                        {
                            SceneManager.LoadScene("HomeParentScene"); // Cambia por la escena de padres
                        }
                        else if (selectedRole == "Teacher")
                        {
                            SceneManager.LoadScene("HomeTeacherScene"); // Cambia por la escena de maestros
                        }
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ El valor de username está vacío o nulo en LoginManager.");
                    }
                }
                else
                {
                    Debug.Log("Contraseña incorrecta");
                    feedbackText.text = "Contraseña incorrecta.";
                    feedbackText.color = Color.red;
                }
            }
            else
            {
                Debug.Log("Usuario no encontrado");
                feedbackText.text = "Usuario no encontrado.";
                feedbackText.color = Color.red;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error en la consulta a DynamoDB: " + ex.Message);
            feedbackText.text = "Error del servidor. Intenta de nuevo.";
            feedbackText.color = Color.red;
        }
    }
}

