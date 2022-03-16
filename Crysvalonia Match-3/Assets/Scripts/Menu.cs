using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Canvas menuCanvas;
    public Canvas resetCanvas;
    public Canvas creditsCanvas;

    public static bool isPaused = false;
    private bool creditsIsOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        resetCanvas.enabled = false;
        menuCanvas.enabled = false;
        creditsCanvas.enabled = false;
    }

    public void MenuButton()
    {
        menuCanvas.enabled = true;
        isPaused = true;
    }

    public void MenuBack()
    {
        if(!creditsIsOpen)
        {
            menuCanvas.enabled = false;
            isPaused = false;
        }
    }

    public void MenuCredits()
    {
        creditsCanvas.enabled = true;
        if (isPaused)
        {
            menuCanvas.enabled = false;
        }
        creditsIsOpen = true;
    }

    public void CreditsBack()
    {
        creditsCanvas.enabled = false;
        if(isPaused)
        {
            menuCanvas.enabled = true;
        }
        creditsIsOpen = false;
    }

    public void MenuGuide()
    {

    }

    public void MenuWebsite()
    {

    }

    public void ResetButton()
    {
        if (!creditsIsOpen)
        {
            resetCanvas.enabled = true;
            isPaused = true;
        }
    }

    public void ResetYesButton()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        isPaused = false;
    }

    public void ResetNoButton()
    {
        resetCanvas.enabled = false;
        isPaused = false;
    }

    public void StarGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
