using System;
using Dacodelaac.Core;
using Dacodelaac.Events;
using UnityEngine;

namespace Dev.Scripts.Controller
{
    [CreateAssetMenu(menuName = "Sound/SoundManager")]
    public class SoundManager : BaseSO
    {
        [SerializeField] public AudioSFXEvent playAudioEvent;
        [SerializeField] public PlayAudioEvent musicAudioEvent;
        [SerializeField] public AudioClip bgIdle;
        [SerializeField] public AudioClip bgPuzzle;
        [SerializeField] public AudioSfx moneyReceivedIdle;
        [SerializeField] public AudioSfx pinPulled;
        [SerializeField] public AudioSfx pinCollision;
        [SerializeField] public AudioSfx puzzleWin;
        [SerializeField] public AudioSfx mainWin;
        [SerializeField] public AudioSfx puzzleLose;
        [SerializeField] public AudioSfx mainLose;
        [SerializeField] public AudioSfx shipBall;
        [SerializeField] public AudioSfx bomb;
        [SerializeField] public AudioSfx moneyReceived;
        [SerializeField] public AudioSfx mainHappy;
        [SerializeField] public AudioSfx mainCry;
        [SerializeField] public AudioSfx mainAngry;
        [SerializeField] public AudioSfx mainPhonePicture;
        [SerializeField] public AudioSfx mainPhoneKiss;
        [SerializeField] public AudioSfx mainX10Reward;
        [SerializeField] public AudioSfx upgrade;
        [SerializeField] public AudioSfx newModel;
        [SerializeField] public AudioSfx mainSing;
        [SerializeField] public AudioSfx mainScare;
        [SerializeField] public AudioSfx loseBomb;
        [SerializeField] public AudioSfx mainMakeup;
        [SerializeField] public AudioSfx mainUgly;
        [SerializeField] public AudioSfx mainShowed;

        public void PlayAudio(AudioSfx audioClip)
        {
            playAudioEvent.Raise(audioClip);
        }

        public void PlayMusic(AudioClip audioClip)
        {
            musicAudioEvent.Raise(audioClip);
        } 
    }
    
    [Serializable]
    public class AudioSfx
    {
        public AudioClip audio;
        [Range(0f, 2f)] public float volume = 1f;
    }
}
