using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ResultSceneManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text scoreText;
    public TMP_Text resultCountText;
    public TMP_Text feedbackText;
    public Image brainImage;
    public Image iconImage;

    [Header("Brain Sprites")]
    public Sprite emptyBrain, quarterBrain, halfBrain, threeQuarterBrain, fullBrain;

    [Header("Area Icons")]
    public Sprite attentionIcon, memoryIcon, logicIcon, mathIcon;

    /*──────────────────────────────────────────────────────────*/
    void Start()
    {
        string area = PlayerPrefs.GetString("CognitiveArea", "atencion");
        string gameName = PlayerPrefs.GetString("GameName", "Juego");
        int index = PlayerPrefs.GetInt("CurrentGameIndex", 0);

        int correct = PlayerPrefs.GetInt("CorrectCount", -1);
        int incorrect = PlayerPrefs.GetInt("IncorrectCount", -1);

        int score;
        bool tieneAciertos = (correct >= 0 || incorrect >= 0);

        if (tieneAciertos)
        {
            correct = Mathf.Max(correct, 0);
            incorrect = Mathf.Max(incorrect, 0);
            score = Mathf.Clamp(correct * 80 - incorrect * 20, 0, 1000);
        }
        else
        {
            score = PlayerPrefs.GetInt("FinalScore", 0);
        }

        /*── Guarda todo para SummaryScene ─────────────────────*/
        PlayerPrefs.SetInt($"Score{index}", score);
        PlayerPrefs.SetString($"GameName{index}", gameName);
        PlayerPrefs.SetString($"CognitiveArea{index}", area);
        PlayerPrefs.SetInt($"HasCI{index}", tieneAciertos ? 1 : 0);
        if (tieneAciertos)
        {
            PlayerPrefs.SetInt($"Correct{index}", correct);
            PlayerPrefs.SetInt($"Incorrect{index}", incorrect);
        }

        /*── UI ────────────────────────────────────────────────*/
        resultCountText.gameObject.SetActive(true);
        resultCountText.text = tieneAciertos
            ? $"Correctas: {correct}\nIncorrectas: {incorrect}"
            : score >= 900 ? "¡Completado al 100%! 🎯"
            : score >= 400 ? "¡Parcialmente completado! 🧩"
                           : "No se logró completar. ¡Inténtalo de nuevo! ⚠️";

        scoreText.text = score.ToString();
        iconImage.sprite = GetIcon(area);
        UpdateBrain(score);
        UpdateTexts(score);
    }

    /*──────────────────── helpers UI ────────────────────────*/
    void UpdateBrain(int s)
    {
        brainImage.sprite = s >= 900 ? fullBrain
                        : s >= 700 ? threeQuarterBrain
                        : s >= 500 ? halfBrain
                        : s >= 300 ? quarterBrain
                                 : emptyBrain;
    }
    void UpdateTexts(int s)
    {
        titleText.text = s >= 900 ? "¡Impresionante!"
                       : s >= 700 ? "¡Muy bien!"
                       : s >= 500 ? "Bastante bien"
                                : "¡Tú puedes mejorar!";

        feedbackText.text = s >= 900 ? "Tu concentración fue excelente, sigue así."
                       : s >= 700 ? "Buen rendimiento, intenta llegar más alto."
                       : s >= 500 ? "Vas bien, intenta equivocarte menos."
                                : "No te preocupes, cada intento te hace mejor.";
    }
    Sprite GetIcon(string a)
    {
        switch (a.ToLower())
        {
            case "memoria": return memoryIcon;
            case "logica": return logicIcon;
            case "calculo": return mathIcon;
            default: return attentionIcon;
        }
    }

    /*── Timer para cambiar de escena ─────────────────────────*/
    void Update()
    {
        if (Time.timeSinceLevelLoad < 5f) return;

        int next = PlayerPrefs.GetInt("CurrentGameIndex", 0) + 1;

        /* limpia PlayerPrefs del juego actual */
        PlayerPrefs.DeleteKey("CorrectCount");
        PlayerPrefs.DeleteKey("IncorrectCount");
        PlayerPrefs.DeleteKey("FinalScore");
        PlayerPrefs.DeleteKey("CognitiveArea");
        PlayerPrefs.DeleteKey("GameName");

        if (next < GameSessionData.Instance.Assignments.Count)
        {
            PlayerPrefs.SetInt("CurrentGameIndex", next);
            UnityEngine.SceneManagement.SceneManager.LoadScene("BriefScene");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SummaryScene");
        }
    }
}
