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

    public DIRECTION GetCurrentDirection()
    {
        return GetComponent<DayNightCycle>().facingDirection;
    }
    public void SetCurrentDirection(DIRECTION newDirection)
    {
        GetComponent<DayNightCycle>().facingDirection = newDirection;
    }

    // Start is called before the first frame update
    void Start()
    {
        monsterRigidbody = GetComponent<Rigidbody2D>();
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

    /*
    private List<DIRECTION> GetPossibleDirections()
    {
        float raycastDistance = 1.0f;
        LayerMask obstacleLayerMask;
        if (Physics2D.Raycast(this.transform.position, Vector2.right, raycastDistance, obstacleLayerMask))
        {

        }
    }*/

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (moveData != null && moveData.movementPattern == MOVEMENT_PATTERN.BACK_AND_FORTH)
        {
            switch(GetCurrentDirection())
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
        }
    }
}
