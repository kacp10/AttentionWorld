using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float jumpForce = 5f;

    public int ballNumber; // 👈 Esto es lo nuevo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Jump();
    }

    public void Jump()
    {
        float randomForce = Random.Range(jumpForce - 1f, jumpForce + 1f);
        rb.velocity = Vector2.up * randomForce;
    }

    public float GetHeight()
    {
        return transform.position.y;
    }
}
