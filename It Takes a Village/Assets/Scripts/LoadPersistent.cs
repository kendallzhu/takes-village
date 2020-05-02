using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPersistent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            SceneManager.LoadScene("Persistent", LoadSceneMode.Single);
        }
    }
}
