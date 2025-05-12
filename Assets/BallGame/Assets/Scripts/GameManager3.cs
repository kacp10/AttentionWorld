using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance;

    public TMP_Text resultText;
    public TMP_Text timerText;

    private List<GameObject> activeBalls;
    private bool inputEnabled = false;
    private float timeRemaining = 60f;
    private bool gameOver = false;

    private int correctCount = 0;
    private int IncorrectCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f; 
        FindObjectOfType<BallSpawner>().SpawnInitialBalls();
        resultText.text = "Cargando pelotas...";
    }

    void Update()
    {
        if (gameOver) return;

        timeRemaining -= Time.deltaTime;
        timerText.text = "Tiempo: " + Mathf.CeilToInt(timeRemaining) + "s";

        if (timeRemaining <= 0)
        {
            EndGame();
            return;
        }

        if (!inputEnabled || activeBalls == null) return;

        for (int i = 0; i < activeBalls.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                CheckAnswer(i + 1);
                break;
            }
        }
    }

    public void SetActiveBalls(List<GameObject> balls)
    {
        activeBalls = balls;
        resultText.color = new Color32(0x3C, 0x8E, 0xAD, 0xFF);
        resultText.text = "Observa las pelotas...";
        inputEnabled = false;

        Invoke(nameof(EnableInput), 3f);
    }

    void EnableInput()
    {
        resultText.color = new Color32(0x3C, 0x8E, 0xAD, 0xFF);
        resultText.text = "¿Cuál pelota salta más alto?";
        inputEnabled = true;
    }

    void CheckAnswer(int pressedNumber)
    {
        if (activeBalls == null || activeBalls.Count == 0) return;

        foreach (var ball in activeBalls)
            ball.GetComponent<Ball>().Stop();

        GameObject highest = activeBalls
            .OrderByDescending(b => b.GetComponent<Ball>().GetMaxHeight())
            .First();

        int correctNumber = highest.GetComponent<Ball>().ballNumber;

        if (pressedNumber == correctNumber)
        {
            resultText.color = Color.green;
            correctCount++;
            resultText.text = " ¡Correcto!";
        }
        else
        {
            resultText.color = Color.red;
            IncorrectCount++;
            resultText.text = $" Incorrecto. Era la #{correctNumber}";
        }

        inputEnabled = false;
        Invoke(nameof(NextRound), 1.5f);
    }

    void NextRound()
    {
        if (!gameOver)
        {
            FindObjectOfType<BallSpawner>().NextRound(false);
        }
    }

    void EndGame()
    {
        gameOver = true;

        // Guardar puntaje y datos para la escena de resultados
        PlayerPrefs.SetInt("CorrectCount", correctCount);
        PlayerPrefs.SetInt("IncorrectCount", IncorrectCount); 
        PlayerPrefs.SetString("CognitiveArea", "atencion"); // se puede hacer dinámico luego
        PlayerPrefs.SetString("GameName", "Pelotas saltarinas");

        // Ir a escena Resultados
        SceneManager.LoadScene("ResultScene");
    }
}
