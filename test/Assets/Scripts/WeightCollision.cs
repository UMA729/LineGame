using UnityEngine;

public class WeightCollision : MonoBehaviour
{
    public StrokeController.WeightData weight_data;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("LineOut"))
        {
            return;
        }
        else if(!collision.gameObject.CompareTag("Line"))
            weight_data.isReducing = true;
    }
}
