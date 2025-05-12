
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnAreaTopLeft;
    public Transform spawnAreaBottomRight;
    public Sprite[] ballSprites;

    private List<GameObject> balls = new List<GameObject>();
    private int currentRound = 1;

    public void SpawnInitialBalls()
    {
        currentRound = 1;
        StartCoroutine(SpawnAndLaunch());
    }

    public void NextRound(bool repeatSameRound = false)
    {
        foreach (var ball in balls)
        {
            Destroy(ball);
        }

        if (!repeatSameRound && currentRound < 5)
        {
            currentRound++;
        }

        StartCoroutine(SpawnAndLaunch());
    }

    private IEnumerator SpawnAndLaunch()
    {
        balls.Clear();

        int ballCount = Mathf.Clamp(currentRound + 1, 2, 5);

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
            rb.gravityScale = 1f;
            rb.drag = 0.1f;

            balls.Add(ball);
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < balls.Count; i++)
        {
            float jump = Random.Range(6f, 10f); // Rebote más moderado

            Ball ballScript = balls[i].GetComponent<Ball>();
            ballScript.ApplyInitialJump(jump);
            ballScript.SetJumpForce(jump);
        }

        yield return new WaitForSeconds(1.5f); // 🔁 deja que empiecen a subir

        GameManager3.Instance.SetActiveBalls(balls);

    }

    public List<GameObject> GetCurrentBalls()
    {
        return balls;
    }
}
