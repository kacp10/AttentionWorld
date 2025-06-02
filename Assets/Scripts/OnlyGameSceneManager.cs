using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnlyGameSceneManager : MonoBehaviour
{
    [Header("Botones de Áreas Cognitivas")]
    public Button atentionButton;
    public Button memoryButton;
    public Button logicButton;
    public Button calculateButton;

    [Header("Botón de Regreso")]
    public Button backButton;

    void Start()
    {
        atentionButton.onClick.AddListener(() => LoadScene("AttentionScene"));   // Introducción Atención
        memoryButton.onClick.AddListener(() => LoadScene("MemoryScene"));         // Introducción Memoria
        logicButton.onClick.AddListener(() => LoadScene("LogicScene"));           // Introducción Lógica
        calculateButton.onClick.AddListener(() => LoadScene("CalculateScene"));   // Introducción Cálculo

        backButton.onClick.AddListener(() => LoadScene("GameScene"));                  // Escena anterior general
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
