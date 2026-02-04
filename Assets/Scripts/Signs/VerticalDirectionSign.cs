using UnityEngine;

public class VerticalDirectionSign : MonoBehaviour
{
    public SignManager sign;
    public GameObject player = null;
    public bool initDirectionIsDown = true;
    public bool isDown = true;
    public float upBoost = 2.0f;

    private void OnValidate()
    {
        if (initDirectionIsDown)
        {
            isDown = !sign.flipped;
        }
        else
        {
            isDown = sign.flipped;
        }
    }

    private void Start()
    {
        sign.OnFlip += OnFlip;
    }

    void OnFlip(bool flipped)
    {
        if (initDirectionIsDown)
        {
            isDown = !flipped;
        }
        else
        {
            isDown = flipped;
        }
    }

    void OnJumpStarted()
    {
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb.linearVelocityY > 0)
        {
            if (isDown)
            {
                rb.linearVelocityY = 0f;
                player.GetComponent<PlayerMovement>().isJumping = false;
            }
            else
            {
                rb.linearVelocityY *= upBoost;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(sign.playerTag) && player == null)
        {
            player = collision.gameObject;
            player.GetComponent<PlayerMovement>().OnJumpStarted += OnJumpStarted;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(sign.playerTag) && player != null)
        {
            player.GetComponent<PlayerMovement>().OnJumpStarted -= OnJumpStarted;
            player = null;
        }
    }
}
