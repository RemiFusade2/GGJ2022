using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    [Header("Player id")]
    public int playerID;

    [Header("Data")]
    public PlayerData playerData;

    [Header("Settings - controls")]
    public string horizontalInputName;
    public string verticalInputName;

    private Player rewiredPlayer;

    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }

    private Rigidbody2D playerRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rewiredPlayer = ReInput.players.GetPlayer(playerID);
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UpdateHorizontalInput();
        UpdateVerticalInput();

        if (GameManager.instance.gameIsRunning)
        {
            Vector2 moveInput = ((HorizontalInput * Vector2.right).normalized + (VerticalInput * Vector2.up).normalized) * playerData.moveSpeed;
            playerRigidbody.velocity = moveInput;
        }
    }

    #region Update Inputs

    private void UpdateHorizontalInput()
    {
        HorizontalInput = rewiredPlayer.GetAxis(horizontalInputName);
    }

    private void UpdateVerticalInput()
    {
        VerticalInput = rewiredPlayer.GetAxis(verticalInputName);
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collectible"))
        {
            GameManager.instance.IncreaseScore(100);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Exit"))
        {
            GameManager.instance.FinishLevel();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Monster"))
        {
            GameManager.instance.LoseLife();
        }
        else if (collision.collider.CompareTag("Collectible"))
        {
            GameManager.instance.IncreaseScore(100);
            Destroy(collision.collider.gameObject);
        }
        else if (collision.collider.CompareTag("Key"))
        {
            GameManager.instance.AddKey();
            Destroy(collision.collider.gameObject);
        }
        else if (collision.collider.CompareTag("Door"))
        {
            if (GameManager.instance.UseKey())
            {
                // we open door
                GameManager.instance.IncreaseScore(500);
                Destroy(collision.collider.gameObject);
            }
            else
            {
                // we can't open the door, we do nothing
            }
        }
    }

    public void StopPlayer()
    {
        playerRigidbody.velocity = Vector2.zero;
    }

    public void Teleport(Vector2 newPosition)
    {
        playerRigidbody.isKinematic = true;
        playerRigidbody.GetComponent<Collider2D>().enabled = false;
        StopPlayer();
        playerRigidbody.position = newPosition;

        playerRigidbody.GetComponent<Collider2D>().enabled = true;
        playerRigidbody.isKinematic = false;
    }
}
