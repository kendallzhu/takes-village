using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const float roundDuration = 10;

    // references
    private ActionManager actionManager;

    // active fields
    public List<int> roundScores;
    public bool isPlayingRound;
    public float roundStartTime;

    public int score;

    void Awake()
    {
        score = 0;
        actionManager = GameObject.FindObjectOfType<ActionManager>();
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
        roundStartTime = Time.time;
        isPlayingRound = true;
        score = 0;
        // TODO: tell actionmanager to rotate stuff
        if (roundScores.Count > 0)
        {
            SceneManager.UnloadSceneAsync("SquareMap");
        }
        SceneManager.LoadScene("SquareMap", LoadSceneMode.Additive);
    }

    public void EndRound()
    {
        isPlayingRound = false;
        roundScores.Add(score);
    }

    public float TimeLeft()
    {
        return roundDuration - (Time.time - roundStartTime);
    }
}
