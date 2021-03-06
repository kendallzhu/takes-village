﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const float baseRoundDuration = 24;
    public const float maxRoundDuration = 72;

    // references
    private ActionManager actionManager;
    AudioSource soundTrackPlayer;

    // active fields
    public List<int> roundScores;
    public bool isPlayingRound;
    public float roundStartTime;

    public int score;

    void Awake()
    {
        Debug.Assert(!isPlayingRound);
        score = 0;
        actionManager = GameObject.FindObjectOfType<ActionManager>();
        soundTrackPlayer = GetComponent<AudioSource>();
        StartRound();
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeLeft() <= 0)
        {
            EndRound();
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
    }

    public void StartRound()
    {
        if (roundScores.Count > 0)
        {
            // Debug.Log("unload!");
            SceneManager.UnloadSceneAsync("SquareMap");
            actionManager.RotatePlayers(Time.time - roundStartTime);
            // play special noises
            AudioClip roundStartSound = Resources.Load<AudioClip>("SoundEffects/UI/StartRound");
            soundTrackPlayer.PlayOneShot(roundStartSound);
            StartCoroutine(PlaySoundtrackAfterDelay(PickSoundTrack(), 1.5f));
        } else
        {
            StartCoroutine(PlaySoundtrackAfterDelay(PickSoundTrack(), .1f));
        }
        // Debug.Log("load!");
        roundStartTime = Time.time;
        isPlayingRound = true;
        score = 0;
        SceneManager.LoadScene("SquareMap", LoadSceneMode.Additive);
    }

    IEnumerator PlaySoundtrackAfterDelay(AudioClip sound, float delay)
    {
        soundTrackPlayer.clip = null;
        yield return new WaitForSeconds(delay);
        soundTrackPlayer.volume = 1;
        soundTrackPlayer.clip = sound;
        soundTrackPlayer.loop = true;
        soundTrackPlayer.Play();
    }

    public AudioClip PickSoundTrack()
    {
        switch (NumPlayersActive())
        {
            case 0:
                return Resources.Load<AudioClip>("SoundTrack/1layer");
            case 1:
                return Resources.Load<AudioClip>("SoundTrack/2layer");
            case 2:
                return Resources.Load<AudioClip>("SoundTrack/3layer");
            case 3:
                return Resources.Load<AudioClip>("SoundTrack/4layer");
            default:
                return Resources.Load<AudioClip>("SoundTrack/fullSoundTrack");
        }
    }

    public void EndRound()
    {
        actionManager.EndActions();
        isPlayingRound = false;
        roundScores.Add(score);
        soundTrackPlayer.Stop();
    }

    public float CurrentRoundDuration()
    {
        return Mathf.Min(baseRoundDuration * (NumPlayersActive() + 1), maxRoundDuration);
    }

    public float TimeLeft()
    {
        return CurrentRoundDuration() - (Time.time - roundStartTime);
    }

    public void LoadCredits()
    {
        SceneManager.UnloadSceneAsync("SquareMap");
        SceneManager.LoadScene("Credits", LoadSceneMode.Additive);
        AudioClip fullSoundtrack = Resources.Load<AudioClip>("SoundTrack/fullSoundTrack");
        StartCoroutine(PlaySoundtrackAfterDelay(fullSoundtrack, 0f));
    }

    public int NumPlayersActive()
    {
        return Mathf.Min(4, roundScores.Count);
    }

    public int HighScore()
    {
        if (roundScores.Count == 0)
        {
            return 0;
        }
        return Mathf.Max(roundScores.ToArray());
    }
}
