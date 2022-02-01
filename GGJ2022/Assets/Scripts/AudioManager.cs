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
        soundEffectAudioSource.PlayOneShot(insertCoinSFX);
    }
    public void PlayStartLevelSFX()
    {
        soundEffectAudioSource.PlayOneShot(startLevelSFX);
    }
    public void PlayGrabCollectibleSFX()
    {
        soundEffectAudioSource.PlayOneShot(grabCollectibleSFX);
    }
    public void PlayGrabKeySFX()
    {
        soundEffectAudioSource.PlayOneShot(grabKeySFX);
    }
    public void PlayGrabDiamondSFX()
    {
        soundEffectAudioSource.PlayOneShot(grabDiamondSFX);
    }
    public void PlayGrabCoalSFX()
    {
        soundEffectAudioSource.PlayOneShot(grabCoalSFX);
    }
    public void PlayGrabExtraLifeSFX()
    {
        soundEffectAudioSource.PlayOneShot(grabExtraLifeSFX, 0.9f);
    }
    public void PlayOpenDoorSFX()
    {
        soundEffectAudioSource.PlayOneShot(openDoorSFX);
    }
    public void PlayLoseLifeSFX()
    {
        soundEffectAudioSource.PlayOneShot(loseLifeSFX);
    }
    public void PlayFinishLevelSFX()
    {
        soundEffectAudioSource.PlayOneShot(finishLevelSFX);
    }
    public void PlayDayNightSwitchSFX()
    {
        soundEffectAudioSource.PlayOneShot(dayNightSwitchSFX);
    }
}
