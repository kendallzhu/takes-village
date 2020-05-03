using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const float roundDuration = 5;

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
            Debug.Log("unload!");
            SceneManager.UnloadSceneAsync("SquareMap");
            actionManager.RotatePlayers(Time.time - roundStartTime);
        }
        Debug.Log("load!");
        roundStartTime = Time.time;
        isPlayingRound = true;
        score = 0;
        SceneManager.LoadScene("SquareMap", LoadSceneMode.Additive);
        AudioClip soundTrack = Resources.Load<AudioClip>("SoundTrack/fullSoundTrack");
        soundTrackPlayer.clip = soundTrack;
        soundTrackPlayer.Play();
    }

    public void EndRound()
    {
        actionManager.EndActions();
        isPlayingRound = false;
        roundScores.Add(score);
    }

    public float TimeLeft()
    {
        return roundDuration - (Time.time - roundStartTime);
    }
}
