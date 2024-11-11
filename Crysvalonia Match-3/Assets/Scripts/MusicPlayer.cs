using UnityEngine;

/// <summary>
/// This script will handle the music. In the future, it will hopefully also handle any sound effects.
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    //TODO: Create a system for controlling audio level
    //TODO: Create a system for handling sound effects once I add those in

    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioClip[] musicList;

    GameController controllerInstance;

    int currentMusicClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controllerInstance = GameController.Instance; //since Gamecontroller creates the instance on Awake it should exist at this point

        if (controllerInstance == null)
            Debug.LogError("MusicPlayer: No ControllerInstance found!");

        if(musicPlayer == null)
        {
            musicPlayer = FindObjectsByType<AudioSource>(FindObjectsSortMode.InstanceID)[0]; //music player should always be the only audio source in start scene where this is initialized
        }

        currentMusicClip = 0;

        musicPlayer.clip = musicList[currentMusicClip];
        musicPlayer.Play();
        musicPlayer.loop = true;

        controllerInstance.OnSceneChange.AddListener(onSceneChange);
    }

    private void OnDestroy()
    {
        //not removing events can cause memory leaks, so double ensure that if this gets destroyed no listener is left
        if (controllerInstance != null)
            controllerInstance.OnSceneChange.RemoveListener(onSceneChange);
    }

    public void ToggleMusicPause(bool isPlaying)
    {
        if (isPlaying)
            musicPlayer.Play();
        else
            musicPlayer.Pause();
    }

    void onSceneChange()
    {
        Debug.Log("MusicPlayer: onSceneChange invoked!");
        musicPlayer.Pause(); //Making sure there is no two audio files playing at the same time

        currentMusicClip++;
        if(currentMusicClip >= musicList.Length) //for now this works since there is only two soundtracks, but if I ever add any more I'll need to figure out something else
        {
            currentMusicClip = 0;
        }

        musicPlayer.clip = musicList[currentMusicClip];
        musicPlayer.Play();
    }
}
