using UnityEngine;
using UnityEngine.UI;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using TMPro;
using System.Threading.Tasks;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField userNameInput;  // Campo de texto para el usuario
    public TMP_InputField emailInput;  // Campo de texto para el email
    public TMP_InputField passwordInput;  // Campo de texto para la contrase�a
    public TMP_InputField confirmPasswordInput;  // Campo de texto para confirmar la contrase�a
    public Button registerButton;  // Bot�n de registro
    public Button loginButton;  // Bot�n de login
    public Button childButton;  // Bot�n para seleccionar el rol de Child
    public Button parentsButton;  // Bot�n para seleccionar el rol de Parents
    public Button teacherButton;  // Bot�n para seleccionar el rol de Teacher
    public TMP_Text feedbackText;  // Texto para mostrar los mensajes de feedback
    public GameObject emailField;  // Referencia al campo del correo electr�nico, para ocultarlo si es ni�o

    private AmazonDynamoDBClient dynamoDBClient;

    private string userRole = "Parents";  // El rol del usuario (Child, Parents, Teacher)

    void Start()
    {
        // Configura la visibilidad del campo de email
        UpdateEmailFieldVisibility();

        // Inicializa el cliente de DynamoDB
        var credentials = new Amazon.CognitoIdentity.CognitoAWSCredentials(
           // Reemplaza con tu ID de grupo de identidades
            Amazon.RegionEndpoint.USEast1
        );
        dynamoDBClient = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast1);

        // Asignar el evento de clic al bot�n de registro
        registerButton.onClick.AddListener(async () => await HandleRegister());

        // Asignar el evento de clic al bot�n de login
        loginButton.onClick.AddListener(GoToLoginScene);

        // Asignar los eventos de clic a los botones de rol
        childButton.onClick.AddListener(() => SetUserRole("Child"));
        parentsButton.onClick.AddListener(() => SetUserRole("Parents"));
        teacherButton.onClick.AddListener(() => SetUserRole("Teacher"));
    }

    void SetUserRole(string role)
    {
        userRole = role;
        UpdateEmailFieldVisibility();
    }

    void UpdateEmailFieldVisibility()
    {
        // Ocultar el campo de email si el rol es Child
        if (userRole == "Child")
        {
            emailField.SetActive(false);
        }
        else
        {
            emailField.SetActive(true);
        }
    }

    private async Task HandleRegister()
    {
        string username = userNameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (password != confirmPassword)
        {
            feedbackText.text = "Las contrase�as no coinciden.";
            feedbackText.color = Color.red;
            return;
        }

        var request = new PutItemRequest
        {
            TableName = "PlayerData",
            Item = new Dictionary<string, AttributeValue>
            {
                { "PlayerID", new AttributeValue { S = username } },
                { "Password", new AttributeValue { S = password } },
                { "Role", new AttributeValue { S = userRole } },
                { "Email", new AttributeValue { S = email } }  // Solo si el rol no es 'Child'
            }
        };

        // Si el rol es 'Child', eliminar el campo de email de la solicitud
        if (userRole == "Child")
        {
            request.Item.Remove("Email");
        }

        try
        {
            await dynamoDBClient.PutItemAsync(request);
            feedbackText.text = "Registro exitoso. Por favor, inicia sesi�n.";
            feedbackText.color = Color.green;

            // Redirigir a la escena de login despu�s de un breve retraso
            Invoke("GoToLoginScene", 2);
        }
        catch (System.Exception ex)
        {
            feedbackText.text = $"Error al registrar: {ex.Message}";
            feedbackText.color = Color.red;
        }
    }

    void GoToLoginScene()
    {
        // Aqu� cargamos la escena de login
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }
}
