using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    const float movementSpeed = 1f;
    const float yDistanceRatio = .5f;
    const float actionRadius = .5f; // size of circle in front of player where action applies
    
    Rigidbody2D rbody;
    private int numCollisions = 0;
    Vector2 lastDirection;
    PlayerRenderer playerRenderer;

    public int id;
    private GameManager gameManager;
    private ActionManager actionManager;
    private Action previousAction;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponentInChildren<PlayerRenderer>();
        actionManager = GameObject.FindObjectOfType<ActionManager>();
    }

    private Vector2 DefaultDirection()
    {
        // make players face into the center
        Vector2 initialDirection = new Vector2(0, 1);
        return Quaternion.Euler(0, 0, -90 * id) * initialDirection;
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
        // load checkpoint if present, for accuracy during replays
        bool isNewAction = currentAction != previousAction;
        previousAction = currentAction;
        if (isNewAction && currentAction.isCheckpoint)
        {
            rbody.position = currentAction.referencePoint + currentAction.relativeCheckpoint;
            // Debug.Log("loaded checkpoint for player" + id.ToString());
        }
        bool isIdle = false;
        if (currentAction.type == Action.Type.move)
        {
            float speedCap = 1;
            /* stifle movement when colliding with anything (to make more predictable)
            Debug.Assert(numCollisions >= 0);
            if (numCollisions > 0)
            {
                speedCap = 1f; // aborted
            }*/
            // move towards current action direction if required
            Vector2 velocity = Vector2.ClampMagnitude(currentAction.direction, speedCap) * movementSpeed;
            velocity = WorldToMap(velocity);
            Vector2 newPos = currentPos + velocity * Time.fixedDeltaTime;
            rbody.MovePosition(newPos);
            // save direction
            isIdle = currentAction.direction.magnitude < .01f;
            if (!isIdle)
            {
                lastDirection = currentAction.direction;
            }
        }
        Vector2 targetPos = currentPos + WorldToMap(lastDirection) * actionRadius;
        List<Collider2D> colliders = Physics2D.OverlapCircleAll(targetPos, actionRadius).ToList();
        List<Collider2D> treeColliders = colliders.Where(c => c.GetComponent<Tree>() != null).ToList();
        // check custom bounds because of x-y asymmetry in isometric map
        treeColliders = treeColliders.Where(c => IsWithinActionRange(c.transform.position)).ToList();
        if (treeColliders.Count > 0)
        {
            Tree targetTree = treeColliders[0].GetComponent<Tree>();
            targetTree.ReceiveAction(currentAction.type, id);
        }

        // set action animation
        if (lastDirection == Vector2.zero)
        {
            lastDirection = DefaultDirection();
        }
        playerRenderer.PlayAnimation(currentAction.type, lastDirection, isIdle);
    }

    bool IsWithinActionRange(Vector2 position)
    {
        Vector2 delta = position - (Vector2)transform.position;
        return MapToWorld(delta).magnitude <= actionRadius;
    }

    Vector2 WorldToMap(Vector2 vector)
    {
        // reduce y component of velocity due to isometric proportions
        return new Vector2(vector.x, vector.y * yDistanceRatio);
    }

    Vector2 MapToWorld(Vector2 vector)
    {
        return new Vector2(vector.x, vector.y / yDistanceRatio);
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
