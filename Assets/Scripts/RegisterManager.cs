using UnityEngine;
using UnityEngine.UI;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using TMPro;
using System.Threading.Tasks;
using System.Linq;  // Aseg�rate de incluir esta l�nea para usar m�todos LINQ

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField userNameInput;  // Campo de texto para el nombre completo
    public TMP_InputField emailInput;  // Campo de texto para el email
    public TMP_InputField passwordInput;  // Campo de texto para la contrase�a
    public TMP_InputField confirmPasswordInput;  // Campo de texto para confirmar la contrase�a
    public TMP_InputField yearOfBirthInput;  // A�o de nacimiento
    public TMP_Dropdown classroomDropdown;  // Dropdown para seleccionar el aula (solo para ni�o y docente)
    public TMP_Dropdown childDropdown;  // Dropdown para que el padre seleccione al ni�o (solo si es padre)
    public Button registerButton;  // Bot�n de registro
    public TMP_Text feedbackText;  // Texto para mostrar los mensajes de feedback

    // Botones para seleccionar rol
    public Button childButton;  // Bot�n para seleccionar el rol de Child
    public Button parentsButton;  // Bot�n para seleccionar el rol de Parents
    public Button teacherButton;  // Bot�n para seleccionar el rol de Teacher

    private AmazonDynamoDBClient dynamoDBClient;
    private string userRole = "";  // El rol del usuario (Child, Parents, Teacher)

    void Start()
    {
        // Inicializa el cliente de DynamoDB
        var credentials = new Amazon.CognitoIdentity.CognitoAWSCredentials("", Amazon.RegionEndpoint.USEast1);
        dynamoDBClient = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast1);

        // Asignar eventos de clic a los botones de rol
        childButton.onClick.AddListener(() => SetUserRole("Child"));
        parentsButton.onClick.AddListener(() => SetUserRole("Parents"));
        teacherButton.onClick.AddListener(() => SetUserRole("Teacher"));

        // Asignar el evento de clic al bot�n de registro
        registerButton.onClick.AddListener(async () => await HandleRegister());

        // Cargar los dropdowns de ni�os y salones al inicio
        LoadChildDropdown();
        LoadClassroomDropdown();
    }

    // Cambiar visibilidad de campos seg�n el rol seleccionado
    void SetUserRole(string role)
    {
        userRole = role;
        UpdateFormBasedOnRole();
    }

    // Configura la visibilidad y bloqueo de los campos seg�n el rol
    void UpdateFormBasedOnRole()
    {
        // Al principio, todo est� desbloqueado
        emailInput.interactable = true;
        yearOfBirthInput.interactable = true;
        classroomDropdown.interactable = true;
        childDropdown.interactable = true;

        // Bloquea campos seg�n el rol
        if (userRole == "Child")
        {
            // Para los ni�os: bloquear los campos email y childDropdown
            emailInput.interactable = false;  // Deshabilitar el campo de email
            childDropdown.interactable = false;  // Deshabilitar el dropdown de ni�os
        }
        else if (userRole == "Parents")
        {
            // Para los padres: bloquear el campo de yearOfBirthInput
            yearOfBirthInput.interactable = false;  // Deshabilitar el campo de a�o de nacimiento
            classroomDropdown.interactable = true;  // El padre puede seleccionar el sal�n
            childDropdown.interactable = true;  // El padre puede seleccionar el ni�o
        }
        else if (userRole == "Teacher")
        {
            // Para los docentes: bloquear el campo de yearOfBirthInput
            yearOfBirthInput.interactable = false;  // Deshabilitar el campo de a�o de nacimiento
            classroomDropdown.interactable = true;  // Los docentes pueden seleccionar el sal�n
            childDropdown.interactable = false;  // Los docentes no pueden seleccionar al ni�o
        }
    }

    // M�todo para cargar el Dropdown de ni�os
    async void LoadChildDropdown()
    {
        // Realizamos la consulta para obtener los nombres de los ni�os desde DynamoDB
        var request = new ScanRequest
        {
            TableName = "PlayerData",
            FilterExpression = "#role = :role",  // Usamos #role como alias para "Role"
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#role", "Role" }  // Alias para la palabra reservada "Role"
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":role", new AttributeValue { S = "Child" } }
            }
        };

        var response = await dynamoDBClient.ScanAsync(request);
        List<string> childrenNames = new List<string>();

        foreach (var item in response.Items)
        {
            // Verificar si el atributo 'Name' existe antes de agregarlo al dropdown
            if (item.ContainsKey("Name"))
            {
                string childName = item["Name"].S;  // Obtener el nombre del ni�o
                childrenNames.Add(childName);
            }
            else
            {
                Debug.LogWarning("Registro sin 'Name': " + item["PlayerID"].S);
            }
        }

        // Llenamos el dropdown con los nombres de los ni�os
        childDropdown.ClearOptions();
        childDropdown.AddOptions(childrenNames);
    }

    // M�todo para cargar el Dropdown de salones
    void LoadClassroomDropdown()
    {
        // Aqu� agregamos los salones disponibles, por ahora solo Salon A y Salon B
        List<string> classrooms = new List<string> { "Salon A", "Salon B" };

        classroomDropdown.ClearOptions();
        classroomDropdown.AddOptions(classrooms);
    }

    private async Task HandleRegister()
    {
        string fullName = userNameInput.text;  // Obtener nombre completo
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;
        string yearOfBirth = yearOfBirthInput.text;
        string selectedClassroom = classroomDropdown.options[classroomDropdown.value].text;

        // Validar contrase�as
        if (password != confirmPassword)
        {
            feedbackText.text = "Las contrase�as no coinciden.";
            feedbackText.color = Color.red;
            return;
        }

        // Generar el PlayerID basado en el nombre completo
        string playerID = GeneratePlayerID(fullName);

        // Crear el registro para DynamoDB
        var request = new PutItemRequest
        {
            TableName = "PlayerData",
            Item = new Dictionary<string, AttributeValue>
            {
                { "PlayerID", new AttributeValue { S = playerID } },  // Generado del nombre completo
                { "Password", new AttributeValue { S = password } },
                { "Role", new AttributeValue { S = userRole } },
                { "Email", new AttributeValue { S = email } },  // Solo si el rol no es 'Child'
                { "Name", new AttributeValue { S = fullName } },  // Subir el nombre completo
                { "YearOfBirth", new AttributeValue { S = yearOfBirth } }, // A�o de nacimiento (solo para ni�o)
                { "Classroom", new AttributeValue { S = selectedClassroom } } // El aula seleccionada
            }
        };

        // Si el rol es 'Child', eliminar el campo de email de la solicitud
        if (userRole == "Child")
        {
            request.Item.Remove("Email");
        }

        // Si el rol es 'Padre', asociar al ni�o seleccionado
        if (userRole == "Parents")
        {
            string selectedChild = childDropdown.options[childDropdown.value].text;
            request.Item.Add("ParentID", new AttributeValue { S = selectedChild });
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

    // M�todo para generar el PlayerID
    private string GeneratePlayerID(string fullName)
    {
        string[] nameParts = fullName.Split(' ');  // Dividir por espacios
        if (nameParts.Length >= 3)
        {
            string firstNameLetter = nameParts[0].Substring(0, 1);  // Primera letra del primer nombre
            string secondNameLetter = nameParts[1].Substring(0, 1); // Primera letra del segundo nombre
            string lastName = nameParts[2];  // Apellido completo

            return (firstNameLetter + secondNameLetter + lastName).ToLower(); // Convertir a min�sculas
        }
        else
        {
            return ""; // Retornar vac�o si el nombre completo no tiene la estructura correcta
        }
    }

    void GoToLoginScene()
    {
        // Aqu� cargamos la escena de login
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }
}
