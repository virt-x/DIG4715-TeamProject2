using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public UnityEngine.UI.Image fadePanel;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    void Update()
    {
        //Quit when the escape key is pressed
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
    public void LoadMenu()
    {
        StartCoroutine(ToMenu());
    }

    IEnumerator ToMenu()
    {
        //fadePanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
        yield break;
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
