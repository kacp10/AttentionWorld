using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TMP_Text timerText;
    public TMP_Text correctText;
    public TMP_Text wrongText;
    public TMP_Text resultText;

    public TMP_InputField answerInput;
    public Button confirmButton;

    private float roundTime = 30f;
    private float currentTime;

    private int correctCount = 0;
    private int wrongCount = 0;
    private bool gameActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        confirmButton.onClick.AddListener(CheckAnswer);
        StartGame();
    }

    void Update()
    {
        if (!gameActive) return;

        currentTime -= Time.deltaTime;
        timerText.text = "TIEMPO: " + Mathf.CeilToInt(currentTime).ToString();

        if (currentTime <= 0)
        {
            EndGame();
        }
    }

    void StartGame()
    {
        currentTime = roundTime;
        correctCount = 0;
        wrongCount = 0;
        gameActive = true;
        resultText.text = "";
        NextConstellation();
        UpdateUI();
    }

    void EndGame()
    {
        gameActive = false;
        resultText.text = $"¡Juego terminado!\n✅ {correctCount}  ❌ {wrongCount}";
    }

    void NextConstellation()
    {
        answerInput.text = "";
        ConstellationGenerator.Instance.GenerateRandom();
    }

    public void CheckAnswer()
    {
        if (!gameActive) return;

        int expected = ConstellationGenerator.Instance.GetCurrentStarCount();
        int entered = int.TryParse(answerInput.text, out int value) ? value : -1;

        if (entered == expected)
        {
            correctCount++;
            ConstellationGenerator.Instance.IncreaseRotationSpeed(); // Aumentar velocidad
        }
        else
        {
            wrongCount++;
        }

        UpdateUI();
        NextConstellation();
    }


    void UpdateUI()
    {
        correctText.text = "✅ " + correctCount;
        wrongText.text = "❌ " + wrongCount;
    }
}
