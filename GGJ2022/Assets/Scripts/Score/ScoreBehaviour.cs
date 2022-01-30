using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBehaviour : MonoBehaviour
{
    public float lifetime = 1f; // in seconds
    public float moveUpDistanceInPixels = 5;

    private float visibilityTimer;

    public SpriteRenderer spriteRenderer;

    public Sprite score1Sprite;
    public Sprite scoreMinus1Sprite;
    public Sprite score50Sprite;
    public Sprite scoreMinus50Sprite;
    public Sprite score100Sprite;
    public Sprite score500Sprite;
    public Sprite score1000Sprite;


    private Vector2 GetPosition()
    {
        return new Vector2(this.transform.position.x, this.transform.position.y);
    }

    private void SetPosition(Vector2 position)
    {
        Vector2 roundedPosition = new Vector2( (Mathf.Round(position.x * 16) / 16), (Mathf.Round(position.y * 16) / 16) );
        this.transform.position = new Vector3(roundedPosition.x, roundedPosition.y, 0);
    }

    public void Initialize(Vector2 position, int scoreValue)
    {
        SetPosition(position);
        visibilityTimer = lifetime;

        if (scoreValue == -50)
        {
            spriteRenderer.sprite = scoreMinus50Sprite;
        }
        else if (scoreValue == -1)
        {
            spriteRenderer.sprite = scoreMinus1Sprite;
        }
        else if (scoreValue == 1)
        {
            spriteRenderer.sprite = score1Sprite;
        }
        else if(scoreValue == 50)
        {
            spriteRenderer.sprite = score50Sprite;
        }
        else if (scoreValue == 100)
        {
            spriteRenderer.sprite = score100Sprite;
        }
        else if(scoreValue == 500)
        {
            spriteRenderer.sprite = score500Sprite;
        }
        else if (scoreValue == 1000)
        {
            spriteRenderer.sprite = score1000Sprite;
        }
        else
        {
            spriteRenderer.sprite = score100Sprite;
        }
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
