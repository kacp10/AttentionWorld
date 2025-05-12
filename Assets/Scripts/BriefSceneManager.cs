using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BriefSceneManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text explanationText;
    public Button startButton;

    void Start()
    {
        string playerId = UserSession.Instance?.GetLoggedInUser() ?? "Jugador";

        int gameIndex = PlayerPrefs.GetInt("CurrentGameIndex", 0);
        var data = GameSessionData.Instance.GetCurrentAssignment();

        titleText.text = $"{data.DisplayName} ({playerId})";

        explanationText.text = $"En el {gameIndex + 1} de los 4 ejercicios de hoy, " +
                               $"trabajaremos la parte de {data.Area.ToLower()} de su cerebro.\n\n" +
                               data.Instruction;

        startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(data.SceneName);
        });
    }
}
