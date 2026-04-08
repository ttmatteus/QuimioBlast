using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float velocidade = 7f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(x * velocidade, y * velocidade);
    }
}