using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles anything and everything that needs to move between scenes
/// </summary>
public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioClip[] musicList;
    [SerializeField] GameObject menu;

    public UnityEvent OnSceneChange; //called when changing scenes

    private void Awake() //Set this script to not destry itself and create the static instance
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }
}
