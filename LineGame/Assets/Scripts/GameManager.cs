using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public int stageNum = 0;
    public int continueNum = 0;
    public bool isPaused = false;
    public bool hasKey = false;

    [SerializeField]GameObject PauseCanvas;

    public void Awake()
    {

        Application.targetFrameRate = 60; // ‰Šúó‘Ô‚Í-1‚É‚È‚Á‚Ä‚¢‚é

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PauseCanvas != null)
        {
            PauseCanvas.gameObject.SetActive(true);
            GameManager.instance.isPaused = true;
        }
    }
}
