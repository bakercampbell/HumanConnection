using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController1 : MonoBehaviour
{
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float extraHeight;
    [SerializeField] private LayerMask ground;

    private PlayerControls playerControls;
    private Rigidbody rb;
    private CapsuleCollider col;

    private bool isGrounded;


    private void Awake()
    {
        //Calls the PLayer Input
        playerControls = new PlayerControls();

        //Components attached to the player
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();


    }

    //called when the script is enabled (necessary for input system)
    private void OnEnable()
    {
        playerControls.Enable();
    }

    //Also required for input system to function
    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        //When Jump is pressed, passes value to the Jump function
        playerControls.Inside.Jump.performed += ctx => Jump(ctx.ReadValue<float>());

    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        
    }

    private void Jump(float val)
    {
        //Checks if button was pressed and player was grounded
        if (val == 1 && isGrounded is true)
        {
            //add upward force on rigidbody
            rb.AddForce(new Vector3(0, jumpSpeed), ForceMode.Impulse);
        }
    }


}
