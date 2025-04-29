using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public int ballCount = 3;
    public Transform spawnAreaTopLeft;
    public Transform spawnAreaBottomRight;
    public Sprite[] ballSprites;

    private List<GameObject> balls = new List<GameObject>();

    public void SpawnInitialBalls()
    {
        StartCoroutine(SpawnAndLaunch());
    }

    public void NextRound(bool repeatSameRound = false)
    {
        foreach (var ball in balls)
        {
            Destroy(ball);
        }

        if (!repeatSameRound && ballCount < 10)
        {
            ballCount++; // Aumenta dificultad si acierta
        }

        StartCoroutine(SpawnAndLaunch());
    }

    private IEnumerator SpawnAndLaunch()
    {
        balls.Clear();

        // Separación horizontal
        float totalWidth = Mathf.Abs(spawnAreaBottomRight.position.x - spawnAreaTopLeft.position.x);
        float spacing = totalWidth / (ballCount + 1);
        float startX = spawnAreaTopLeft.position.x + spacing;
        float posY = (spawnAreaTopLeft.position.y + spawnAreaBottomRight.position.y) / 2f;

        for (int i = 0; i < ballCount; i++)
        {
            float posX = startX + spacing * i;
            Vector2 spawnPos = new Vector2(posX, posY);

            GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
            Ball ballScript = ball.GetComponent<Ball>();
            ballScript.ballNumber = i + 1;

            if (i < ballSprites.Length)
                ball.GetComponent<SpriteRenderer>().sprite = ballSprites[i];

            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            rb.gravityScale = 1;

            balls.Add(ball);
        }

        yield return new WaitForSeconds(1f); // dejar que se vean antes de rebotar

        int index = 0;
        foreach (var ball in balls)
        {
            float randomJump = Random.Range(5f, 7.5f);
            Ball ballScript = ball.GetComponent<Ball>();
            ballScript.ApplyInitialJump(randomJump);
            ballScript.SetJumpForce(randomJump);
            ballScript.ballNumber = index + 1;
            index++;
        }

        // ✅ Aquí se activa después del impulso
        GameManager3.Instance.SetActiveBalls(balls);
    }

    public List<GameObject> GetCurrentBalls()
    {
        return balls;
    }
}
