using UnityEngine;
using UnityEditor;
using System.Collections;

public class Action
{
    public enum Type { move, plant, water, pick }

    public Type type;
    public Vector2 direction;
    public float timeStamp;
    // checkpoints to increase reliability of replays
    public bool isCheckpoint = false;
    public Vector2 relativeCheckpoint; // where the action started, relative to reference point
    public Vector2 referencePoint; // where the player started

    public Action(Type type, Vector2 direction, float timeStamp)
    {
        this.type = type;
        this.direction = direction.normalized;
        this.timeStamp = timeStamp;
    }

    public Action(Type type, Vector2 direction, float timeStamp, Vector2 relativeCheckpoint)
    {
        this.type = type;
        this.direction = direction.normalized;
        this.timeStamp = timeStamp;
        this.referencePoint = new Vector2(0, 0);
        this.relativeCheckpoint = relativeCheckpoint;
    }

    public bool Equals(Action other)
    {
        float deltaX = Mathf.Abs(direction.x - other.direction.x);
        float deltaY = Mathf.Abs(direction.y - other.direction.y);
        return type == other.type && (deltaX < .01f) && (deltaY < .01f);
    }

    public void RotateDirection(float degrees)
    {
        direction = Quaternion.Euler(0, 0, 90) * direction;
        // TODO: factor out into the map class
        relativeCheckpoint.x *= .5f;
        relativeCheckpoint.y *= 2;
        relativeCheckpoint = Quaternion.Euler(0, 0, 90) * relativeCheckpoint;
        Vector2 corner0 = Vector2.zero;
        Vector2 corner1 = new Vector2(8.5f, 4.25f);
        Vector2 corner2 = new Vector2(0, 8.5f);
        Vector2 corner3 = new Vector2(-8.5f, 4.25f);
        if (referencePoint == corner0)
        {
            referencePoint = corner1;
        } else if (referencePoint == corner1)
        {
            referencePoint = corner2;
        }
        else if (referencePoint == corner2)
        {
            referencePoint = corner3;
        }
        else if (referencePoint == corner3)
        {
            referencePoint = corner0;
        } else
        {
            Debug.Log("reference point is not one of the corner?!");
        }
    }

    public override string ToString()
    {
        return type.ToString() + ": " + direction.ToString();
    }

    // minimum time for a given action
    public float BufferTime()
    {
        switch (type)
        {
            case Type.plant:
                return 1f;
            case Type.water:
                return 1f;
            case Type.pick:
                return 1f;
            case Type.move:
                // make it less finicky when releasing keys
                if (direction.x != 0 && direction.y != 0)
                {
                    return .2f;
                }
                return 0f;
            default:
                return 0f;
        }
    }
}