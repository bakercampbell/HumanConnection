using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController1 : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float health = 100;
    [SerializeField]
    private float invincibilitySeconds = 1.5f;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool isInvincible = false;
    private bool dead = false;

    private InputAction moveAction;
    private InputAction aimAction;
    private InputAction shootAction;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];
        shootAction = playerInput.actions["Shoot"];
    }

    private void Update()
    {
        if (dead)
        {
            StartCoroutine(DeathSequence());
        }
        return;
    }

    private void FixedUpdate()
    {
        playerVelocity.y = 0f;
      

        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(input.y, 0, 0);
        controller.Move(move * Time.deltaTime * playerSpeed);
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Segment"))
        {
            Debug.Log("you were hit.");
            LoseHealth();
        }
    }

    public void LoseHealth()
    {
        
        if (isInvincible) return;
        
        health -= 20;

        //player is dead
        if (health <= 0)
        {
            health = 0;
            Debug.Log("You dead, buddy.");
            dead = true;
            return;
        }

        StartCoroutine(BecomeInvincible());
    }

    private IEnumerator BecomeInvincible()
    {
        Debug.Log("Player turned invincible...");
        isInvincible = true;

        yield return new WaitForSeconds(invincibilitySeconds);

        isInvincible = false;
        Debug.Log("You are able to die!!!");
    }

    private IEnumerator DeathSequence()
    {

        SceneManager.LoadScene("MazeLevel", LoadSceneMode.Single);
        
        yield return new WaitForSeconds(4);

    }

}
