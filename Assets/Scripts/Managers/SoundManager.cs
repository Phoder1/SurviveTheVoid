using System.Collections.Generic;
using UnityEngine;
public enum Sound { BackGround, Consume, HarvestTree, HarvestStone, Walk, UIopen, UiClose }
public enum VolumeGroup { Music, Sounds }
public class SoundManager : MonoSingleton<SoundManager>
{
    Dictionary<Sound, AudioSource> audioDic;

    [SerializeField] AudioSourceClass[] audioSources;
    private float musicVolume;
    private float soundsVolume;

    public override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    } 
    public override void Init() {
        //musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
        //soundsVolume = PlayerPrefs.GetFloat("SoundsVolume", 1);
        audioDic = new Dictionary<Sound, AudioSource>();
        foreach (AudioSourceClass audioSource in audioSources) {
            audioDic.Add(audioSource.sound, audioSource.audioSource);
            audioSource.defaultVolume = audioSource.audioSource.volume;
        }
        PlaySoundLooped(Sound.BackGround);
        SetVolumeGroup(VolumeGroup.Music, 0.5f);
        SetVolumeGroup(VolumeGroup.Sounds, 0.5f);
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.O)) {
            PlaySound(Sound.UIopen);
        }
    }

    // Update is called once per frame
    public void PlaySound(Sound sound) {
        audioDic[sound].loop = false;
        audioDic[sound].Play();
    }
    public void PlaySoundLooped(Sound sound) {
        audioDic[sound].loop = true;
        audioDic[sound].Play();
    }
    public void DisableLooping(Sound sound) => audioDic[sound].loop = false;
    public void StopSound(Sound sound) {
        audioDic[sound].Stop();


    }
    public void PauseSound(Sound sound) {
        audioDic[sound].Pause();



    }
    public void RestartSound(Sound sound) {
        audioDic[sound].Stop();
        audioDic[sound].Play();
    }
    public void DelayPlay(Sound sound, float delay) {
        audioDic[sound].PlayDelayed(delay);

    }
    public void SetVolumeGroup(VolumeGroup volumeGroup, float volume) {
        switch (volumeGroup) {
            case VolumeGroup.Music:
                musicVolume = volume;
                //PlayerPrefs.SetFloat("MusicVolume", musicVolume);
                break;
            case VolumeGroup.Sounds:
                soundsVolume = volume;
                //PlayerPrefs.SetFloat("SoundsVolume", soundsVolume);
                break;
        }
        foreach (AudioSourceClass audioSource in audioSources)
            if (audioSource.VolumeGroup == volumeGroup)
                audioSource.audioSource.volume = volume * audioSource.defaultVolume;
    }
    public float GetVolumeGroup(VolumeGroup volumeGroup) {
        switch (volumeGroup) {
            case VolumeGroup.Music:
                return musicVolume;
            case VolumeGroup.Sounds:
                return soundsVolume;
            default:
                throw new System.NotImplementedException();
        }
    }


    private void SetVolume(Sound sound, float volume) => audioDic[sound].volume = volume;
    private float GetVolume(Sound sound) => audioDic[sound].volume;

    [System.Serializable]
    class AudioSourceClass
    {
        public AudioSource audioSource;
        public Sound sound;
        public VolumeGroup VolumeGroup;
        [HideInInspector]
        public float defaultVolume;
    }

}
