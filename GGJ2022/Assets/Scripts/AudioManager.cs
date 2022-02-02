using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusicAudioSource;
    public AudioSource soundEffectAudioSource;

    [Header("Audio Clips")]
    public AudioClip titleScreenMusic;
    public AudioClip inGameMusic;
    [Space]
    public AudioClip insertCoinSFX;
    public AudioClip startLevelSFX;
    public AudioClip grabCollectibleSFX;
    public AudioClip grabKeySFX;
    public AudioClip grabDiamondSFX;
    public AudioClip grabCoalSFX;
    public AudioClip grabExtraLifeSFX;
    public AudioClip openDoorSFX;
    public AudioClip loseLifeSFX;
    public AudioClip finishLevelSFX;
    public AudioClip dayNightSwitchSFX;

    private void Awake()
    {
        instance = this;
    }

    public void StopMusic()
    {
        backgroundMusicAudioSource.Stop();
    }

    public void PlayInGameMusic()
    {
        backgroundMusicAudioSource.Stop();
        backgroundMusicAudioSource.clip = inGameMusic;
        backgroundMusicAudioSource.volume = .8f;
        backgroundMusicAudioSource.Play();
    }
    public void PlayTitleMusic()
    {
        backgroundMusicAudioSource.Stop();
        backgroundMusicAudioSource.clip = titleScreenMusic;
        backgroundMusicAudioSource.volume = 0.15f;
        backgroundMusicAudioSource.Play();
    }

    public void PlayInsertCoinSFX()
    {
        soundEffectAudioSource.pitch = Random.Range(0.9f, 1.1f);
        soundEffectAudioSource.PlayOneShot(insertCoinSFX);
    }
    public void PlayStartLevelSFX()
    {
        soundEffectAudioSource.pitch = 1;
        soundEffectAudioSource.PlayOneShot(startLevelSFX);
    }
    public void PlayGrabCollectibleSFX()
    {
        soundEffectAudioSource.pitch = 1;
        soundEffectAudioSource.PlayOneShot(grabCollectibleSFX);
    }
    public void PlayGrabKeySFX()
    {
        soundEffectAudioSource.pitch = 1;
        soundEffectAudioSource.PlayOneShot(grabKeySFX);
    }
    public void PlayGrabDiamondSFX()
    {
        soundEffectAudioSource.pitch = 1;
        soundEffectAudioSource.PlayOneShot(grabDiamondSFX);
    }
    public void PlayGrabCoalSFX()
    {
        soundEffectAudioSource.pitch = 1;
        soundEffectAudioSource.PlayOneShot(grabCoalSFX);
    }
    public void PlayGrabExtraLifeSFX()
    {
        soundEffectAudioSource.pitch = 1;
        soundEffectAudioSource.PlayOneShot(grabExtraLifeSFX, 0.9f);
    }
    public void PlayOpenDoorSFX()
    {
        soundEffectAudioSource.pitch = 1;
        soundEffectAudioSource.PlayOneShot(openDoorSFX);
    }
    public void PlayLoseLifeSFX()
    {
        soundEffectAudioSource.pitch = 1;
        soundEffectAudioSource.PlayOneShot(loseLifeSFX);
    }
    public void PlayFinishLevelSFX()
    {
        soundEffectAudioSource.pitch = 1;
        soundEffectAudioSource.PlayOneShot(finishLevelSFX);
    }
    public void PlayDayNightSwitchSFX()
    {
        soundEffectAudioSource.pitch = 1;
        soundEffectAudioSource.PlayOneShot(dayNightSwitchSFX);
    }
}
