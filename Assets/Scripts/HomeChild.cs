using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeChild : MonoBehaviour
{
    public Button startGameButton;  // Bot�n para iniciar el juego
    public Button progressButton;   // Bot�n para ver el progreso
    public Button settingsButton;   // Bot�n para ajustes
    public Button profileButton;    // Bot�n para perfil
    public Button helpButton;       // Bot�n para ayuda

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
        Debug.Log("Bot�n de Start Game fue presionado. Intentando cargar GameScene.");
        SceneManager.LoadScene("GameScene");
    }

    void GoToProgressScene()
    {
        // Verifica que exista un usuario logueado usando el m�todo GetLoggedInUser()
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
        Debug.Log("Esta funcionalidad se implementar� m�s adelante.");
    }
}
