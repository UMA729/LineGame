using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    Pause P;

    private void Start()
    {
        P = FindAnyObjectByType<Pause>();
    }
    public void OnClick(string sceneName)
    {
        Pause pause = FindAnyObjectByType<Pause>();

        if (GameManager.instance != null &&
            GameManager.instance.isPaused &&
            pause != null)
        {
            pause.EndPause();
        }
         Debug.Log("GameManager : " + GameManager.instance);
    Debug.Log("Paused : " + GameManager.instance.isPaused);
    Debug.Log("Pause : " + P);

        SceneManager.LoadScene(sceneName);
    }
    public void ClickRestart()
    {
        if (GameManager.instance.isPaused && P != null)
        {
            P.EndPause();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
