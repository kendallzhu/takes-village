using UnityEngine;
using UnityEditor;
using System.Collections;

public class Action
{
    public enum Type { move, plant, water, pick }

    public Type type;
    public Vector2 direction;
    public float timeStamp;

    public Action(Type type, Vector2 direction, float timeStamp)
    {
        this.type = type;
        this.direction = direction.normalized;
        this.timeStamp = timeStamp;
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