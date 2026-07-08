using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private int HP = 3;

    private Vector2 moveInput = Vector2.zero;

    private Rigidbody2D rb;
    public bool isGrounded;

    StrokeController SC;
    StartPosition SP;
    Cage cage;
    Gimmick g;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SC = FindAnyObjectByType<StrokeController>();
        SP = FindAnyObjectByType<StartPosition>();
        cage = FindAnyObjectByType<Cage>();
        g = FindAnyObjectByType<Gimmick>();

    }
    private void FixedUpdate()
    {
        if (SC.now_stroke)
            return;

        if (Mathf.Abs(moveInput.x) > 0)
        {
            rb.linearVelocity = new Vector2(
                moveInput.x * speed,
                rb.linearVelocity.y
            );
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            Debug.Log("Jumping");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AttackGimmick"))
        {
            Destroy(collision.gameObject);
            --HP;
            if (HP == 0)
            {
                SP.PlayerSpawn();
                Destroy(this.gameObject);
                HP = 3;
            }
        }
        if (collision.gameObject.CompareTag("Key"))
        {
            if (!GameManager.instance.hasKey)
            {
                GameManager.instance.hasKey = true;
                Destroy(collision.gameObject);
            }
              
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Arrow") || collision.gameObject.CompareTag("Line"))
        {
            foreach (ContactPoint2D point in collision.contacts)
            {
                // 上から乗った場合だけ
                if (point.normal.y > 0.5f)
                {
                    isGrounded = true;
                }
            }
        }
        if (collision.gameObject.CompareTag("Cage") && GameManager.instance.hasKey)
        {
            GameManager.instance.hasKey = false;
            cage.isOpen = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("Arrow")  || 
            collision.gameObject.CompareTag("Line"))
        {
            isGrounded = false;
        }
    }
}
