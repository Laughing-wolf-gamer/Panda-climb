using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace GamerWolf.Utils{
    
    
    
    // [System.Serializable]
    
    
    public class AudioManager : MonoBehaviour{
        public static AudioManager current{get;private set;}
        

        [SerializeField] private Sounds[] sounds;
        [SerializeField] private GameDataSO gameDataSO;
        private List<AudioSource> sfxSourceList;
        

        
        private void Awake(){
            if(current == null){
                current = this;
            }
            else{
                Destroy(current.gameObject);
                Debug.Log($"Another Audio Manager is Found And Destroyed");
            }
            DontDestroyOnLoad(current.gameObject);
        }
        private void Start(){
            sfxSourceList = new List<AudioSource>();
            foreach(Sounds s in sounds){
                s.source = gameObject.AddComponent<AudioSource>();
                if(s.isSfx){
                    sfxSourceList.Add(s.source);
                }
                
                s.source.loop = s.isLooping;
                s.source.pitch = s.pitchSlider;
                s.source.volume = s.volumeSlider;
                s.source.playOnAwake = s.playOnAwake;
                s.source.clip = s.audioClip;
                s.source.mute = s.isMute;
            }
        }
        private void Update(){
            MuteMusic(!gameDataSO.GetMusicState());
            MuteSFX(!gameDataSO.GetSoundState());
        }

        public void MuteMusic(bool Mute){
            
            for (int i = 0; i < sounds.Length; i++){
                if(sounds[i].soundType == Sounds.SoundType.BGM){
                    sounds[i].source.mute = Mute;
                }
                
            }
            
        }
        public void MuteSFX(bool mute){
            // Debug.Log("Sound is " + mute);
            for (int i = 0; i < sfxSourceList.Count; i++){
                sfxSourceList[i].mute = mute;
            }
        }
        
        
        public void PlayMusic(Sounds.SoundType soundType){
            if(AudioManager.current != null){
                Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
                if(s.soundType == soundType){
                    if(!s.isSfx){
                        s.source.Play();
                    }
                }
            }
        }
        public void PauseMusic(Sounds.SoundType soundType){
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            if(s != null){
                if(s.source.clip != null){
                    s.source.Pause();
                }
            }
        }
        public void PlayOneShotMusic(Sounds.SoundType soundType){
            if(AudioManager.current != null){
                Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
                if(s != null){
                    if(s.source.clip != null){
                        s.source.PlayOneShot(s.audioClip);
                    }
                }
            }
        }
        public void StopAudio(Sounds.SoundType soundType){
            Sounds s = Array.Find(sounds ,s => s.soundType == soundType);
            if(s != null){
                if(s.source.clip != null){
                    s.source.Stop();
                }
                
            }
        }
        
    }

}