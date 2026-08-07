using UnityEngine;

public class Pause : MonoBehaviour
{

    private void Awake()
{
    DontDestroyOnLoad(gameObject);
}
    // Update is called once per frame

    private void Update()
    {
        if (GameManager.instance.isPaused)
        StartPause();
    }
    public void StartPause()
    {
        Time.timeScale = 0;
    }

    public void EndPause()
    {
        if (GameManager.instance.isPaused)
        {
            Time.timeScale = 1;
            GameManager.instance.isPaused = false;
            gameObject.SetActive(false);
        }
    }
}
