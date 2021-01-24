using System.Collections.Generic;
using UnityEngine;
public enum Sounds { BackGround, Consume, HarvestTree, HarvestStone, Walk, UIopen, UiClose }
public class SoundManager : MonoSingleton<SoundManager>
{
    Dictionary<Sounds, AudioSource> audioDic;

    [SerializeField] AudioSource[] audioinitList;
    [SerializeField] Sounds[] sounds;
    public override void Init() {
        audioDic = new Dictionary<Sounds, AudioSource>();
        for (int i = 0; i < sounds.Length; i++) {
            audioDic.Add(sounds[i], audioinitList[i]);

        }
        PlaySoundLooped(Sounds.BackGround);
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.O)) {
            PlaySound(Sounds.UIopen);
        }
    }

    // Update is called once per frame
    public void PlaySound(Sounds sound) {
        audioDic[sound].loop = false;
        audioDic[sound].Play();
    }
    public void PlaySoundLooped(Sounds sound) {
        audioDic[sound].loop = true;
        audioDic[sound].Play();
    }
    public void DisableLooping(Sounds sound) => audioDic[sound].loop = false;
    public void StopSound(Sounds sound) {
        audioDic[sound].Stop();


    }
    public void PauseSound(Sounds sound) {
        audioDic[sound].Pause();



    }
    public void RestartSound(Sounds sound) {
        audioDic[sound].Stop();
        audioDic[sound].Play();
    }
    public void DelayPlay(Sounds sound, float delay) {
        audioDic[sound].PlayDelayed(delay);

    }

}
