using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public GameManager gameManager;
    public Canvas menuCanvas;
    public Button playButton;
    public Button creditsButton;
    public const float scoreThreshold = 100; // allow going to credits

    // Use this for initialization
    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        menuCanvas = transform.Find("MenuCanvas").GetComponent<Canvas>();
        playButton = menuCanvas.transform.Find("Panel").Find("PlayButton").GetComponent<Button>();
        playButton.onClick.AddListener(gameManager.StartRound);
        creditsButton = menuCanvas.transform.Find("Panel").Find("CreditsButton").GetComponent<Button>();
        creditsButton.onClick.AddListener(gameManager.LoadCredits);
    }

    // Update is called once per frame
    void Update()
    {
        menuCanvas.gameObject.SetActive(!gameManager.isPlayingRound);
        creditsButton.gameObject.SetActive(gameManager.HighScore() >= scoreThreshold);
    }
}
