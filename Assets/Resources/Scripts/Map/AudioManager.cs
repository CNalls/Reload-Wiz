using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//singleton for audio manager
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; //this is the singleton instance
    public AudioSource backgroundMusic;
    public AudioClip[] soundEffects;


    void Awake()
    {
        if (Instance == null) // this is for checking if the audio manager is already running, therefore were wanting it to be null 
        {
            Instance = this; //if not null set the instance to the audio manager and do not destroy said scene :)
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject); //if an instance that isnt this one is alr running destoy it
        }
    }

    public void PlaySoundEffect(int index) //not even gonna lie I looked up how to do this part for some reason int index goes here but no one will explain why
    {
        if (index < soundEffects.Length && soundEffects[index] != null) //this is auto filled dont even know if it works
        {
            AudioSource.PlayClipAtPoint(soundEffects[index], transform.position);
        }
    }
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying) // if the background music isnt null and isnt playing, play it
        {
            backgroundMusic.Play();
        }
        //add two different mixers to fade in and out audios that change
    }

    public void StopBackgroundMusic()
    {
        if (backgroundMusic != null && backgroundMusic.isPlaying) // if background music isnt null and is playing, stop it
        {
            backgroundMusic.Stop();
        }
    }
}
