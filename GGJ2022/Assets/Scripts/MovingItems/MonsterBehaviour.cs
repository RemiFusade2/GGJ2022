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

    public float speed;

    public Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        monsterRigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(WaitAndChangeDirection(1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        monsterRigidbody.velocity = direction * speed;
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

    private IEnumerator WaitAndChangeDirection(float delay)
    {
        yield return new WaitForSeconds(delay);

        int randomValue = Random.Range(0, 4);
        if (randomValue == 0)
        {
            direction = Vector2.right;
        }
        else if (randomValue == 1)
        {
            direction = -Vector2.right;
        }
        else if(randomValue == 2)
        {
            direction = Vector2.up;
        }
        else
        {
            direction = -Vector2.up;
        }
        StartCoroutine(WaitAndChangeDirection(1.0f));
    }
}
