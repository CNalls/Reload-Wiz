using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SuperPupSystems.Audio;

namespace SuperPupSystems.Audio
{
[CreateAssetMenu(fileName = "AudioSet", menuName = "Data/AudioSet", order = 1)]
public class AudioSet : ScriptableObject
{
   public List<AudioInfo> SFX;
   public List<AudioInfo> Music;
   public bool shuffleMusic = true;
}
}