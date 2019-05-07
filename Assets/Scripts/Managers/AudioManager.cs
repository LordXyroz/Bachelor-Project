using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class using static instance.
/// Controls music and sound effects.
/// </summary>
public class AudioManager : MonoBehaviour
{
    private enum FadeState
    {
        None,
        FadingOut,
        FadingIn
    }

    [Header("Instance")]
    public static AudioManager instance = null;

    [Header("Audio Source")]
    public AudioSource bgmSource;
    public AudioSource efxSource;

    [Header("BGM")]
    public AudioClip mainMenuOST;
    public AudioClip scenarioCreatorOST;
    public AudioClip gameplayOST1;

    [Header("EFX")]
    public AudioClip efx1;

    [Header("Controls")]
    private bool _bgmIsPlaying = false;

    public bool BgmIsPlaying
    {
        get => _bgmIsPlaying;
    }

    private float _bgmVolume = 0.0f;
    private float _efxVolume = 1.0f;

    [SerializeField]
    private float _bgmMaxVolume = 1.0f;
    private float _efxMaxVolume = 1.0f;

    public float BgmMaxVolume
    {
        get => _bgmMaxVolume;
    }

    private FadeState _fade = FadeState.None;

    private const float _fadeThreshold = 0.01f;
    private float _fadeInSpeed = 0.1f;
    private float _fadeOutSpeed = 0.5f;

    private AudioClip _nextClip = null;
    
    
    /// <summary>
    /// Runs before start.
    /// By using static Audiomanager instance, we can ensure only a single one if found in a scene.
    /// Sets the object carrying this script to not be destroyed between scenes.
    /// Starts the bgm music loop.
    /// </summary>
    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        PlayMenuBGM(true, 0.05f);
    }

    /// <summary>
    /// If we are fading, increases or decreases volume of music, or changes to next clip if we are not.
    /// </summary>
    void Update()
    {
        if (!bgmSource.enabled)
            return;

        if (_fade == FadeState.FadingOut)
        {
            if (_bgmVolume > _fadeThreshold)
            {
                _bgmVolume -= _fadeOutSpeed * Time.deltaTime;
                bgmSource.volume = _bgmVolume;
            }
            else
            {
                FadeToNextClip();
            }
        }
        else if (_fade == FadeState.FadingIn)
        {
            if (_bgmVolume < _bgmMaxVolume)
            {
                _bgmVolume += _fadeInSpeed * Time.deltaTime;
                bgmSource.volume = _bgmVolume;
            }
            else
            {
                _fade = FadeState.None;
            }
        }
    }

    /// <summary>
    /// Private fade function.
    /// Will start the fading if called with a different audio clip than is currently playing.
    /// </summary>
    /// <param name="clip">Audio clip to fade to</param>
    /// <param name="fadeIn">Fade in speed (volume per frame)</param>
    /// <param name="fadeOut">Fade out speed (volume per frame)</param>
    private void Fade(AudioClip clip, float fadeIn = 0.1f, float fadeOut = 0.5f)
    {
        if (clip == null || clip == bgmSource.clip)
            return;

        _nextClip = clip;
        _fadeInSpeed = fadeIn;
        _fadeOutSpeed = fadeOut;

        if (bgmSource.isPlaying)
            _fade = FadeState.FadingOut;
        else
            FadeToNextClip();
    }        

    /// <summary>
    /// Private function,
    /// Will change state to FadingIn and play the next clip.
    /// </summary>
    private void FadeToNextClip()
    {
        bgmSource.clip = _nextClip;
        _fade = FadeState.FadingIn;

        if (bgmSource.enabled)
            bgmSource.Play();
    }

    /// <summary>
    /// Plays the menu music.
    /// </summary>
    /// <param name="fade">Whether clips should fade</param>
    /// <param name="fadeIn">Fade in speed (volume per frame)</param>
    /// <param name="fadeOut">Fade out speed (volume per frame)</param>
    public void PlayMenuBGM(bool fade, float fadeIn = 0.1f, float fadeOut = 0.5f)
    {
        if (fade)
        {
            Fade(mainMenuOST, fadeIn, fadeOut);
        }
        else
        {
            bgmSource.clip = mainMenuOST;
            bgmSource.Play();
            _bgmIsPlaying = true;
        }
    }

    /// <summary>
    /// Plays the scenario creator music.
    /// Currently unused as there is no special music for that scene.
    /// </summary>
    /// <param name="fade">Whether clips should fade</param>
    /// <param name="fadeIn">Fade in speed (volume per frame)</param>
    /// <param name="fadeOut">Fade out speed (volume per frame)</param>
    public void PlayScenarioCreatorBGM(bool fade, float fadeIn = 0.1f, float fadeOut = 0.5f)
    {
        if (fade)
        {
            Fade(scenarioCreatorOST, fadeIn, fadeOut);
        }
        else
        {
            bgmSource.clip = scenarioCreatorOST;
            bgmSource.Play();
            _bgmIsPlaying = true;
        }
    }

    /// <summary>
    /// Plays the gameplay ost.
    /// Should set a playlist of different clips in the future.
    /// </summary>
    /// <param name="fade">Whether clips should fade</param>
    /// <param name="fadeIn">Fade in speed (volume per frame)</param>
    /// <param name="fadeOut">Fade out speed (volume per frame)</param>
    public void PlayGameplayBGM(bool fade, float fadeIn = 0.1f, float fadeOut = 0.5f)
    {
        if (fade)
        {
            Fade(gameplayOST1, fadeIn, fadeOut);
        }
        else
        {
            bgmSource.clip = gameplayOST1;
            bgmSource.Play();
            _bgmIsPlaying = true;
        }
    }

    /// <summary>
    /// Changes the volume of background music.
    /// </summary>
    /// <param name="vol">Volume to change to from 0.f to 1.f</param>
    public void SetBGMVolume(float vol)
    {
        _bgmMaxVolume = vol;
        bgmSource.volume = vol;
    }
}
