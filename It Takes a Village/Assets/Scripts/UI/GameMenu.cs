using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public GameManager gameManager;
    public Canvas menuCanvas;
    public Button playButton;

    // Use this for initialization
    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        menuCanvas = transform.Find("MenuCanvas").GetComponent<Canvas>();
        playButton = menuCanvas.transform.Find("Panel").Find("PlayButton").GetComponent<Button>();
        playButton.onClick.AddListener(gameManager.StartRound);
    }

    // Update is called once per frame
    void Update()
    {
        menuCanvas.gameObject.SetActive(!gameManager.isPlayingRound);
    }
}
