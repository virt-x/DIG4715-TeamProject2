using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
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
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //StartCoroutine(Begin());
    }

    IEnumerator Begin()
    {
        fadePanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        for (int i = 4; i > 0; i--)
        {
            fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.b, fadePanel.color.g, fadePanel.color.a + 0.25f);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield break;
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}