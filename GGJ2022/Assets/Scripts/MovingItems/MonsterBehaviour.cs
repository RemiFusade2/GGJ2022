using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIRECTION
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public class MonsterBehaviour : MonoBehaviour
{
    private Rigidbody2D monsterRigidbody;

    public MoveData moveData;
    public Animator animator;

    public DIRECTION GetCurrentDirection()
    {
        return GetComponent<DayNightCycle>().facingDirection;
    }
    public void SetCurrentDirection(DIRECTION newDirection)
    {
        GetComponent<DayNightCycle>().facingDirection = newDirection;

        if (monsterRigidbody != null)
        {
            switch (newDirection)
            {
                case DIRECTION.DOWN:
                case DIRECTION.UP:
                    monsterRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                    break;
                case DIRECTION.LEFT:
                case DIRECTION.RIGHT:
                    monsterRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        monsterRigidbody = GetComponent<Rigidbody2D>();

        switch (GetComponent<DayNightCycle>().facingDirection)
        {
            case DIRECTION.DOWN:
            case DIRECTION.UP:
                monsterRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                break;
            case DIRECTION.LEFT:
            case DIRECTION.RIGHT:
                monsterRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 GetDirection()
    {
        Vector2 direction = Vector2.zero;
        switch (GetCurrentDirection())
        {
            case DIRECTION.UP:
                direction = Vector2.up;
                break;
            case DIRECTION.DOWN:
                direction = -Vector2.up;
                break;
            case DIRECTION.RIGHT:
                direction = Vector2.right;
                break;
            case DIRECTION.LEFT:
                direction = -Vector2.right;
                break;
        }
        return direction;
    }

    private void SetAnimation()
    {
        switch (GetCurrentDirection())
        {
            case DIRECTION.UP:
                animator.SetInteger("Direction", 0);
                break;
            case DIRECTION.DOWN:
                animator.SetInteger("Direction", 1);
                break;
            case DIRECTION.LEFT:
                animator.SetInteger("Direction", 2);
                break;
            case DIRECTION.RIGHT:
                animator.SetInteger("Direction", 3);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.gameIsRunning)
        {
            if (moveData != null)
            {
                monsterRigidbody.velocity = GetDirection() * moveData.moveSpeed;
            }
        }
        else
        {
            monsterRigidbody.velocity = Vector2.zero;
        }
    }

    private bool FreePathInDirection(Vector2 dir)
    {
        bool pathIsFree = true;
        float raycastDistance = 1.0f;
        if (Physics2D.Raycast(this.transform.position, dir, raycastDistance, moveData.obstacleLayerMask))
        {
            pathIsFree = false;
        }
        return pathIsFree;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("test");

        if (moveData != null && moveData.movementPattern == MOVEMENT_PATTERN.BACK_AND_FORTH)
        {
            if (!FreePathInDirection(GetDirection()))
            {
                switch (GetCurrentDirection())
                {
                    case DIRECTION.UP:
                        SetCurrentDirection(DIRECTION.DOWN);
                        break;
                    case DIRECTION.DOWN:
                        SetCurrentDirection(DIRECTION.UP);
                        break;
                    case DIRECTION.RIGHT:
                        SetCurrentDirection(DIRECTION.LEFT);
                        break;
                    case DIRECTION.LEFT:
                        SetCurrentDirection(DIRECTION.RIGHT);
                        break;
                }
                SetAnimation();
            }
        }
    }
}
