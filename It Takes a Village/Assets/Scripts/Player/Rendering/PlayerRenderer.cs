using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerRenderer : MonoBehaviour
{
    public static readonly string[] idleDirectionAnimations = { "Static N", "Static NW", "Static W", "Static SW", "Static S", "Static SE", "Static E", "Static NE" };
    public static readonly string[] runDirectionAnimations = {"Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE"};

    Animator animator;
    GameObject actionBubble;
    SpriteRenderer actionIcon;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        actionBubble = transform.Find("ActionBubble").gameObject;
        actionIcon = actionBubble.transform.Find("Icon").GetComponent<SpriteRenderer>();
    }

    public void PlayAnimation(Action.Type actionType, Vector2 direction, bool isIdle = false)
    {
        string[] directionAnimations = idleDirectionAnimations;
        if (actionType == Action.Type.move)
        {
            actionBubble.SetActive(false);
            if (isIdle)
            {
                directionAnimations = idleDirectionAnimations;
            } else
            {
                directionAnimations = runDirectionAnimations;
            }
        } else if (actionType == Action.Type.plant)
        {
            directionAnimations = runDirectionAnimations;
            actionBubble.SetActive(true);
            actionIcon.sprite = Resources.Load<Sprite>(actionType.ToString());
        } else if (actionType == Action.Type.water)
        {
            directionAnimations = runDirectionAnimations;
            actionBubble.SetActive(true);
            actionIcon.sprite = Resources.Load<Sprite>(actionType.ToString());
        } else if (actionType == Action.Type.pick)
        {
            directionAnimations = runDirectionAnimations;
            actionBubble.SetActive(true);
            actionIcon.sprite = Resources.Load<Sprite>(actionType.ToString());
        }
        int directionIndex = DirectionToIndex(direction, 8);

        //tell the animator to play the requested state
        animator.Play(directionAnimations[directionIndex]);
    }

    //helper functions

    //this function converts a Vector2 direction to an index to a slice around a circle
    //this goes in a counter-clockwise direction.
    public static int DirectionToIndex(Vector2 dir, int sliceCount){
        //get the normalized direction
        Vector2 normDir = dir.normalized;
        //calculate how many degrees one slice is
        float step = 360f / sliceCount;
        //calculate how many degress half a slice is.
        //we need this to offset the pie, so that the North (UP) slice is aligned in the center
        float halfstep = step / 2;
        //get the angle from -180 to 180 of the direction vector relative to the Up vector.
        //this will return the angle between dir and North.
        float angle = Vector2.SignedAngle(Vector2.up, normDir);
        //add the halfslice offset
        angle += halfstep;
        //if angle is negative, then let's make it positive by adding 360 to wrap it around.
        if (angle < 0){
            angle += 360;
        }
        //calculate the amount of steps required to reach this angle
        float stepCount = angle / step;
        //round it, and we have the answer!
        return Mathf.FloorToInt(stepCount);
    }







    //this function converts a string array to a int (animator hash) array.
    public static int[] AnimatorStringArrayToHashArray(string[] animationArray)
    {
        //allocate the same array length for our hash array
        int[] hashArray = new int[animationArray.Length];
        //loop through the string array
        for (int i = 0; i < animationArray.Length; i++)
        {
            //do the hash and save it to our hash array
            hashArray[i] = Animator.StringToHash(animationArray[i]);
        }
        //we're done!
        return hashArray;
    }

}
