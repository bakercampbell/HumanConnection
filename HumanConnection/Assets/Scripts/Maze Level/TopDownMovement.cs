using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class TopDownMovement : MonoBehaviour
{
    CharacterController character;
    Vector3 moveVector;
    Vector3 rotateVector;
    Vector3 startPos;
    [SerializeField, Range(0, 20)]
    float speed, turnSpeed, carryingSpeed;
    public float prevSpeed;
    [SerializeField, Range(0, 5)]
    float captureTimer, captureTimerReset;
    [SerializeField]
    LayerMask playerMask, interactableMask;
    [SerializeField]
    Transform firePoint;
    NavMeshObstacle navObstacle;
    public Transform dragPoint;
    bool canShoot = true;
    bool isClicked;
    public bool isCaptured;
    public bool isCarrying;
    public bool isSwarmed;
    int shotCount;
    
    void Start()
    {
        character = GetComponent<CharacterController>();
        startPos = transform.position;
        prevSpeed = speed;
        navObstacle = GetComponent<NavMeshObstacle>();
        shotCount = 0;
    }

    private void Update()
    {
        OutlineInteractable();
        if (isCaptured)
        {
            captureTimer -= Time.deltaTime;
            character.enabled = false;
        }
        else
        {
            character.enabled = true;
            captureTimer = captureTimerReset;
        }
        
        if (captureTimer <= 0)
        {
            transform.position = startPos;
            captureTimer = captureTimerReset;
            isCaptured = false;

        }
        if (isCarrying)
        {
            speed = carryingSpeed;
        }
        else
        {
            speed = prevSpeed;
        }
        if (transform.position.y > 2f)
            transform.position = transform.position - new Vector3(0, 1, 0);
    }
    void FixedUpdate()
    {
        character.Move(speed * Time.fixedDeltaTime * moveVector);
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

    void OutlineInteractable()
    {
        RaycastHit hit;
        var rayDirection = firePoint.position - transform.position;
        if (Physics.Raycast(firePoint.position, rayDirection, out hit, 5.5f, ~playerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(firePoint.position, rayDirection);
            if (hit.transform.gameObject.GetComponent<Outline>())
            {
                Debug.Log("Are you in range?");
                hit.transform.gameObject.SendMessage("Outlined", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void Shoot()
    {
        
        if (Mouse.current.rightButton.wasPressedThisFrame && canShoot)
        {
            Debug.Log(shotCount + 1);
            var fireDir = firePoint.transform.position - transform.position;
            var bulletObj = GetBullet();
            bulletObj?.GetComponent<Rigidbody>().AddForce(fireDir.normalized * 10);
            shotCount++;
            StartCoroutine(CanShoot());
            
        }
    }

    GameObject GetBullet()
    {
        GameObject bullet = Object_Pooler.SharedInstance.GetPooledObject();
        if (bullet != null)
        {
            bullet.transform.position = firePoint.transform.position;
            bullet.transform.rotation = firePoint.transform.rotation;
            bullet.SetActive(true);
            return bullet;
        }
        return null;
    }
    
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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget), turnSpeed);
        }
    }
    public void Captured()
    {
        Debug.Log("Help! I'm being oppressed!");
        isCaptured = true;
        ReleaseVillager();
    }

    public void Escaped()
    {
        Debug.Log("You'll never catch me!");
        isCaptured = false;
    }

    void ReleaseVillager()
    {
        isCarrying = false;
        var villager = GetComponentInChildren<VillagerBehaviour_Maze>()?.gameObject;
        if (villager != null)
            villager.GetComponent<VillagerBehaviour_Maze>().Rescue();
    }

}
