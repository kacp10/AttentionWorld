using UnityEngine;

public class QuestionGenerator : MonoBehaviour
{
    public static QuestionGenerator Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public string GenerateQuestion(int ageRange)
    {
        int num1 = Random.Range(1, 10);
        int num2 = Random.Range(1, 10);
        string question = "";

        if (ageRange == 0) // 6-7 años: solo suma y resta
        {
            int operation = Random.Range(0, 2); // 0 para suma, 1 para resta
            if (operation == 0)
                question = $"{num1} + {num2} = ?";
            else
                question = $"{num1} - {num2} = ?";
        }
        else if (ageRange == 1) // 8-10 años: suma, resta y multiplicación
        {
            int operation = Random.Range(0, 3); // 0 para suma, 1 para resta, 2 para multiplicación
            if (operation == 0)
                question = $"{num1} + {num2} = ?";
            else if (operation == 1)
                question = $"{num1} - {num2} = ?";
            else
                question = $"{num1} x {num2} = ?";
        }

        return question;
    }
}
