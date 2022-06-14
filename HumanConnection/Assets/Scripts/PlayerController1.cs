using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;



[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController1 : MonoBehaviour
{
    [SerializeField, Tooltip("How fast the player moves.")]
    private float playerSpeed = 2.0f;
    [SerializeField, Tooltip("How high should the player jump.")]
    private float jumpHeight = 1.0f;
    [SerializeField, Tooltip("Speed of gravity, make sure it's negative for downwards gravity.")]
    private float gravityValue = -9.81f;
    [SerializeField, Tooltip("How fast the player rotates when moving the mouse around.")]
    private float rotationSpeed = 5f;
    [SerializeField, Tooltip("Prefab of the bullet to spawn when the player shoots.")]
    private GameObject bulletPrefab;
    [SerializeField, Tooltip("Location of the gun barrel to spawn the bullet prefab at.")]
    private Transform barrelTransform;
    [SerializeField, Tooltip("Where all the instantiated bullet prefabs should be put under, to avoid cluttering the hierarchy.")]
    private Transform bulletParent;
    [SerializeField, Tooltip("If the aim raycast does not hit the environment, this is the distance from the player when the bullet should be destroyed. This is to avoid bullet from traveling too far into the distance.")]
    private float bulletHitMissDistance = 3f;
    [SerializeField]
    private int health = 100;
    [SerializeField]
    private int invincibilitySeconds = 2;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private bool isInvincible;    
    private Transform cameraTransform;

    // Cached player input action to avoid continuously using string reference such as "Move".
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        // Cache a reference to all of the input actions to avoid them with strings constantly.
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        // Lock the cursor to the middle of the screen.
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        shootAction.performed += _ => ShootGun();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => ShootGun();
    }

    /// <summary>
    /// Spawn a bullet and shoot in the direction of the gun barrel.If the raycast hits the environment,
    /// the bullet travels towards to point of contact, else it will not have a target and be destroyed
    /// at a certain distance away from the player.
    /// </summary>
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
    {
        groundedPlayer = controller.isGrounded;
        // If the player is on the ground, there is no need to apply a downwards force.
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        // Take into account the camera direction when moving the player.
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Changes the height position of the player.
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Rotate the player towards aim/camera direction.
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (health <= 0) Dead();
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
