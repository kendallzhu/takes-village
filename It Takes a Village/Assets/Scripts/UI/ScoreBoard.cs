using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBoard : MonoBehaviour
{
    public GameManager gameManager;
    public Text scoreText;

    // Use this for initialization
    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        scoreText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "score: " + gameManager.score.ToString();
    }
}
