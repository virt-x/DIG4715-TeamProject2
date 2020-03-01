using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;

    public GameObject pauseUI, gameUI;
    public PlayerMovement player;


    void Update()
    {
       if (Input.GetKeyDown(KeyCode.P))
        {
            if (paused)
            {
                Resume();
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Pause();
                Cursor.lockState = CursorLockMode.None;
            }
        } 
    }

    public void Resume()
    {
        gameUI.SetActive(true);
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    void Pause()
    {
        gameUI.SetActive(false);
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        Debug.Log("Loading menu....");
    }
}
