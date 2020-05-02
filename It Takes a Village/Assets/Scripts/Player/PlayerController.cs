using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1f;
    
    Rigidbody2D rbody;
    Vector2 lastDirection;
    PlayerRenderer playerRenderer;

    public int id;
    private ActionManager actionManager;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponentInChildren<PlayerRenderer>();
        actionManager = GameObject.FindObjectOfType<ActionManager>();
        if (actionManager == null)
        {
            SceneManager.LoadScene("Persistent");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 currentPos = rbody.position;
        Action currentAction = actionManager.GetCurrentAction(id);
        bool isIdle = false;
        if (currentAction.type == Action.Type.move)
        {
            // move towards current action direction if required
            Vector2 velocity = Vector2.ClampMagnitude(currentAction.direction, 1) * movementSpeed;
            Vector2 newPos = currentPos + velocity * Time.fixedDeltaTime;
            rbody.MovePosition(newPos);
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
}
