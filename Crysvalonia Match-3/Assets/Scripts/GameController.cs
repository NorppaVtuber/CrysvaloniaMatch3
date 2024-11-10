using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles anything and everything that needs to move between scenes
/// </summary>
public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] GameObject menu;

    public UnityEvent OnSceneChange; //called when changing scenes

    private void Awake() //Set this script to not destry itself and create the static instance
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    private void Start()
    {
        //menu.SetActive(false);
    }

    private void OnDestroy()
    {
        OnSceneChange.RemoveAllListeners(); //when the instance is destroyed, make sure no event listeners are left
    }

    public void ChangeScenes()
    {
        Scene current = SceneManager.GetActiveScene();

        if(current.buildIndex == 0) //start scene
        {
            SceneManager.LoadScene("GameScene");
        }
        else if(current.buildIndex == 1) //game scene
        {
            SceneManager.LoadScene("StartScene");
        }
        else
        {
            Debug.LogError("Scene not found!");
            return; //don't try change scenes if you don't find one!
        }

        OnSceneChange.Invoke(); //Scene changed correctly, call the onSceneChange event so other scripts can handle what they need to handle

        //menu.SetActive(false);
    }

    //HACK: Temp code until I can refactor the entire menu system
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ChangeScenes();
    }
}
