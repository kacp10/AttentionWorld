using UnityEngine;

public class Ball : MonoBehaviour
{
    public int ballNumber;
    public float jumpForce;

    private float maxHeight = 0f;
    private bool isStopped = false;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isStopped && rb.velocity.y > 0.1f && transform.position.y > maxHeight)
        {
            maxHeight = transform.position.y;
        }
    }

    public void ApplyInitialJump(float force)
    {
        rb.velocity = new Vector2(0f, force); // Salto inicial con velocidad fija
    }

    public float GetMaxHeight()
    {
        return maxHeight;
    }

    public void Stop()
    {
        isStopped = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        rb.isKinematic = true;
    }
    public void SetJumpForce(float force)
    {
        jumpForce = force;
    }

    public void ResetBall()
    {
        isStopped = false;
        rb.isKinematic = false;
        rb.gravityScale = 1;
        maxHeight = 0f;
    }
}
