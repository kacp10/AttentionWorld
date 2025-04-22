using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TMP_Text questionText;  // Muestra la pregunta
    public TMP_InputField answerInput; // Entrada del jugador
    public TMP_Text resultText;    // Muestra el resultado (correcto o incorrecto)
    public TMP_Text scoreText;     // Muestra el puntaje
    public TMP_Text timerText;     // Muestra el tiempo restante
    public int score = 0;          // Puntaje inicial
    public float timeLimit = 30f;  // Tiempo limitado
    private float currentTime;     // Tiempo actual

    private int correctAnswer;     // Respuesta correcta de la pregunta
    private int ageRange;          // Rango de edad

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Esto asegura que el GameManager no se destruya entre escenas
        }
        else
        {
            Destroy(gameObject);  // Si ya existe una instancia, destruye este objeto
        }
    }

    void Start()
    {
        ageRange = PlayerPrefs.GetInt("AgeRange", 0);  // Recuperar el rango de edad desde PlayerPrefs
        currentTime = timeLimit;
        ShowNewQuestion();
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerText.text = "Tiempo Restante: " + Mathf.Round(currentTime).ToString() + "s";
        }
        else
        {
            resultText.text = "¡Tiempo agotado!";
            resultText.color = Color.red;  // Mostrar mensaje de tiempo agotado
        }
    }

    // Inicia el juego dependiendo de la edad seleccionada
    public void StartGame(int ageRange)
    {
        this.ageRange = ageRange;
        // Configura las preguntas basadas en el rango de edad
        if (ageRange == 0)  // 6-7 años: solo suma y resta
        {
            Debug.Log("Modo 6-7 años activado");
        }
        else if (ageRange == 1)  // 8-10 años: suma, resta y multiplicación
        {
            Debug.Log("Modo 8-10 años activado");
        }
    }

    // Mostrar una nueva pregunta aleatoria
    public void ShowNewQuestion()
    {
        int num1 = Random.Range(1, 20);  // Números más grandes para dificultar las restas
        int num2 = Random.Range(1, 20);

        if (ageRange == 0) // 6-7 años (sumas y restas básicas, no negativos)
        {
            if (Random.Range(0, 2) == 0)  // 50% de chance para suma o resta
            {
                questionText.text = num1 + " + " + num2;
                correctAnswer = num1 + num2;
            }
            else
            {
                // Asegurarse de que la resta no sea negativa
                if (num1 < num2) num1 = num2 + Random.Range(1, 5);  // Aseguramos que num1 siempre sea mayor
                questionText.text = num1 + " - " + num2;
                correctAnswer = num1 - num2;
            }
        }
        else if (ageRange == 1) // 8-10 años (sumas, restas y multiplicaciones)
        {
            int operation = Random.Range(0, 3); // 0: Suma, 1: Resta, 2: Multiplicación

            if (operation == 0)  // Suma
            {
                questionText.text = num1 + " + " + num2;
                correctAnswer = num1 + num2;
            }
            else if (operation == 1)  // Resta
            {
                // Asegurarse de que la resta no sea negativa
                if (num1 < num2) num1 = num2 + Random.Range(1, 5);  // Aseguramos que num1 siempre sea mayor
                questionText.text = num1 + " - " + num2;
                correctAnswer = num1 - num2;
            }
            else  // Multiplicación
            {
                num1 = Random.Range(1, 10);
                num2 = Random.Range(1, 10);
                questionText.text = num1 + " × " + num2;
                correctAnswer = num1 * num2;
            }
        }
    }

    // Verificar si la respuesta es correcta
    public void CheckAnswer()
    {
        string userAnswer = answerInput.text;

        if (userAnswer == correctAnswer.ToString())
        {
            score++;
            scoreText.text = "Puntaje: " + score.ToString();
            resultText.text = "¡Correcto!";
            resultText.color = Color.green; // Asegúrate de que se vea bien
        }
        else
        {
            resultText.text = "¡Incorrecto! La respuesta correcta es " + correctAnswer.ToString();
            resultText.color = Color.red; // Asegúrate de que se vea bien
        }

        // **Avanzar automáticamente a la siguiente pregunta**
        Invoke("ShowNextQuestion", 1f);  // Esperar 1 segundo para que el jugador vea el resultado
    }

    // Función para mostrar la siguiente pregunta
    void ShowNextQuestion()
    {
        ShowNewQuestion();   // Mostrar nueva pregunta
        answerInput.text = "";  // Limpiar la respuesta anterior
        resultText.text = "";  // Limpiar el texto de resultado
    }
}
