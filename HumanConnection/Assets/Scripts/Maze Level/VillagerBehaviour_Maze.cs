using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VillagerBehaviour_Maze : MonoBehaviour, Interactable
{
    NavMeshAgent nav;

    enum VillagerState {Idle, Moving, Running, Hiding, Captured};
    VillagerState currentState;
    [SerializeField]
    GameObject[] lightPoles;
    GameObject closestLight;
    GameObject furthestLight;
    [SerializeField, Range(0, 100)]
    float moveTimer, moveTimerReset;
    [SerializeField, Range(0, 100)]
    float lightDetectionRange, playerDetectionRange;
    int lightLayer;
    int playerLayer;
    
    Transform moveTarget;
    Transform runAwayTarget;


    [SerializeField]
    bool hasRandomStartTime = true;
    public bool isCaptured;


    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        moveTarget = NextMoveTarget();
        if (hasRandomStartTime)
        {
            moveTimer = Random.Range(2, 7);
        }
        lightLayer = LayerMask.NameToLayer("Light");
        playerLayer = LayerMask.NameToLayer("Player");
    }

    void Update()
    {
        moveTimer -= Time.deltaTime;

        if (moveTimer < 0)
        {
            currentState = VillagerState.Moving;
            moveTarget = NextMoveTarget();
            moveTimer = Random.Range(moveTimerReset/2, moveTimerReset);

        }

        else if (nav.enabled && nav.remainingDistance < 5 && nav.velocity == Vector3.zero)
            currentState = VillagerState.Idle;

        /*if (DetectPlayer())
        {
            currentState = VillagerState.Running;
        }
        else
            currentState = VillagerState.Moving;
        */
        switch (currentState)
        {
            case VillagerState.Idle:
                Idle();
                break;
            case VillagerState.Moving:
                Move();
                break;
            case VillagerState.Hiding:
                Hide();
                break;
            case VillagerState.Running:
                RunAway();
                break;
            case VillagerState.Captured:
                Captured();
                break;
        }
    }

    public void Interact()
    {
        Debug.Log("Captured");
        nav.enabled = false;
        var capturedParent = FindObjectOfType<TopDownMovement>().dragPoint.transform;
        transform.Translate(capturedParent.localPosition);
        transform.parent = capturedParent;
        currentState = VillagerState.Captured;
    }
    Transform NextMoveTarget()
    {
        return lightPoles[Random.Range(0, lightPoles.Length)].transform;
    }

    void FindClosestLight()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, lightDetectionRange, 1 << lightLayer);
        float minimumDistance = Mathf.Infinity;
        foreach (Collider collider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < minimumDistance)
            {
                minimumDistance = distance;
                closestLight = collider.gameObject;
            }
        }
        if (closestLight != null)
        {
            Debug.Log("Nearest Light: " + closestLight.transform.parent.name + "; Distance: " + minimumDistance);
        }
        else
        {
            Debug.Log("There is no light in the given radius");
        }
    }

    GameObject FindFurthestLight()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, lightDetectionRange, 1 << lightLayer);
        float maximumDistance = 0;
        foreach (Collider collider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance > maximumDistance)
            {
                maximumDistance = distance;
                furthestLight = collider.gameObject;
            }
        }
        if (furthestLight != null)
        {
            return furthestLight;
            Debug.Log("Furthest Light: " + closestLight.transform.parent.name + "; Distance: " + maximumDistance);
        }
        else
        {
            Debug.Log("There is no light in the given radius");
        }
        return null;
    }

    bool DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, playerDetectionRange, 1 << playerLayer);
        foreach (Collider collider in hitColliders)
        {
            if (collider != null)
            {
                return true;
            }
        }
        return false;
    }

    void Move()
    {
        //if (runAwayTarget != null)
            //runAwayTarget = null;
        if (nav.enabled)
            nav.SetDestination(moveTarget.position);
    }

    void Idle()
    {
        //Do Stuff. Maybe just an animation?
    }

    void Hide()
    {
        
    }

    void RunAway()
    {
        Debug.Log("Player detected by" + gameObject.name);
        /*    runAwayTarget = NextMoveTarget();

        if (runAwayTarget.position != moveTarget.position)
        {
            nav.SetDestination(runAwayTarget.position);
        }
        else
            RunAway();
        */
        nav.SetDestination(FindFurthestLight().transform.position);
    }

    void Captured()
    {
        //Do something
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            RunAway();
            Debug.Log("Trigger");
        }
    }
}
