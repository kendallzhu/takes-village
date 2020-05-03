using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public enum Stage { seed, sapling, tree, emptyTree }
    
    public Stage stage;
    private Stage previousDisplayStage;
    private Stage displayStage;
    private bool isSuppressedSpriteSwitch;
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
        displayStage = stage;
        previousDisplayStage = stage;
    }

    void Update()
    {
        // update the sprite display to match displayStage
        if (previousDisplayStage != displayStage)
        {
            switch (previousDisplayStage)
            {
                case Stage.tree:
                    SwitchSprites("emptyTree", .1f);
                    break;
                case Stage.seed:
                    SwitchSprites("sapling", .5f);
                    break;
                case Stage.sapling:
                    SwitchSprites("tree", .5f);
                    break;
                default:
                    Debug.Log("tree stage is messed up?");
                    break;
            }
            previousDisplayStage = displayStage;
        }
    }

    private delegate void Function();

    void DoAfterDelay(Function f, float delay)
    {
        StartCoroutine(DoAfterDelayHelper(f, delay));
    }

    IEnumerator DoAfterDelayHelper(Function f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }

    void SwitchSprites(string spriteName, float delay = 0)
    {
        DoAfterDelay(() => sprite.sprite = Resources.Load<Sprite>(spriteName), delay);
    }

    public void UpdateDisplay()
    {
        displayStage = stage;
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
                displayStage = Stage.emptyTree;
                gameManager.IncreaseScore(10);
                break;
            // but user controlled player actions don't cause growth immediately
            // (only in later rounds, they are done via action replay, then the plant grows)
            case Stage.seed:
                stage = Stage.sapling;
                actionSound = Resources.Load<AudioClip>("SoundEffects/Action/Dig1");
                SwitchSprites("plantedSeed", .1f);
                if (!isUserAction)
                {
                    displayStage = Stage.sapling;
                }
                break;
            case Stage.sapling:
                stage = Stage.tree;
                actionSound = Resources.Load<AudioClip>("SoundEffects/Action/Water");
                SwitchSprites("wateredSapling", .1f);
                if (!isUserAction)
                {
                    displayStage = Stage.tree;
                }
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
