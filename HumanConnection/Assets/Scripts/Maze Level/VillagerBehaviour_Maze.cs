using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VillagerBehaviour_Maze : MonoBehaviour, Interactable
{
    NavMeshAgent nav;

    [SerializeField]
    HiveMind hiveMind;
    enum VillagerState {Idle, Moving, Running, Hiding, Hidden, Captured, Stunned, Swarm};
    [SerializeField]
    VillagerState currentState;
    [SerializeField]
    VillagerState startingState;
    [SerializeField]
    GameObject[] lightPoles;
    GameObject closestLight;
    GameObject furthestLight;
    GameObject closestHidingSpot;
    GameObject swarmTarget;
    [SerializeField, Range(0, 100)]
    float moveTimer, moveTimerReset, hideTimer, hideTimerReset, stunTimer, stunTimerReset, swarmTimer, swarmTimerReset;
    [SerializeField, Range(0, 100)]
    float lightDetectionRange, playerDetectionRange, villagerDetectionRange;
    int lightLayer;
    int playerLayer;
    int hidingLayer;
    int villagerLayer;
    
    Transform moveTarget;
    Transform runAwayTarget;


    [SerializeField]
    bool hasRandomStartTime = true;
    public bool isCaptured;
    public bool isHiding;

    //These bools monitor the AI's safety
    public bool isInLight, isInCrowd, isProtected, isSafe;

    [SerializeField]
    Color outlineColor;//, hidingOutline;

    Animator anim;


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
        villagerLayer = LayerMask.NameToLayer("Interactable");
        anim = GetComponentInChildren<Animator>();
        
    }

    void Update()
    {
        if (DetectOthers())
            isInCrowd = true;
        else
            isInCrowd = false;

        AmISafe();

        if (currentState == VillagerState.Moving || currentState == VillagerState.Idle)
        {
            moveTimer -= Time.deltaTime;
            if (currentState == VillagerState.Moving && nav.remainingDistance < 8f)
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
            case VillagerState.Hidden:
                Hidden();
                break;
            case VillagerState.Stunned:
                Stunned();
                break;
            case VillagerState.Swarm:
                Swarm(swarmTarget);
                break;
        }
    }

    public void Interact()
    {
            Debug.Log("Captured");
            nav.enabled = false;
            var capturedParent = FindObjectOfType<TopDownMovement>();
            transform.position = capturedParent.dragPoint.transform.position;
            transform.parent = capturedParent.dragPoint.transform;
            capturedParent.isCarrying = true;
            isCaptured = true;
            anim.SetBool("isStunned", false);
            anim.SetBool("isHidden", true);
            currentState = VillagerState.Captured;
    }

    private void LateUpdate()
    {
        float moveSpeed = nav.velocity.magnitude;
        anim.SetFloat("MoveSpeed", moveSpeed);
    }

    public void AmISafe()
    {
        if (isInLight || isInCrowd || isProtected)
        {
            isSafe = true;
        }
        else
        {
            isSafe = false;
            Debug.Log("I'm so scared");
        }
    }

    /*void CheckOutline()
    {
        outlineTimer -= Time.deltaTime;
        if (outlineTimer <= 0)
            outline.enabled = false;
        else
            outline.enabled = true;
    }*/
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
    GameObject FindClosestHidingSpot()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, lightDetectionRange, 1 << hidingLayer);
        float minimumDistance = Mathf.Infinity;
        /*foreach (Collider collider in hitColliders)
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
        }*/
        closestHidingSpot = hitColliders[Random.Range(0, hitColliders.Length - 1)].gameObject;
        return closestHidingSpot;
    }

    bool DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, playerDetectionRange, 1 << playerLayer);
        foreach (Collider collider in hitColliders)
        {
            if (collider != null)
            {
//if (CheckLineOfSight(collider.gameObject))
                //{
                    Debug.Log("I see you, you sicko!");
                    return true;
                //}
               // else
                    //return false;
            }
        }
        return false;
    }

    bool DetectOthers()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, villagerDetectionRange, 1 << villagerLayer);
        if (hitColliders.Length > 1)
        {
            foreach (Collider collider in hitColliders)
            {
                if (collider != null)
                {
                    if (collider.gameObject.tag == "NPC")
                    {
                        if (Vector3.Distance(collider.gameObject.transform.position, transform.position) < villagerDetectionRange / 3)
                            return true;
                        else if (CheckLineOfSight(collider.gameObject))
                        {
                            Debug.Log("I feel safe in a crowd");
                            return true;
                        }
                        else
                            return false;
                    }
                }
            }
        }
        return false;
    }

    bool CheckLineOfSight(GameObject target)
    {
        RaycastHit hit;
        Vector3 dir = target.transform.position - transform.position;
        Debug.DrawRay(transform.position, dir, Color.red, .1f);
        if (Physics.Raycast(transform.position, dir.normalized, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject == target)
                return true;
        }
        return false;

    }

    void Move()
    {
        currentState = VillagerState.Moving;
        nav.speed = 6;
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
        isInLight = false;
        currentState = VillagerState.Hiding;
        nav.stoppingDistance = 0.5f;
        nav.autoBraking = true;
        Hiding();
    }

    void Hiding()
    {
        if (nav.enabled)
            nav.SetDestination(closestHidingSpot.transform.position);
    }

    void Hidden()
    {
        //Debug.Log(gameObject.name + " is hiding at " + closestHidingSpot.gameObject.name);
        anim.SetBool("isHidden", true);
        hideTimer -= Time.deltaTime;
        //outline.OutlineColor = hidingOutline;
        if (hideTimer < 0)
        {
            //outline.OutlineColor = outlineColor;
            isHiding = false;
            anim.SetBool("isHidden", false);
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
        nav.speed = 8;
        /*    runAwayTarget = NextMoveTarget();

        if (runAwayTarget.position != moveTarget.position)
        {
            nav.SetDestination(runAwayTarget.position);
        }
        else
            RunAway();
        */
        Debug.Log("I'm going home!");
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
        //swarmTimer = swarmTimerReset;
        //GetHit();
    }

    public void Rescue()
    {
        isHiding = false;
        isCaptured = false;
        anim.SetBool("isHidden", false);
        transform.parent = null;

        CarryOn();
    }
    void GetHit()
    {
        Debug.Log("Help!");
        if (currentState != VillagerState.Captured) currentState = VillagerState.Stunned;
        Stunned();
        /*hiveMind.swarmTarget = FindObjectOfType<TopDownMovement>().gameObject;
        if (hiveMind.swarmTarget != null)
        {
            hiveMind.swarmTarget.GetComponent<TopDownMovement>().isSwarmed = true;
            hiveMind.OnSwarm();

        }*/
    }

    void Stunned()
    {
        Debug.Log("You will fear the proletariat!");
        anim.SetBool("isStunned", true);
        nav.velocity = Vector3.zero;
        if (currentState != VillagerState.Captured) stunTimer -= Time.deltaTime;
        nav.isStopped = true;
        if (stunTimer <= 0)
        {
            stunTimer = stunTimerReset;
            if (currentState != VillagerState.Captured)
            {
                CarryOn();
                currentState = VillagerState.Running;
                RunAway();
            }
        }
    }

    void Swarm(GameObject target)
    {
        var player = target.GetComponent<TopDownMovement>();
        Debug.Log("I'm making a citizens arrest!");
        if (currentState == VillagerState.Idle || currentState == VillagerState.Moving || currentState == VillagerState.Swarm)
        {
            if (currentState != VillagerState.Captured)
            {
                anim.SetBool("isYelling", true);
                swarmTimer -= Time.deltaTime;
                nav.speed = 10;
                nav.stoppingDistance = 0;
                nav.radius = 1f;
                player.swarmCounter++;
                player.isSwarmed = true;

                if (nav.remainingDistance > 2)
                    nav.SetDestination(player.gameObject.transform.position);
                transform.LookAt(player.gameObject.transform.position);

                if (swarmTimer <= 0)
                {
                    swarmTimer = swarmTimerReset;
                    CarryOn();
                }
            }
        }
    }

    void CarryOn()
    {
        nav.enabled = true;
        nav.speed = 8;
        nav.stoppingDistance = 8;
        nav.radius = 2;
        nav.isStopped = false;
        anim.SetBool("isStunned", false);
        anim.SetBool("isYelling", false);
        anim.SetBool("isHidden", false);
        currentState = VillagerState.Running;
        RunAway();
        moveTimer = Random.Range(0f, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            GetHit();

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            /*if (currentState != VillagerState.Hidden)
            {
                moveTimer = moveTimerReset;
                RunAway();
            }*/
        }
        if (other.gameObject.tag == "LightPole")
        {
            var light = other.gameObject.GetComponentInParent<LightPoleBehaviour>();
            light.lightOffEvent += Hide; 
            light.lightOnEvent += ComeOut;
            if (light.isOn)
            {
                Debug.Log("I feel safe in the light");
                isInLight = true;
            }
        }

        if (other.gameObject.GetComponent<RepairManBehaviour>())
        {
            other.gameObject.GetComponent<RepairManBehaviour>().onRescueEvent += Rescue;
            isProtected = true;
            Debug.Log("I love a man in uniform");
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (currentState != VillagerState.Captured)
            {
                var player = other.gameObject.GetComponent<TopDownMovement>();
                if (player.isCarrying)
                {
                    Debug.Log("What are you doing with them?");
                    if (DetectPlayer() && currentState != VillagerState.Stunned)
                    {
                        Debug.Log(Vector3.Distance(transform.position, player.gameObject.transform.position));
                        swarmTarget = other.gameObject;
                        currentState = VillagerState.Swarm;
                        Swarm(other.gameObject);
                    }
                }
            }
        }

        if (other.gameObject.tag == "LightPole")
        {
            if (other.gameObject.GetComponentInParent<LightPoleBehaviour>().isOn)
            {
                isSafe = true;
            }
            else
            {
                isSafe = false;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "LightPole")
        {
            var light = other.gameObject.GetComponentInParent<LightPoleBehaviour>();
            light.lightOffEvent -= Hide;
            light.lightOnEvent -= ComeOut;
            if (light.isOn)
            {
                Debug.Log("I'm vulnerable");
                isInLight = false;
            }
        }
        if (other.gameObject.GetComponent<RepairManBehaviour>())
        {
            other.gameObject.GetComponent<RepairManBehaviour>().onRescueEvent -= Rescue;
            isProtected = false;
            Debug.Log("Leaving so soon?");
        }
    }
}
