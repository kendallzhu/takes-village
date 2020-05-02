using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour
{
    public GameManager gameManager;
    public Text timerText;

    // Use this for initialization
    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        timerText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        timerText.enabled = gameManager.isPlayingRound;
        float timeLeft = gameManager.TimeLeft();
        timerText.text = "timer: " + ((int)timeLeft).ToString();
    }
}
