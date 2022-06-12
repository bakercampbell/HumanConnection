using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class TopDownMovement : MonoBehaviour
{
    CharacterController character;
    Vector3 moveVector;
    [SerializeField, Range(0,20)]
    float speed, turnSpeed;
    [SerializeField]
    LayerMask playerMask, interactableMask;
    [SerializeField]
    GameObject crossHair, bullet;
    [SerializeField]
    Transform firePoint;
    public Transform dragPoint;
    bool canShoot = true;
    void Start()
    {
        character = GetComponent<CharacterController>();
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
        var potentialInteraction = Physics.OverlapSphere(firePoint.position, .5f, interactableMask, QueryTriggerInteraction.Ignore);
        if (potentialInteraction.Length > 0)
        {
            potentialInteraction[0].GetComponent<Interactable>()?.Interact();
            
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

    IEnumerator CanShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(.5f);
        canShoot = true;
    }
    */
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
}
