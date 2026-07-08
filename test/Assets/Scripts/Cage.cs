using UnityEngine;

public class Cage : MonoBehaviour
{

    public bool isOpen = false;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float limmiter = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        limmiter += transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            if (transform.position.y < limmiter)
                transform.position += transform.up * speed * Time.deltaTime;
        }
    }
}
