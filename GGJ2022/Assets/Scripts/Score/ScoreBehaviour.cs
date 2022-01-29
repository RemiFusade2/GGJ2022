using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBehaviour : MonoBehaviour
{
    public float lifetime = 1f; // in seconds
    public float moveUpDistanceInPixels = 5;

    private float visibilityTimer;


    private Vector2 GetPosition()
    {
        return new Vector2(this.transform.position.x, this.transform.position.y);
    }

    private void SetPosition(Vector2 position)
    {
        Vector2 roundedPosition = new Vector2( (Mathf.Round(position.x * 16) / 16), (Mathf.Round(position.y * 16) / 16) );
        this.transform.position = new Vector3(roundedPosition.x, roundedPosition.y, 0);
    }

    public void Initialize(Vector2 position)
    {
        SetPosition(position);
        visibilityTimer = lifetime;
    }

    public void MoveOneStepUp()
    {
        float stepValue = 0.0625f;
        Vector2 position = GetPosition();
        SetPosition(position + stepValue * Vector2.up);
    }

    private void DestroyItself()
    {
        Destroy(this.gameObject);
    }

    private void Start()
    {
        InvokeRepeating("MoveOneStepUp", lifetime / moveUpDistanceInPixels, lifetime / moveUpDistanceInPixels);
        Invoke("DestroyItself", lifetime);
    }
}
