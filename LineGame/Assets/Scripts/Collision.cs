using UnityEngine;

public class Collision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("入ってはいる");
        if (collision.gameObject.CompareTag("AttackGimmick"))
        {
        //Debug.Log("入る");

            Destroy(collision.gameObject);
        }
    }
}