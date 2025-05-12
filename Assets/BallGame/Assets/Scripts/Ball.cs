
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    public int ballNumber;
    public float jumpForce = 9f;

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
        jumpForce = force;
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isStopped) return;
        if (collision.collider.CompareTag("Floor"))
        {
            rb.velocity = new Vector2(0f, jumpForce); // Rebote continuo
        }
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
