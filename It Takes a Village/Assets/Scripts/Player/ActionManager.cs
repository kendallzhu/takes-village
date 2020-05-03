using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ActionManager : MonoBehaviour
{
    public const int userControlledPlayerId = 0;
    // index is the player id
    public List<ActionQueue> actionQueues = new List<ActionQueue> {
        new ActionQueue(),
        new ActionQueue(),
        new ActionQueue(),
        new ActionQueue()
    };

    public GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        actionQueues = new List<ActionQueue> {
            new ActionQueue(),
            new ActionQueue(),
            new ActionQueue(),
            new ActionQueue()
        };
    }

    // TODO: change to scriptable object for easier testing?
    void Update()
    {
        // take mouse input
        /*if (Input.GetMouseButton(0))
        {
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        } */
        if (gameManager.isPlayingRound)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
            Vector2 checkpoint = GetUserControlledPlayerPos();
            GetUserControlledActionQueue().AddAction(new Action(Action.Type.move, inputVector, Time.time, checkpoint));
            if (Input.GetKey("1"))
            {
                GetUserControlledActionQueue().AddAction(new Action(Action.Type.plant, inputVector, Time.time, checkpoint));
            }
            else if (Input.GetKey("2"))
            {
                GetUserControlledActionQueue().AddAction(new Action(Action.Type.water, inputVector, Time.time, checkpoint));
            }
            else if (Input.GetKey("3"))
            {
                GetUserControlledActionQueue().AddAction(new Action(Action.Type.pick, inputVector, Time.time, checkpoint));
            }
        }
    }

    public void EndActions()
    {
        foreach (ActionQueue actionQueue in actionQueues)
        {
            actionQueue.EndActions();
        }
    }

    public void RotatePlayers(float timeOffset)
    {
        ActionQueue userControlledActionQueue = GetUserControlledActionQueue();
        actionQueues.RemoveAt(0);
        actionQueues.Add(userControlledActionQueue);
        foreach (ActionQueue actionQueue in actionQueues)
        {
            actionQueue.Reload(timeOffset);
        }
        GetUserControlledActionQueue().Clear();
    }

    Vector2 GetUserControlledPlayerPos()
    {
        PlayerController userControlledPlayer =
            GameObject.FindObjectsOfType<PlayerController>().ToList().Find(p => p.id == userControlledPlayerId);
        return userControlledPlayer.transform.position;
    }

    public ActionQueue GetUserControlledActionQueue()
    {
        return actionQueues[userControlledPlayerId];
    }

    public ActionQueue GetActionQueue(int playerId)
    {
        return actionQueues[playerId];
    }

    public Action GetCurrentAction(int playerId)
    {
        return GetActionQueue(playerId).GetCurrentAction();
    }
}
