using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProgressSceneManager : MonoBehaviour
{
    [Header("Botones")]
    public Button goToCalendarButton;
    public Button goToGraficButton;   // Nuevo botón para la gráfica
    public Button goToClasificationButton;
    public Button goToRegisterButton;

    void Start()
    {
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
}
