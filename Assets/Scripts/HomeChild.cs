using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeChild : MonoBehaviour
{
    [Header("Botones Principal")]
    public Button startGameButton;  // Iniciar juego
    public Button progressButton;   // Ver progreso
    public Button settingsButton;   // Ajustes
    public Button profileButton;    // Perfil
    public Button helpButton;
    public Button logoutButton; 

    void Start()
    {
        startGameButton.onClick.AddListener(GoToGameScene);
        progressButton.onClick.AddListener(GoToProgressScene);
        settingsButton.onClick.AddListener(GoToSettingsScene);
        profileButton.onClick.AddListener(GoToProfileScene);
        helpButton.onClick.AddListener(GoToHelpScene);
        logoutButton.onClick.AddListener(GoToLoginScene);
    }

    void GoToGameScene()
    {
        Debug.Log("[HomeChild] Cargando GameScene...");
        SceneManager.LoadScene("GameScene");
    }
   
    void GoToProgressScene()
    {
        if (UserSession.Instance == null)
        {
            Debug.LogError("[HomeChild] UserSession no encontrada: redirigiendo a LoginScene");
            SceneManager.LoadScene("LoginScene");
            return;
        }
        string user = UserSession.Instance.GetLoggedInUser();
        if (string.IsNullOrWhiteSpace(user))
        {
            Debug.LogError("[HomeChild] Usuario no logueado: redirigiendo a LoginScene");
            SceneManager.LoadScene("LoginScene");
        }
        else
        {
            Debug.Log($"[HomeChild] Usuario: {user}. Cargando ProgressScene...");
            SceneManager.LoadScene("ProgressScene");
        }
    }

    void GoToSettingsScene()
    {
        Debug.Log("[HomeChild] Cargando SettingsScene...");
        SceneManager.LoadScene("SettingsScene");
    }

    void GoToProfileScene()
    {
        Debug.Log("[HomeChild] Cargando ProfileManager...");
        SceneManager.LoadScene("ProfileManager");
    }

    void GoToHelpScene()
    {
        Debug.Log("[HomeChild] Cargando HelpScene...");
        SceneManager.LoadScene("HelpScene");
    }
    void GoToLoginScene()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
