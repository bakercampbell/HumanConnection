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
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 5f;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform barrelTransform;
    [SerializeField]
    private Transform bulletParent;
    [SerializeField]
    private float bulletHitMissDistance = 5f;    
    [SerializeField]
    private int health = 100;
    [SerializeField]
    private int invincibilitySeconds = 2;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private bool isInvincible = false;

    
    private Transform cameraTransform;


    // Cached player input action to avoid continuously using string reference such as "Move".
    private InputAction moveAction;
    private InputAction shootAction;

    private GameObject monster;
    private MonsterController monsterController;

    //private GameObject tazer;
    //private TazerController tazerController;

    private void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
        monsterController = monster.GetComponent<MonsterController>();
        //tazer = GameObject.FindGameObjectWithTag("Tazer");
        //tazerController = tazer.GetComponent<TazerController>();
    }

    private void Awake()
    {
        
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        //reference to all of the input actions with strings
        moveAction = playerInput.actions["Move"];
        shootAction = playerInput.actions["Shoot"];
        // lock the cursor to the middle of the screen.
        Cursor.lockState = CursorLockMode.Locked;
    }

    //shoot action
    private void OnEnable()
    {
        shootAction.performed += _ => ShootGun(); 

    }

    private void OnDisable()
    {
        shootAction.performed -= _ => ShootGun();
       
    }

    //spawns a bullet, referencing the bullet controller script.
    private void ShootGun()
    {
        
        RaycastHit hit;
        GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, bulletHitMissDistance))
        {
            bulletController.target = hit.point;
            bulletController.hit = true;
            Debug.Log("HIT!!!");

        }
        else
        {
            bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
            bulletController.hit = false;
            Debug.Log("Oooh, and that's a bad miss...");
        }
        
    }



    void Update()
    {   //checks if player is grounded, stops downward velocity
        Grounded();

        //directs movement and jumping
        Movement();
        
        //dead..
        Dead();

        Vector3 tooFar = new Vector3(46, transform.position.y, transform.position.z);
        if (transform.position.x > 46)
        {
            transform.position = tooFar;
        }
     
    }

    private void Grounded()
    {
        groundedPlayer = controller.isGrounded;
        
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
    }

    private void Movement()
    { 
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.y, 0, input.x * -1);
        
        
        controller.Move(move * Time.deltaTime * playerSpeed);

   

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Rotate the player towards aim/camera direction.
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    //triggers damage
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Segment"))
        {
            Debug.Log("you were hit.");
            LoseHealth();
        }
        else if (other.CompareTag("GotBehind"))
        {
            Debug.Log("too far, bud.");
            monsterController.behindBoss = true;
        }
    }

 

    //subtracts health and starts temporary invincibility coroutine
    public void LoseHealth()
    {
        
        if (isInvincible) return;
        
        health -= 20;

        
        
        StartCoroutine(BecomeInvincible());
    }

    //starts death sequence if health is zero or below
    public void Dead()
    {
        if (health > 0.1)
        {
            return;
        }
        else
        health = 0;
        Debug.Log("You dead, buddy.");
        StartCoroutine(DeathSequence());
        return;
    } 

    //invincibility Cooldown upon taking damage.
    private IEnumerator BecomeInvincible()
    {
        Debug.Log("Player turned invincible...");
        isInvincible = true;

        yield return new WaitForSeconds(invincibilitySeconds);

        isInvincible = false;
        Debug.Log("You are able to die!!!");
    }

    //waits druing animation then loads the next level
    private IEnumerator DeathSequence()
    {

        SceneManager.LoadScene("OutsideLab", LoadSceneMode.Single);
        
        yield return new WaitForSeconds(4);

    }



}
    