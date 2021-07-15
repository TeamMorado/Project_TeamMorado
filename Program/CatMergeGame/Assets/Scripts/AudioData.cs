using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "AudioData", order = 2)]
public class AudioData : ScriptableObject
{
    public string sz_Key;
    public float f_AudioVolume;
    public AudioClip audioClip;
}
