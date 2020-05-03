using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    const float movementSpeed = 1f;
    const float ySpeedRatio = .5f;
    
    Rigidbody2D rbody;
    private int numCollisions = 0;
    Vector2 lastDirection;
    PlayerRenderer playerRenderer;

    public int id;
    private GameManager gameManager;
    private ActionManager actionManager;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponentInChildren<PlayerRenderer>();
        actionManager = GameObject.FindObjectOfType<ActionManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (actionManager == null)
        {
            return;
        }
        Vector2 currentPos = rbody.position;
        Action currentAction = actionManager.GetCurrentAction(id);
        bool isIdle = false;
        if (currentAction.type == Action.Type.move)
        {
            // stifle movement when colliding with anything (to make more predictable)
            Debug.Assert(numCollisions >= 0);
            float speedCap = 1;
            if (numCollisions > 0)
            {
                speedCap = 1f; // aborted
            }
            // move towards current action direction if required
            Vector2 velocity = Vector2.ClampMagnitude(currentAction.direction, speedCap) * movementSpeed;
            // reduce y component of velocity due to isometric proportions
            velocity = new Vector2(velocity.x, velocity.y * ySpeedRatio);
            Vector2 newPos = currentPos + velocity * Time.fixedDeltaTime;
            rbody.MovePosition(newPos);
            // save direction
            isIdle = currentAction.direction.magnitude < .01f;
            if (!isIdle)
            {
                lastDirection = currentAction.direction;
            }
        }
        float actionRange = .25f;
        Vector2 targetPos = currentPos + lastDirection * actionRange;
        List<Collider2D> colliders = Physics2D.OverlapCircleAll(targetPos, .25f).ToList();
        Collider2D treeCollider = colliders.Find(c => c.GetComponent<Tree>() != null);
        if (treeCollider != null)
        {
            Tree targetTree = treeCollider.GetComponent<Tree>();
            targetTree.ReceiveAction(currentAction.type, id);
        }

        // set action animation
        playerRenderer.PlayAnimation(currentAction.type, lastDirection, isIdle);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        numCollisions++;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        numCollisions--;
    }
}
