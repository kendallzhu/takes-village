using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public const float growTime = 12f;
    public enum Stage { seed, sapling, tree, emptyTree }
    
    public Stage stage;
    private bool isGrowing;
    private SpriteRenderer sprite;
    // list of player ids who have taken actions on this tree this round
    private GameManager gameManager;
    AudioSource audioSource;

    void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
        sprite.sprite = Resources.Load<Sprite>(stage.ToString());
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

    public void GrowSeed()
    {
        if (stage == Stage.seed)
        {
            stage = Stage.sapling;
            SwitchSprites("sapling", 0);
            isGrowing = false;
        }
    }

    public void GrowSapling()
    {
        if (stage == Stage.sapling)
        {
            stage = Stage.tree;
            SwitchSprites("tree", 0f);
            isGrowing = false;
        }
    }

    public void GrowTree()
    {
        if (stage == Stage.tree)
        {
            stage = Stage.emptyTree;
            SwitchSprites("emptyTree", 0f);
            isGrowing = false;
        }
    }

    public void GrowthAction(int playerId)
    {
        isGrowing = true;
        bool isUserAction = (playerId == ActionManager.userControlledPlayerId);
        AudioClip actionSound = null;
        switch (stage)
        {
            // picking fruit is always immediate
            case Stage.tree:
                actionSound = Resources.Load<AudioClip>("SoundEffects/Action/Harvest");
                gameManager.IncreaseScore(10);
                GrowTree();
                break;
            // but user controlled player actions don't cause growth immediately
            // (only in later rounds, they are done via action replay, then the plant grows)
            case Stage.seed:
                actionSound = Resources.Load<AudioClip>("SoundEffects/Action/Dig1");
                SwitchSprites("plantedSeed", .1f);
                DoAfterDelay(GrowSeed, growTime);
                break;
            case Stage.sapling:
                actionSound = Resources.Load<AudioClip>("SoundEffects/Action/Water");
                SwitchSprites("wateredSapling", .1f);
                DoAfterDelay(GrowSapling, growTime);
                break;
            default:
                break;
        }
        if (isUserAction)
        {
            audioSource.volume = 1f;
        }
        else
        {
            // quieter when other people do stuff
            audioSource.volume = .5f;
        }
        audioSource.PlayOneShot(actionSound);
    }

    public void ReceiveAction(Action.Type actionType, int playerId)
    {
        if (actionType == Action.Type.move || isGrowing)
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
