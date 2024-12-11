using System.Collections;
using System.Collections.Generic;
using System.Media;
using SuperPupSystems.Audio;
using UnityEngine;
namespace SuperPupSystems
{
public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FadeIn(AudioInfo _info)
    {
        if (_info.source == null)
            return null;

        _info.volume = 0;
        return null;
    }
}
}