using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingAudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Play a sound effect
        AudioManager.Instance.PlaySoundEffect(0);
    }

    // Update is called once per frame
    void Update()
    {
        // Play or stop background music
        AudioManager.Instance.PlayBackgroundMusic();
        AudioManager.Instance.StopBackgroundMusic();
    }
}
