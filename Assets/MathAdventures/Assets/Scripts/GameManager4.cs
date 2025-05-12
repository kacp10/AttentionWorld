using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager4 : MonoBehaviour
{
    public static GameManager4 Instance;

    public TMP_Text questionText;
    public TMP_InputField answerInput;
    public TMP_Text resultText;
    public TMP_Text timerText;

    private float currentTime;
    private int correctCount = 0;
    private int incorrectCount = 0;
    private int correctAnswer;
    private int ageRange;

    public float timeLimit = 5f;
    private bool gameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ageRange = PlayerPrefs.GetInt("AgeRange", 0);
        currentTime = timeLimit;
        answerInput.ActivateInputField();
        answerInput.onSubmit.AddListener(delegate { CheckAnswer(); });
        answerInput.placeholder.gameObject.SetActive(false); // Sin placeholder
        ShowNewQuestion();
    }

    void Update()
    {
        if (gameOver) return;

        currentTime -= Time.deltaTime;
        timerText.text = "Tiempo: " + Mathf.CeilToInt(currentTime) + "s";

        if (currentTime <= 0)
        {
            EndGame();
        }
    }

    void ShowNewQuestion()
    {
        int num1 = Random.Range(1, 20);
        int num2 = Random.Range(1, 20);

        if (ageRange == 0) // 6–7 años
        {
            if (Random.Range(0, 2) == 0)
            {
                questionText.text = $"{num1} + {num2}";
                correctAnswer = num1 + num2;
            }
            else
            {
                if (num1 < num2) num1 = num2 + Random.Range(1, 5);
                questionText.text = $"{num1} - {num2}";
                correctAnswer = num1 - num2;
            }
        }
        else // 8–10 años
        {
            int op = Random.Range(0, 3);
            if (op == 0)
            {
                questionText.text = $"{num1} + {num2}";
                correctAnswer = num1 + num2;
            }
            else if (op == 1)
            {
                if (num1 < num2) num1 = num2 + Random.Range(1, 5);
                questionText.text = $"{num1} - {num2}";
                correctAnswer = num1 - num2;
            }
            else
            {
                num1 = Random.Range(1, 10);
                num2 = Random.Range(1, 10);
                questionText.text = $"{num1} × {num2}";
                correctAnswer = num1 * num2;
            }
        }

        answerInput.text = "";
        resultText.text = "";
        answerInput.ActivateInputField();
    }

    public void CheckAnswer()
    {
        if (gameOver) return;

        string userAnswer = answerInput.text.Trim();
        answerInput.ActivateInputField();

        if (int.TryParse(userAnswer, out int userValue))
        {
            if (userValue == correctAnswer)
            {
                resultText.text = "✔ ¡Correcto!";
                resultText.color = Color.green;
                correctCount++;
            }
            else
            {
                resultText.text = $"✘ Incorrecto";
                resultText.color = Color.red;
                incorrectCount++;
            }
        }
        else
        {
            resultText.text = "⛔ Ingresa un número válido";
            resultText.color = Color.yellow;
        }

        Invoke(nameof(ShowNewQuestion), 1.2f);
    }

    void EndGame()
    {
        gameOver = true;
        answerInput.interactable = false;

        // Guardar resultados en PlayerPrefs
        PlayerPrefs.SetInt("CorrectCount", correctCount);
        PlayerPrefs.SetInt("IncorrectCount", incorrectCount);
        PlayerPrefs.SetString("CognitiveArea", "calculo");
        PlayerPrefs.SetString("GameName", "Cálculo divertido");

        Invoke(nameof(LoadResultScene), 2f);
    }

    void LoadResultScene()
    {
        SceneManager.LoadScene("ResultScene");
    }
}
