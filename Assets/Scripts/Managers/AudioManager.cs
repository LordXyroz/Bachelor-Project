using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    
    // Ensures singleton
    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        PlayMenuBGM(true, 0.05f);
    }

    // Update is called once per frame
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

    private void FadeToNextClip()
    {
        bgmSource.clip = _nextClip;
        _fade = FadeState.FadingIn;

        if (bgmSource.enabled)
            bgmSource.Play();
    }

    public void PlayMenuBGM(bool fade, float fadeIn = 0.1f, float fadeOut = 0.5f)
    {
        if (fade)
        {
            Fade(mainMenuOST);
        }
        else
        {
            bgmSource.clip = mainMenuOST;
            bgmSource.Play();
            _bgmIsPlaying = true;
        }
    }

    public void PlayScenarioCreatorBGM(bool fade, float fadeIn = 0.1f, float fadeOut = 0.5f)
    {
        if (fade)
        {
            Fade(scenarioCreatorOST);
        }
        else
        {
            bgmSource.clip = scenarioCreatorOST;
            bgmSource.Play();
            _bgmIsPlaying = true;
        }
    }

    public void PlayGameplayBGM(bool fade, float fadeIn = 0.1f, float fadeOut = 0.5f)
    {
        if (fade)
        {
            Fade(gameplayOST1);
        }
        else
        {
            bgmSource.clip = gameplayOST1;
            bgmSource.Play();
            _bgmIsPlaying = true;
        }
    }

    public void SetBGMVolume(float vol)
    {
        _bgmMaxVolume = vol;
        bgmSource.volume = vol;
    }
}
