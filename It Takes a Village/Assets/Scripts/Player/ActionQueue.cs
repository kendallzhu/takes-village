using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ActionQueue
{
    // an action can be either a location or an object
    private Queue<Action> queue;
    private Action current;
    private Queue<Action> performed;

    public ActionQueue()
    {
        Clear();
    }

    // return the action that is the furthest buffered
    private Action GetNewestAddedAction()
    {
        if (queue.Count > 0)
        {
            return queue.Last();
        }
        return current;
    }

    public void AddAction(Action action)
    {
        Action precedingAction = GetNewestAddedAction();
        // Debug.Assert(precedingAction.timeStamp <= action.timeStamp);
        if (precedingAction.Equals(action))
        {
            // for identical actions - do nothing
            return;
        }
        if (precedingAction.BufferTime() > Time.time - precedingAction.timeStamp)
        {
            // if not enough time has passed - do nothing
            return;
        }
        queue.Enqueue(action);
    }

    public void EndActions()
    {
        queue.Enqueue(new Action(Action.Type.move, Vector2.zero, Time.time));
    }

    public Action GetCurrentAction()
    {
        while (queue.Count > 0 && queue.Peek().timeStamp < Time.time)
        {
            current = queue.Dequeue();
            performed.Enqueue(current);
        }
        return current;
    }

    public void Clear()
    {
        queue = new Queue<Action>();
        performed = new Queue<Action>();
        current = new Action(Action.Type.move, Vector2.zero, 0);
    }


    // loads up all of the performed actions to be replayed
    public void Reload(float timeOffset)
    {
        queue = performed;
        performed = new Queue<Action>();
        current = new Action(Action.Type.move, Vector2.zero, 0);
        // shift timestamps
        foreach (Action action in queue)
        {
            action.timeStamp += timeOffset;
        }
        // transform directions by 90 degrees counter clockwise
        foreach (Action action in queue)
        {
            action.RotateDirection(90);
        }
    }
}