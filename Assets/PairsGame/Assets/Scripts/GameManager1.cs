using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance;

    [Header("Cartas")]
    public GameObject cardPrefab;
    public Sprite[] cardFaces; // del 1 al 6
    public Sprite cardBack;    // imagen de reverso (candado)

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text resultText;

    private List<Card> flippedCards = new List<Card>();
    private int score = 0;
    private int matchedPairs = 0;
    private float timeLeft = 45f;
    private bool gameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GenerateBoard();
    }

    void Update()
    {
        if (gameOver) return;

        timeLeft -= Time.deltaTime;
        timerText.text = Mathf.Max(0, Mathf.FloorToInt(timeLeft)).ToString("00");

        if (timeLeft <= 0 && !gameOver)
        {
            EndGame(false);
        }
    }

    void GenerateBoard()
    {
        List<int> types = new List<int>();
        for (int i = 1; i <= 6; i++) { types.Add(i); types.Add(i); } // 6 pares

        // Mezclar
        for (int i = 0; i < types.Count; i++)
        {
            int rand = Random.Range(i, types.Count);
            int temp = types[i];
            types[i] = types[rand];
            types[rand] = temp;
        }

        float startX = -3.5f;
        float startY = 2.5f;

        int index = 0;
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                Vector2 pos = new Vector2(startX + col * 2.2f, startY - row * 2.2f);
                GameObject cardObj = Instantiate(cardPrefab, pos, Quaternion.identity);
                Card card = cardObj.GetComponent<Card>();
                card.Init(types[index], cardFaces[types[index] - 1]);
                card.back.sprite = cardBack;
                index++;
            }
        }

        UpdateUI();
    }

    public void OnCardFlipped(Card card)
    {
        if (flippedCards.Contains(card) || card.IsMatched()) return;

        flippedCards.Add(card);

        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);

        if (flippedCards[0].cardType == flippedCards[1].cardType)
        {
            flippedCards[0].MarkAsMatched();
            flippedCards[1].MarkAsMatched();
            matchedPairs++;
            score += 15;
        }
        else
        {
            flippedCards[0].Hide();
            flippedCards[1].Hide();
            score = Mathf.Max(0, score - 5);
        }

        flippedCards.Clear();
        UpdateUI();

        if (matchedPairs == 6)
        {
            EndGame(true);
        }
    }

    void EndGame(bool won)
    {
        gameOver = true;
        resultText.text = won ? "¡GANASTE!" : "PERDISTE";
        resultText.color = won ? Color.green : Color.red;
    }

    void UpdateUI()
    {
        scoreText.text = score.ToString("00");
    }

    public bool CanFlipCard(Card card)
    {
        return !gameOver && flippedCards.Count < 2;
    }
}
