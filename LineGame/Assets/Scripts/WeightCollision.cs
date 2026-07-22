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

        bool ignoreTag = false;

        foreach (string tag in weight_data.tag)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                ignoreTag = true;
                break;
            }
        }

        // �����^�O�ȊO�ɐG��Ă���Ԃ����k��
        weight_data.isReducing = !ignoreTag;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("LineOut"))
        {
            weight_data.isInfrate = false;
        }
    }
}
