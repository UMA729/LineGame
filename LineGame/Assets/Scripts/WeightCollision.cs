using UnityEngine;

public class WeightCollision : MonoBehaviour
{
    public StrokeController.WeightData weight_data;
    public bool isIgnoreTag = false;

    private void OnCollisionStay2D(Collision2D collision)
    {
        // LineOut�͖���
        //if (collision.gameObject.layer == LayerMask.NameToLayer("LineOut"))
        //    return;

        weight_data.isDestroying = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        weight_data.isDestroying = false;
    }
}
