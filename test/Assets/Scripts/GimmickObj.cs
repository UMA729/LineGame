using UnityEngine;

public class GimmickObj : MonoBehaviour
{
    float speed = 3f;
    float rotateSpeed = 360f;

    private Vector3 moveDirection;

    void Start()
    {
        // 生成時の右方向を保存
        moveDirection = transform.right;
    }

    void Update()
    {
        // 保存した方向へ進む
        transform.position += moveDirection * speed * Time.deltaTime;

        // 転がる
        transform.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}