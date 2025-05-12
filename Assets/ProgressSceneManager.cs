using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProgressSceneManager : MonoBehaviour
{
    [Header("Botones")]
    public Button goToCalendarButton;
    public Button goToGraficButton;   // ← Nuevo botón para la gráfica

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
    }
}
