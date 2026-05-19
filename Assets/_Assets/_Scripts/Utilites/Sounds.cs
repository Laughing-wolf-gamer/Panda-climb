using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Audio",menuName = "Gamer Wolf Utilities/Audios")]
public class Sounds : ScriptableObject {
    public enum SoundType{
        BGM,
        PlaneSound,
        distructionSound1,
        RingCrossed,
        click,

    
    }
    
    public SoundType soundType;
    public AudioClip audioClip;
    public bool isLooping;
    public bool playOnAwake;
    public bool playonShot;
    [Range(0f,1f)]
    public float volumeSlider = 1f;
    [Range(-3f,3f)]
    public float pitchSlider = 1f;
    public bool isMute;
    public bool isSfx;
    

    [HideInInspector]
    public AudioSource source;
    
    
}
// namespace GamerWolf.Utils{
    
// }
