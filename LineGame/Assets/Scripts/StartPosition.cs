using UnityEngine;

public class StartPosition : MonoBehaviour
{

    [SerializeField] private GameObject player_pre;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerSpawn();
    }

    public void PlayerSpawn()
    {
        Instantiate(player_pre, transform.position, transform.rotation);
    }
}
