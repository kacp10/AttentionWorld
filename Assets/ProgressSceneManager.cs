using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProgressSceneManager : MonoBehaviour
{
    [Header("Botones")]
    public Button goToCalendarButton;
    public Button goToGraficButton;
    public Button goToClasificationButton;
    public Button goToRegisterButton;
    public Button backButton;
    bool isInitialized = false;

    void Start()
    {
        isInitialized = true;

        backButton.onClick.AddListener(ReturnToHomeScene);

        // Listener para abrir el calendario
        goToCalendarButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("CalendarScene");
        });

        // Listener para abrir la gráfica
        goToGraficButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("GraficScene");
        });

        goToClasificationButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("ClasificationScene");
        });

        goToRegisterButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("RegisterDataScene");
        });
    }

    void ReturnToHomeScene()
    {
        // Detectar rol actual
        string role = UserSession.Instance != null ? UserSession.Instance.GetCurrentRole() : "Child";

        switch (role)
        {
            case "Parents":
                SceneManager.LoadScene("HomeParentsScene");
                break;
            case "Teacher":
                SceneManager.LoadScene("HomeTeacherScene");
                break;
            default: // Child u otro
                SceneManager.LoadScene("HomeChildScene");
                break;
        }
    }
}
