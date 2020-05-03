using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public enum Stage { seed, sapling, tree, emptyTree }
    
    public Stage stage;
    private SpriteRenderer sprite;
    // list of player ids who have taken actions on this tree this round
    private List<int> actionPlayerIds;
    private GameManager gameManager;
    AudioSource audioSource;

    void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        actionPlayerIds = new List<int>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
        sprite.sprite = Resources.Load<Sprite>(stage.ToString());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator SwitchSpritesAfterDelay(Sprite newSprite, float delay)
    {
        yield return new WaitForSeconds(delay);
        sprite.sprite = newSprite;
    }

    public void GrowthAction(int playerId)
    {
        // each player can take a growth action on a plant only once per a round
        if (actionPlayerIds.Contains(playerId))
        {
            // show some icon tho!
            return;
        }
        AudioClip actionSound = null;
        bool isUserAction = (playerId == ActionManager.userControlledPlayerId);
        switch (stage)
        {
            // picking fruit is always immediate
            case Stage.tree:
                actionSound = Resources.Load<AudioClip>("SoundEffects/Action/Harvest");
                stage = Stage.emptyTree;
                gameManager.IncreaseScore(10);
                StartCoroutine(SwitchSpritesAfterDelay(Resources.Load<Sprite>("emptyTree"), .1f));
                break;
            // but user controlled player actions don't cause growth immediately
            // (only in later rounds, they are done via action replay, then the plant grows)
            case Stage.seed:
                if (!isUserAction)
                {
                    stage = Stage.sapling;
                    StartCoroutine(SwitchSpritesAfterDelay(Resources.Load<Sprite>("plantedSeed"), .1f));
                    StartCoroutine(SwitchSpritesAfterDelay(Resources.Load<Sprite>("sapling"), .7f));
                } else
                {
                    StartCoroutine(SwitchSpritesAfterDelay(Resources.Load<Sprite>("plantedSeed"), .1f));
                }
                actionSound = Resources.Load<AudioClip>("SoundEffects/Action/Dig1");
                // gameManager.IncreaseScore(1);
                break;
            case Stage.sapling:
                if (!isUserAction)
                {
                    stage = Stage.tree;
                    StartCoroutine(SwitchSpritesAfterDelay(Resources.Load<Sprite>("wateredSapling"), .1f));
                    StartCoroutine(SwitchSpritesAfterDelay(Resources.Load<Sprite>("tree"), .7f));
                } else
                {
                    StartCoroutine(SwitchSpritesAfterDelay(Resources.Load<Sprite>("wateredSapling"), .1f));
                }
                actionSound = Resources.Load<AudioClip>("SoundEffects/Action/Water");
                // gameManager.IncreaseScore(2);
                break;
            default:
                break;
        }
        actionPlayerIds.Add(playerId);
        audioSource.clip = actionSound;
        if (isUserAction)
        {
            audioSource.volume = 1f;
        } else
        {
            // quieter when other people do stuff
            audioSource.volume = .5f;
        }
        audioSource.Play();
    }

    public void ReceiveAction(Action.Type actionType, int playerId)
    {
        if (actionType == Action.Type.move)
        {
            return;
        }
        else if (actionType == Action.Type.plant)
        {
            if (stage == Stage.seed)
            {
                GrowthAction(playerId);
            }
        }
        else if (actionType == Action.Type.water)
        {
            if (stage == Stage.sapling)
            {
                GrowthAction(playerId);
            }
        }
        else if (actionType == Action.Type.pick)
        {
            if (stage == Stage.tree)
            {
                GrowthAction(playerId);
            }
        }
    }
}
