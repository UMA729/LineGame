using UnityEngine;

public class PlayerPivot : MonoBehaviour
{
    PlayerController p_inst;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        p_inst = FindAnyObjectByType<PlayerController>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            p_inst.isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            p_inst.isGrounded = false;
        }
    }
}
