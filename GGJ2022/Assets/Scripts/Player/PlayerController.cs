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

    [Header("Animator")]
    public Animator animator;
    private bool isDay = true;

    private Rigidbody2D playerRigidbody;

    private bool preventLosingMoreLives;

    // Start is called before the first frame update
    void Start()
    {
        rewiredPlayer = ReInput.players.GetPlayer(playerID);
        playerRigidbody = GetComponent<Rigidbody2D>();
        preventLosingMoreLives = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        UpdateHorizontalInput();
        UpdateVerticalInput();

        Animate();

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

    private void DontPreventLosingLives()
    {
        preventLosingMoreLives = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collectible"))
        {
            GameManager.instance.IncreaseScore(100, collision.gameObject.transform.position);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Exit"))
        {
            GameManager.instance.FinishLevel();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Monster") && !preventLosingMoreLives)
        {
            preventLosingMoreLives = true;
            Invoke("DontPreventLosingLives", 1.0f);
            AudioManager.instance.PlayLoseLifeSFX();
            GameManager.instance.LoseLife();
        }
        else if (collision.collider.CompareTag("Collectible"))
        {
            AudioManager.instance.PlayGrabCollectibleSFX();
            GameManager.instance.IncreaseScore(100, collision.gameObject.transform.position);
            LevelManager.instance.CollectCollectible();

            if (LevelManager.instance.WereAllCollectiblesCollected())
            {
                GameManager.instance.TrySpawnCompletionBonus();
            }

            Destroy(collision.collider.gameObject);
        }
        else if (collision.collider.CompareTag("Extra Life"))
        {
            AudioManager.instance.PlayGrabExtraLifeSFX();
            GameManager.instance.AddExtraLife();
            Destroy(collision.collider.gameObject);
        }
        else if (collision.collider.CompareTag("Diamond"))
        {
            AudioManager.instance.PlayGrabDiamondSFX();
            GameManager.instance.IncreaseScore(1000, collision.gameObject.transform.position);
            Destroy(collision.collider.gameObject);
        }
        else if (collision.collider.CompareTag("Coal"))
        {
            AudioManager.instance.PlayGrabCoalSFX();
            GameManager.instance.IncreaseScore(-50, collision.gameObject.transform.position);
            Destroy(collision.collider.gameObject);
        }
        else if (collision.collider.CompareTag("Key"))
        {
            AudioManager.instance.PlayGrabKeySFX();
            GameManager.instance.AddKey();
            GameManager.instance.IncreaseScore(50, collision.gameObject.transform.position);
            LevelManager.instance.CollectKey();
            Destroy(collision.collider.gameObject);
        }
        else if (collision.collider.CompareTag("Door"))
        {
            if (GameManager.instance.UseKey())
            {
                // we open door
                AudioManager.instance.PlayOpenDoorSFX();
                GameManager.instance.IncreaseScore(500, collision.gameObject.transform.position);
                Destroy(collision.collider.gameObject);
            }
            else
            {
                // we can't open the door, we do nothing
            }
        }
        else if (collision.collider.CompareTag("Exit"))
        {
            collision.collider.isTrigger = true;
            GameManager.instance.FinishLevel();
        }
    }

    private void Animate()
    {
        if (VerticalInput != 0)
        {
            animator.SetFloat("VerticalSpeed", VerticalInput);
        }
        else
        {
            animator.SetFloat("VerticalSpeed", 0f);
        }

        if (HorizontalInput != 0)
        {
            animator.SetFloat("HorizontalSpeed", HorizontalInput);
            // To prioritize left/right animation over up/down, otherwise it flicker between them if two arrows are pressed.
            animator.SetFloat("VerticalSpeed", 0f);
        }
        else
        {
            animator.SetFloat("HorizontalSpeed", 0f);
        }

        // Change player sprite depending on cycle.
        isDay = GameManager.instance.currentCycleType == CYCLETYPE.DAY ? true : false;
        animator.SetBool("IsDay", isDay);
    }

    public void MakePlayerMoveRight()
    {
        playerRigidbody.velocity = Vector2.right * playerData.moveSpeed;
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
