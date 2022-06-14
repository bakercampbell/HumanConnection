using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class TopDownMovement : MonoBehaviour
{
    CharacterController character;
    Vector3 moveVector;
    Vector3 startPos;
    [SerializeField, Range(0, 20)]
    float speed, turnSpeed;
    public float prevSpeed;
    [SerializeField, Range(0, 5)]
    float captureTimer, captureTimerReset;
    [SerializeField]
    LayerMask playerMask, interactableMask;
    [SerializeField]
    GameObject crossHair, bullet;
    [SerializeField]
    Transform firePoint;
    public Transform dragPoint;
    bool canShoot = true;
    bool isClicked;
    public bool isCaptured;
    void Start()
    {
        character = GetComponent<CharacterController>();
        startPos = transform.position;
        prevSpeed = speed;
    }

    private void Update()
    {
        if (isCaptured)
        {
            captureTimer -= Time.deltaTime;
            speed = 0;
        }
        if (captureTimer <= 0)
        {
            character.Move(startPos - transform.position);
            speed = prevSpeed;
            captureTimer = captureTimerReset;
            isCaptured = false;

        }
    }
    void FixedUpdate()
    {
            character.Move(moveVector * speed * Time.fixedDeltaTime);
    }

    public void OnMoveChanged(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        moveVector = new Vector3(direction.x, 0, direction.y);
    }

    public void Interact()
    {
        isClicked = !isClicked;
        if (isClicked)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                var potentialInteraction = Physics.OverlapSphere(firePoint.position, .5f, interactableMask, QueryTriggerInteraction.Ignore);
                if (potentialInteraction.Length > 0)
                {
                    potentialInteraction[0].GetComponent<Interactable>()?.Interact();

                }
                StartCoroutine(CanShoot());
            }
        }
    }

    /*public void OnClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && canShoot)
        {
            var fireDir = crossHair.transform.position - transform.position;
            var bulletObj = Instantiate(bullet, firePoint.position, Quaternion.identity);
            bulletObj.GetComponent<Rigidbody>().AddForce(fireDir.normalized * 10);
            StartCoroutine(CanShoot());
        }
    }
    */
    IEnumerator CanShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(.5f);
        canShoot = true;
    }

    public void OnMouseMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var direction = hit.point;
            var lookTarget = new Vector3(direction.x - transform.position.x, 0, direction.z - transform.position.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-lookTarget), turnSpeed);
        }
    }

    public void Captured()
    {
        Debug.Log("Help! I'm being oppressed!");
        isCaptured = true;
    }

}
