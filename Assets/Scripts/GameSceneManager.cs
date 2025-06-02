using UnityEngine;
using UnityEngine.UI;        // Para Button
using UnityEngine.SceneManagement;  // Para SceneManager
using TMPro;                 // Para TextMeshProUGUI

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI greetingText; // Arrástralo desde el Inspector
    [SerializeField] private Button dailyExerciseButton;   // 🔥 Ahora visible en el Inspector
    [SerializeField] private Button singleExerciseButton;  // 🔥 Ahora visible en el Inspector
    [SerializeField] private Button backButton;            // 🔥 Ahora visible en el Inspector

    void Start()
    {
        // Mensaje de saludo
        if (greetingText == null)
        {
            greetingText = GameObject.Find("greetingText")?.GetComponent<TextMeshProUGUI>();
            Debug.Log("Asignación manual de greetingText: " + greetingText);
        }

        string currentUser = UserSession.Instance?.GetLoggedInUser();
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

        // Asignación de botones
        if (dailyExerciseButton != null)
            dailyExerciseButton.onClick.AddListener(() => SceneManager.LoadScene("GameAssignScene"));

        if (singleExerciseButton != null)
            singleExerciseButton.onClick.AddListener(() => SceneManager.LoadScene("SingleExerciseScene"));

        if (backButton != null)
            backButton.onClick.AddListener(() => SceneManager.LoadScene("HomeChildScene"));
    }
}
