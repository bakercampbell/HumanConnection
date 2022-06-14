using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VillagerBehaviour_Maze : MonoBehaviour, Interactable
{
    NavMeshAgent nav;

    enum VillagerState {Idle, Moving, Running, Hiding, Hidden, Captured};
    [SerializeField]
    VillagerState currentState;
    [SerializeField]
    VillagerState startingState;
    [SerializeField]
    GameObject[] lightPoles;
    GameObject closestLight;
    GameObject furthestLight;
    GameObject closestHidingSpot;
    [SerializeField, Range(0, 100)]
    float moveTimer, moveTimerReset, hideTimer, hideTimerReset;
    [SerializeField, Range(0, 100)]
    float lightDetectionRange, playerDetectionRange;
    int lightLayer;
    int playerLayer;
    int hidingLayer;
    
    Transform moveTarget;
    Transform runAwayTarget;


    [SerializeField]
    bool hasRandomStartTime = true;
    public bool isCaptured;
    public bool isHiding;


    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        currentState = startingState;
        moveTarget = NextMoveTarget();
        if (hasRandomStartTime)
        {
            moveTimer = Random.Range(2, 7);
        }
        lightLayer = LayerMask.NameToLayer("Light");
        playerLayer = LayerMask.NameToLayer("Player");
        hidingLayer = LayerMask.NameToLayer("HidingSpot");
    }

    void Update()
    {
        if (currentState == VillagerState.Moving || currentState == VillagerState.Idle)
        {
            moveTimer -= Time.deltaTime;
            if (nav.remainingDistance == 8f)
                currentState = VillagerState.Idle;

            if (moveTimer < 0)
            {
                nav.stoppingDistance = 8;
                nav.autoBraking = false;
                moveTarget = NextMoveTarget();
                Move();
                moveTimer = Random.Range(moveTimerReset / 2, moveTimerReset);

            }
        }

        if (currentState == VillagerState.Running)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer < 0)
            {
                currentState = VillagerState.Moving;
                hideTimer = hideTimerReset;
            }
        }

        if (currentState == VillagerState.Hiding || currentState == VillagerState.Hidden)
            isHiding = true;
        else
            isHiding = false;

        switch (currentState)
        {
            case VillagerState.Idle:
                Idle();
                break;
            case VillagerState.Hiding:
                Hiding();
                break;
            case VillagerState.Hidden:
                Hidden();
                break;
            case VillagerState.Captured:
                Captured();
                break;
        }
    }

    public void Interact()
    {
        if (currentState == VillagerState.Hidden)
        {
            Debug.Log("Captured");
            nav.enabled = false;
            var capturedParent = FindObjectOfType<TopDownMovement>().dragPoint.transform;
            transform.position = capturedParent.position;
            transform.parent = capturedParent;
            isCaptured = true;
            currentState = VillagerState.Captured;
        }
        else
        {
            currentState = VillagerState.Running;
        }
    }
    Transform NextMoveTarget()
    {
        if (CheckAllLights())
        {
            var potentialLight = lightPoles[Random.Range(0, lightPoles.Length)];
            if (!CheckIfLightIsOn(potentialLight))
            {
                potentialLight = NextMoveTarget().gameObject;
            }
            return potentialLight.transform;
        }
        currentState = VillagerState.Hiding;
        return null;
    }

    bool CheckIfLightIsOn(GameObject light)
    {
        if (light.GetComponentInChildren<Light>().enabled)
            return true;

        return false;
    }
    bool CheckAllLights()
    {
        for(int i = 0; i < lightPoles.Length - 1; i++)
        {
            if (lightPoles[i].GetComponentInChildren<Light>().enabled)
                return true;
        }
        return false;
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
        }
        else
        {
            Debug.Log("There is no light in the given radius");
        }
        return null;
    }
    void FindClosestHidingSpot()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, lightDetectionRange, 1 << hidingLayer);
        float minimumDistance = Mathf.Infinity;
        foreach (Collider collider in hitColliders)
        {
            bool isSpotOccupied = collider.gameObject.GetComponent<HidingSpotBehaviour>().isOccupied;
            if (!isSpotOccupied)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    closestHidingSpot = collider.gameObject;
                }
            }
        }
        if (closestHidingSpot != null)
        {
            
        }
        else
        {
            
        }
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
        currentState = VillagerState.Moving;
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
        currentState = VillagerState.Hiding;
        nav.stoppingDistance = 0.5f;
        nav.autoBraking = true;
    }

    void Hiding()
    {
        FindClosestHidingSpot();
        if (nav.enabled)
            nav.SetDestination(closestHidingSpot.transform.position);
        //Debug.Log(gameObject.name + " is going to hide at " + closestHidingSpot.name);
    }

    void Hidden()
    {
        //Debug.Log(gameObject.name + " is hiding at " + closestHidingSpot.gameObject.name);
        hideTimer -= Time.deltaTime;
        if (hideTimer < 0)
        {
            isHiding = false;
            currentState = VillagerState.Moving;
            hideTimer = hideTimerReset;
        }
    }

    void ComeOut()
    {
        hideTimer = 0;
        moveTimer = 1;
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
        if (CheckAllLights() && nav.enabled)
        {
            nav.SetDestination(FindFurthestLight().transform.position);
            currentState = VillagerState.Running;
        }
        else
            currentState = VillagerState.Hiding;
    }

    void Captured()
    {
        //Do something
    }

    void Rescue()
    {
        isHiding = false;
        isCaptured = false;
        transform.parent = null;

        nav.enabled = true;
        currentState = VillagerState.Moving;
        hideTimer = 0;
        moveTimer = 1; 
        RunAway();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (currentState != VillagerState.Hidden)
            {
                moveTimer = moveTimerReset;
                RunAway();
            }
        }
        if (other.gameObject.GetComponent<LightPoleBehaviour>())
        {
            other.gameObject.GetComponent<LightPoleBehaviour>().lightOffEvent += Hide; 
            other.gameObject.GetComponent<LightPoleBehaviour>().lightOnEvent += ComeOut;
        }

        if (other.gameObject.GetComponent<RepairManBehaviour>())
        {
            other.gameObject.GetComponent<RepairManBehaviour>().onRescueEvent += Rescue;
        }
        if (other.gameObject.tag == "HidingSpot")
        {
            if (currentState == VillagerState.Hiding)
            {
                isHiding = true;
                currentState = VillagerState.Hidden;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "LightPole")
        {
            other.gameObject.GetComponent<LightPoleBehaviour>().lightOffEvent -= Hide;
            other.gameObject.GetComponent<LightPoleBehaviour>().lightOnEvent -= ComeOut;
        }
        if (other.gameObject.GetComponent<RepairManBehaviour>())
        {
            other.gameObject.GetComponent<RepairManBehaviour>().onRescueEvent -= Rescue;
        }
    }
}
