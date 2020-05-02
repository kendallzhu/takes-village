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
}