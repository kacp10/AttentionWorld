using UnityEngine;
using UnityEngine.UI;        // Para Button
using UnityEngine.SceneManagement;  // Para SceneManager
using TMPro;                 // Para TextMeshProUGUI

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI greetingText; // Arrástralo desde el Inspector

    void Start()
    {
        if (greetingText == null)
        {
            greetingText = GameObject.Find("greetingText").GetComponent<TextMeshProUGUI>();
            Debug.Log("Asignación manual de greetingText: " + greetingText);
        }

        string currentUser = UserSession.Instance.GetLoggedInUser();
        Debug.Log("Usuario recuperado desde UserSession: " + currentUser);

        if (!string.IsNullOrEmpty(currentUser))
        {
            greetingText.text = $"¡Hola {currentUser}! ¿Qué tipo de ejercicio deseas realizar hoy?";
            Debug.Log("✅ Texto actualizado con el usuario.");
        }
        else
        {
            Debug.LogWarning("⚠️ currentUser está vacío o nulo.");
        }
    }


}

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button dailyExerciseButton;
    [SerializeField] private Button singleExerciseButton;
    [SerializeField] private Button backButton; // Opcional

    void Start()
    {
        // Asignar listeners a los botones (asegúrate de haberlos arrastrado en el Inspector)
        dailyExerciseButton.onClick.AddListener(GoToDailyExercise);
        singleExerciseButton.onClick.AddListener(GoToSingleExercise);

        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBack);
        }
    }

    void GoToDailyExercise()
    {
        // Cambia "DailyExerciseScene" por el nombre real de la escena que tengas
        SceneManager.LoadScene("DailyExerciseScene");
    }

    void GoToSingleExercise()
    {
        // Cambia "SingleExerciseScene" por el nombre real de la escena que tengas
        SceneManager.LoadScene("SingleExerciseScene");
    }

    void GoBack()
    {
        // Carga la escena anterior o la que consideres tu "Menú"
        SceneManager.LoadScene("MainMenu");
    }
}
