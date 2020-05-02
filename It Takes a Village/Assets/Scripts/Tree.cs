using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public enum Stage { seed, sapling, tree, emptyTree }
    [SerializeField]
    public Stage stage;
    private SpriteRenderer sprite;
    // list of player ids who have taken actions on this tree this round
    private List<int> actionPlayerIds;
    private GameManager gameManager;

    void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        actionPlayerIds = new List<int>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        sprite.sprite = Resources.Load<Sprite>(stage.ToString());
    }

    public void GrowthAction(int playerId)
    {
        // each player can take a growth action on a plant only once per a round
        if (actionPlayerIds.Contains(playerId))
        {
            // show some icon tho!
            return;
        }
        bool isUserAction = (playerId != ActionManager.userControlledPlayerId && false);
        switch (stage)
        {
            // picking fruit is always immediate
            case Stage.tree:
                stage = Stage.emptyTree;
                gameManager.IncreaseScore(10);
                break;
            // but user controlled player actions don't cause growth immediately
            // (only in later rounds, they are done via action replay, then the plant grows)
            case Stage.seed:
                if (!isUserAction)
                {
                    stage = Stage.sapling;
                }
                gameManager.IncreaseScore(1);
                break;
            case Stage.sapling:
                if (!isUserAction)
                {
                    stage = Stage.tree;
                }
                gameManager.IncreaseScore(2);
                break;
            default:
                break;
        }
        actionPlayerIds.Add(playerId);
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
