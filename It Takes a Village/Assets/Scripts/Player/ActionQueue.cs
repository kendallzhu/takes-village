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
        queue = new Queue<Action>();
        performed = new Queue<Action>();
        current = new Action(Action.Type.move, Vector2.zero, 0);
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
        if (precedingAction.Equals(action))
        {
            // for identical actions - do nothing
            return;
        }
        queue.Enqueue(action);
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
}