using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] musics;
    public static int currentSong = 0;

    private void Update()
    {
        if(currentSong == 0)
        {
            source.PlayOneShot(musics[0]);
        }
        else
        {
            source.PlayOneShot(musics[1]);
        }
    }

}
