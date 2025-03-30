using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeChild : MonoBehaviour
{
    public Button startGameButton;  // Botón para iniciar el juego
    public Button progressButton;   // Botón para ver el progreso
    public Button settingsButton;   // Botón para ajustes
    public Button profileButton;    // Botón para perfil
    public Button helpButton;       // Botón para ayuda

    void Start()
    {
        startGameButton.onClick.AddListener(GoToGameScene);
        progressButton.onClick.AddListener(GoToProgressScene);
        settingsButton.onClick.AddListener(GoToPlaceholderScene);
        profileButton.onClick.AddListener(GoToPlaceholderScene);
        helpButton.onClick.AddListener(GoToPlaceholderScene);
    }

    void GoToGameScene()
    {
        Debug.Log("Botón de Start Game fue presionado. Intentando cargar GameScene.");
        SceneManager.LoadScene("GameScene");
    }

    void GoToProgressScene()
    {
        // Verifica que exista un usuario logueado usando el método GetLoggedInUser()
        if (UserSession.Instance == null)
        {
            Debug.LogError("No se ha encontrado una instancia de UserSession.");
            SceneManager.LoadScene("LoginScene");
            return;
        }

        string loggedInUser = UserSession.Instance.GetLoggedInUser();

        if (string.IsNullOrWhiteSpace(loggedInUser))
        {
            Debug.LogError("No se ha encontrado un usuario logueado. Redirigiendo al login.");
            SceneManager.LoadScene("LoginScene");
        }
        else
        {
            Debug.Log("Usuario logueado: " + loggedInUser + ". Cargando progressScene.");
            SceneManager.LoadScene("progressScene");
        }
    }

    void GoToPlaceholderScene()
    {
        Debug.Log("Esta funcionalidad se implementará más adelante.");
    }
}
