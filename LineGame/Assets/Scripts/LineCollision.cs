using UnityEngine;

public class LineCollision : MonoBehaviour
{
    public StrokeController.LineData line_data;
    Gimmick gimmick;

    private void Start()
    {
    }
    private void Update()
    {
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("�Ԃ�����");
        
        if (collision.gameObject.CompareTag("AttackGimmick"))
        {
            line_data.life--;
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Press"))
        {
            Gimmick gimmick = collision.GetComponentInParent<Gimmick>();

            if (gimmick != null && gimmick.isPressing)
            {
                // ����
                if (line_data.totalLength > 2.0f)
                {
                    gimmick.isPressing = false;
                }
                else if (line_data.totalLength > 1.2f)
                {
                    line_data.life -= line_data.life;
                    gimmick.isPressing = false;
                }
                else
                {
                    line_data.life -= line_data.life;
                }
            }
        }
    }
}
