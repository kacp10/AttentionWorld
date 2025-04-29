using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance;

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

        // Esperar más para que todas lleguen a su punto máximo
        Invoke(nameof(EnableInput), 3f);
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

        foreach (var ball in activeBalls)
        {
            ball.GetComponent<Ball>().Stop(); // detener rebote al responder
        }

        // Mostrar en consola las alturas
        foreach (var ball in activeBalls)
        {
            Debug.Log("Ball #" + ball.GetComponent<Ball>().ballNumber + " altura: " + ball.GetComponent<Ball>().GetMaxHeight());
        }

        GameObject highest = activeBalls
            .OrderByDescending(b => b.GetComponent<Ball>().GetMaxHeight())
            .First();

        int correctNumber = highest.GetComponent<Ball>().ballNumber;

        if (inputField.text.Trim() == correctNumber.ToString())
        {
            resultText.text = "✔️ ¡Correcto!";
            inputField.interactable = false;
            inputEnabled = false;
            Invoke(nameof(NextRound), 1.5f);
        }
        else
        {
            resultText.text = $"❌ Incorrecto. Era la #{correctNumber}";
            inputField.interactable = false;
            inputEnabled = false;
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
