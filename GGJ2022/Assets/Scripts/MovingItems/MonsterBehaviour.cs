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

    public MoveData moveDataGame1;
    public MoveData moveDataGame2;

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

        SetAnimation();
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

        SetAnimation();
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
            if (GetCurrentMoveData() != null)
            {
                monsterRigidbody.velocity = GetDirection() * GetCurrentMoveData().moveSpeed;
            }
        }
        else
        {
            monsterRigidbody.velocity = Vector2.zero;
        }
    }

    private MoveData GetCurrentMoveData()
    {
        MoveData currentMoveData = moveDataGame1;
        if (LevelManager.instance.GameDifficultyLevel == 2)
        {
            currentMoveData = moveDataGame2;
        }
        return currentMoveData;
    }

    /*
    private bool FreePathInDirection(Vector2 dir)
    {
        bool pathIsFree = true;
        float raycastDistance = 0.6f;

        RaycastHit2D[] allRayHits = Physics2D.RaycastAll(this.transform.position, dir, raycastDistance, moveData.obstacleLayerMask);

        foreach (RaycastHit2D hit in allRayHits)
        {
            if (!hit.collider.Equals(this))
            {
                pathIsFree = false;
            }
        }

        Debug.DrawRay(this.transform.position, new Vector3(dir.x, dir.y, 0), Color.red, 1.0f);

        return pathIsFree;
    }*/

    private void ForcePositionOnGrid()
    {
        Vector2 newPosition = new Vector2(Mathf.Round(monsterRigidbody.position.x * 2) / 2, Mathf.Round(monsterRigidbody.position.y * 2) / 2);
        this.transform.position = newPosition;
        //monsterRigidbody.MovePosition(newPosition);
    }

    private void TryChangeDirection(Collision2D collision)
    {
        if (GetCurrentMoveData() != null)
        {
            bool shouldChangeDirection = Vector2.Dot(GetDirection(), (collision.GetContact(0).point - monsterRigidbody.position)) > 0;
            if (shouldChangeDirection)
            {
                ForcePositionOnGrid();

                switch (GetCurrentMoveData().movementPattern)
                {
                    case MOVEMENT_PATTERN.NOT_MOVING:
                        break;
                    case MOVEMENT_PATTERN.BACK_AND_FORTH:
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
                        break;
                    case MOVEMENT_PATTERN.ALWAYS_TURN_LEFT:
                        switch (GetCurrentDirection())
                        {
                            case DIRECTION.UP:
                                SetCurrentDirection(DIRECTION.LEFT);
                                break;
                            case DIRECTION.DOWN:
                                SetCurrentDirection(DIRECTION.RIGHT);
                                break;
                            case DIRECTION.RIGHT:
                                SetCurrentDirection(DIRECTION.UP);
                                break;
                            case DIRECTION.LEFT:
                                SetCurrentDirection(DIRECTION.DOWN);
                                break;
                        }
                        break;
                    case MOVEMENT_PATTERN.ALWAYS_TURN_RIGHT:
                        switch (GetCurrentDirection())
                        {
                            case DIRECTION.UP:
                                SetCurrentDirection(DIRECTION.RIGHT);
                                break;
                            case DIRECTION.DOWN:
                                SetCurrentDirection(DIRECTION.LEFT);
                                break;
                            case DIRECTION.RIGHT:
                                SetCurrentDirection(DIRECTION.DOWN);
                                break;
                            case DIRECTION.LEFT:
                                SetCurrentDirection(DIRECTION.UP);
                                break;
                        }
                        break;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryChangeDirection(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryChangeDirection(collision);
    }
}
