﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
            GetUserControlledActionQueue().AddAction(new Action(Action.Type.move, inputVector, Time.time));
            if (Input.GetKey("1"))
            {
                GetUserControlledActionQueue().AddAction(new Action(Action.Type.plant, inputVector, Time.time));
            }
            else if (Input.GetKey("2"))
            {
                GetUserControlledActionQueue().AddAction(new Action(Action.Type.water, inputVector, Time.time));
            }
            else if (Input.GetKey("3"))
            {
                GetUserControlledActionQueue().AddAction(new Action(Action.Type.pick, inputVector, Time.time));
            }
        }
    }

    public int NumPlayersActive()
    {
        int count = 0;
        foreach (ActionQueue actionQueue in actionQueues)
        {
            if (actionQueue.IsActive())
            {
                count++;
            }
        }
        return count;
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
