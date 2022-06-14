using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;


[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController1 : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float health = 100;
    [SerializeField]
    private float invincibilitySeconds = 1.5f;
    [SerializeField] 
    private float dodgeDuration = .5f;
    [SerializeField]
    private float dodgeSpeed = .25f;


    private CharacterController controller;
    private PlayerInput playerInput;


    private Vector3 playerVelocity;
    private bool isInvincible = false;
    private Rigidbody rb;


    private InputAction moveAction;
    private InputAction aimAction;
    private InputAction shootAction;
    private InputAction dodgeActionL;
    private InputAction dodgeActionR;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];
        shootAction = playerInput.actions["Shoot"];
        dodgeActionL = playerInput.actions["DodgeL"];
        dodgeActionR = playerInput.actions["DodgeR"];
    }




    private void Update()
    {
        Dodge();          
        if (health <= 0) Dead();
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

    public void Dodge()
    {
        if (dodgeActionR.triggered)
        {
            Debug.Log("Right");
            DodgeRight();

        }
        else if (dodgeActionL.triggered)
        {
            Debug.Log("Left");
            DodgeLeft();

        }
    } 
    private void DodgeRight()
    {

        transform.DOMoveZ(-2, dodgeSpeed).OnComplete(() =>
            {
                transform.DOMove(transform.position, dodgeDuration).OnComplete(() =>
                {
                    transform.DOMoveZ(0, dodgeSpeed);
                });
            });
    }

    private void DodgeLeft()
    {
        
        transform.DOMoveZ(2, dodgeSpeed).OnComplete(() =>
        {
            transform.DOMove(transform.position, dodgeDuration).OnComplete(() =>
            {
                transform.DOMoveZ(0, dodgeSpeed);
            });
        });
    }
    public void LoseHealth()
    {
        
        if (isInvincible) return;
        
        health -= 20;

        
        
        StartCoroutine(BecomeInvincible());
    }

    public void Dead()
    {
        //player is dead
        
        health = 0;
        Debug.Log("You dead, buddy.");
        StartCoroutine(DeathSequence());
        return;
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
