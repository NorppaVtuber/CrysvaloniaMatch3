using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Canvas menuCanvas;
    public Canvas resetCanvas;

    // Start is called before the first frame update
    void Start()
    {
        resetCanvas.enabled = false;
        menuCanvas.enabled = false;
    }

    public void MenuButton()
    {
        menuCanvas.enabled = true;
    }

    public void MenuBack()
    {
        menuCanvas.enabled = false;
    }

    public void MenuCredits()
    {

    }

    public void MenuGuide()
    {

    }

    public void MenuWebsite()
    {

    }

    public void ResetButton()
    {
        resetCanvas.enabled = true;
    }

    public void ResetYesButton()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ResetNoButton()
    {
        resetCanvas.enabled = false;
    }

    public void StarGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
