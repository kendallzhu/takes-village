using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBoard : MonoBehaviour
{
    public GameManager gameManager;
    public Text scoreText;
    AudioSource audioSource;

    // Use this for initialization
    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        scoreText = gameObject.GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        string newText = "Score: " + gameManager.score.ToString();
        if (newText != scoreText.text && gameManager.score != 0)
        {
            StartCoroutine(PlayScoreSoundWithDelay(.3f));
        }
        scoreText.text = newText;
    }

    IEnumerator PlayScoreSoundWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // play sound effect whenever score increases
        AudioClip scoreSound = Resources.Load<AudioClip>("SoundEffects/Event/Score");
        audioSource.PlayOneShot(scoreSound, .5f);
        
    }
}
