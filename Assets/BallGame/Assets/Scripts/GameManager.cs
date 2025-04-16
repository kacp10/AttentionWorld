using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TMP_InputField inputField;
    public TMP_Text resultText;

    private List<GameObject> activeBalls;
    private bool inputEnabled = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        FindObjectOfType<BallSpawner>().SpawnInitialBalls();
        inputField.onSubmit.AddListener(delegate { CheckAnswer(); });
        inputField.interactable = false;
    }

    public void SetActiveBalls(List<GameObject> balls)
    {
        activeBalls = balls;
        resultText.text = "Observa...";
        inputField.text = "";
        inputField.interactable = false;
        inputEnabled = false;

        Invoke(nameof(EnableInput), 2f);
    }

    void EnableInput()
    {
        resultText.text = "¿Cuál pelota salta más alto?";
        inputField.interactable = true;
        inputField.ActivateInputField();
        inputEnabled = true;
    }

    public void CheckAnswer()
    {
        if (!inputEnabled || activeBalls == null || activeBalls.Count == 0)
            return;

        inputEnabled = false;
        inputField.interactable = false;

        foreach (var ball in activeBalls)
        {
            ball.GetComponent<Ball>().Stop(); // ❗ Detiene las pelotas
        }

        GameObject highest = activeBalls.OrderByDescending(b => b.GetComponent<Ball>().GetMaxHeight()).First();
        int correctNumber = highest.GetComponent<Ball>().ballNumber;

        if (inputField.text.Trim() == correctNumber.ToString())
        {
            resultText.text = "✔️ ¡Correcto!";
            Invoke(nameof(NextRound), 1.5f);
        }
        else
        {
            resultText.text = $"❌ Era la #{correctNumber}. Intentémoslo otra vez.";
            Invoke(nameof(RepeatRound), 2f);
        }
    }

    void NextRound()
    {
        FindObjectOfType<BallSpawner>().NextRound(false); // avanza
    }

    void RepeatRound()
    {
        FindObjectOfType<BallSpawner>().NextRound(true); // repite
    }
}
