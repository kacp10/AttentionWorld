using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnlyGameSceneManager : MonoBehaviour
{
    [Header("Botones de �reas Cognitivas")]
    public Button atentionButton;
    public Button memoryButton;
    public Button logicButton;
    public Button calculateButton;

    [Header("Bot�n de Regreso")]
    public Button backButton;

    void Start()
    {
        atentionButton.onClick.AddListener(() => LoadScene("AttentionScene"));   // Introducci�n Atenci�n
        memoryButton.onClick.AddListener(() => LoadScene("MemoryScene"));         // Introducci�n Memoria
        logicButton.onClick.AddListener(() => LoadScene("LogicScene"));           // Introducci�n L�gica
        calculateButton.onClick.AddListener(() => LoadScene("CalculateScene"));   // Introducci�n C�lculo

        backButton.onClick.AddListener(() => LoadScene("GameScene"));                  // Escena anterior general
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
