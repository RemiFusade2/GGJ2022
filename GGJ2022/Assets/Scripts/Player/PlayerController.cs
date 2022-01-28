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
    public string startInputName;

    private Player rewiredPlayer;

    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    public bool StartInput { get; private set; }

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
        UpdateStartInput();
        UpdateHorizontalInput();
        UpdateVerticalInput();

        if (StartInput && !GameManager.instance.gameStarted)
        {
            GameManager.instance.StartGame();
        }

        if (GameManager.instance.gameStarted)
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

    private void UpdateStartInput()
    {
        StartInput = rewiredPlayer.GetButtonDown(startInputName);
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
            GameManager.instance.ReloadGame();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Monster"))
        {
            GameManager.instance.ReloadGame();
        }
    }
}
