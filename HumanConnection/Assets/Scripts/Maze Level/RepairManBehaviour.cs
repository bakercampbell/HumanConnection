using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class RepairManBehaviour : MonoBehaviour
{
    public delegate void OnCamera();
    public event OnCamera onCameraEvent;
    enum RepairManState {Patrolling, Checking, OnTheWayToRepair, Repairing, Saving, Stunned}
    [SerializeField]
    RepairManState currentState;
    [SerializeField]
    RepairManState prevState;

    public delegate void OnRescueDelegate();
    public event OnRescueDelegate onRescueEvent;
    
    NavMeshAgent nav;

    [SerializeField]
    GameObject body, head;

    [SerializeField]
    GameObject[] lightPoles;
    [SerializeField]
    List<GameObject> lightsToRepair = new List<GameObject>();
    GameObject currentLightTarget;
    GameObject prevLightTarget;
    GameObject characterTarget;
    int lightTarget = 0;
    int repairTarget = 0;
    [SerializeField, Range(0, 100)]
    float detectPlayerRange;
    [SerializeField, Range(0, 30)]
    float checkingTimer, checkingTimerReset, repairTimer, repairTimerReset, saveTimer, saveTimerReset, 
        stunTimer, stunTimerReset, captureDelayTimer, captureDelayTimerReset, emergencyOverrideTimer, emergencyOverrideTimerReset;
    [SerializeField]
    LayerMask selfMask;
    [SerializeField]
    GameObject particleSystem;
    public bool isVisible;


    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        foreach (GameObject light in lightPoles)
        {
            var repairableLight = light.GetComponentInChildren<LightPoleBehaviour>();
            if (repairableLight != null)
                repairableLight.lightOffEvent += OnLightsOut;
        }
        currentLightTarget = lightPoles[lightTarget];
    }

    void OnLightsOut()
    {
        if (!CheckAllLights() && lightsToRepair.Count > 0)
        {
            currentState = RepairManState.OnTheWayToRepair;
            GoToRepair(lightsToRepair[repairTarget].transform.position);
        }
        else
            currentState = RepairManState.Patrolling;

    }

    void Update()
    {
        if(nav.enabled && nav.velocity.magnitude == 0)
        {
            emergencyOverrideTimer -= Time.deltaTime;
            if (emergencyOverrideTimer <= 0)
            {
                nav.SetDestination(NextLight());
                lightsToRepair.Clear();
                currentState = RepairManState.Patrolling;
                emergencyOverrideTimer = emergencyOverrideTimerReset;
            }
        }
        if (lightTarget > lightPoles.Length - 1)
        {
            lightTarget = 0;
        }

        switch (currentState)
        {
            case RepairManState.Patrolling:
                Patrol();
                break;
            case RepairManState.Checking:
                Checking();
                break;
            case RepairManState.Repairing:
                Repair();
                break;
            case RepairManState.Saving:
                if (characterTarget.tag == "NPC")
                    SaveVillager();
                else if (characterTarget.tag == "Player")
                    CapturePlayer();
                break;
            case RepairManState.Stunned:
                Stunned();
                break;
        }

    }

    Vector3 NextLight()
    {
        if (lightTarget > lightPoles.Length - 1)
        {
            lightTarget = 0;
        }
        currentLightTarget = lightPoles[lightTarget];
        return lightPoles[lightTarget].transform.position;
    }

    void Checking()
    {
        //Animation, look around lightPole
        if (currentState == RepairManState.Checking && nav.velocity.magnitude < 1)
        {
            checkingTimer -= Time.deltaTime;
            head.transform.DOLookAt(currentLightTarget.transform.position, .5f);
            if (checkingTimer <= 0)
            {
                head.transform.DOLookAt(Vector3.forward, .5f);
                lightTarget++;
                nav.SetDestination(NextLight());
                currentState = RepairManState.Patrolling;
                checkingTimer = checkingTimerReset;
                    
            }
        }
    }

    void Patrol()
    {
        if (currentState == RepairManState.Patrolling)
        {
            if (nav.remainingDistance < 4f)
                currentState = RepairManState.Checking;
            emergencyOverrideTimer = emergencyOverrideTimerReset;
        }
    }

    void GoToRepair(Vector3 lightToRepair)
    {
        if (lightsToRepair.Count > 0)
            nav.SetDestination(lightToRepair);
        else
            nav.SetDestination(lightPoles[Random.Range(0, lightPoles.Length - 1)].transform.position);
    }
    void Repair()
    {


        if (lightsToRepair.Count > 0)
        {
            repairTimer -= Time.deltaTime;
            if (lightsToRepair[repairTarget] != null)
                head.transform.LookAt(lightsToRepair[repairTarget].transform.position);

            if (repairTimer < 0)
            {
                lightsToRepair[repairTarget].GetComponentInChildren<Light>().enabled = true;
                lightsToRepair[repairTarget].GetComponentInChildren<LightPoleBehaviour>().OnLightOn();
                repairTimer = repairTimerReset;
                lightsToRepair.RemoveAt(repairTarget);
                repairTarget++;
                if (repairTarget >= lightsToRepair.Count)
                    repairTarget = 0;
                OnLightsOut();
            }
        }
        else
            currentState = RepairManState.Checking;
    }

    void SaveVillager()
    {
        Debug.Log(CheckLineOfSight());
        if (CheckLineOfSight())
        {
            saveTimer -= Time.deltaTime;
            //prevLightTarget = currentLightTarget;

            Debug.Log("I've come to save the day!");

            nav.isStopped = true;
            head.transform.DOLookAt(characterTarget.transform.position, .5f);

            if (saveTimer < 0)
            {

                onRescueEvent?.Invoke();
                CarryOn();

            }
        }
        else
        {
            head.transform.DOLookAt(Vector3.forward, .5f);
            CarryOn();
        }
    }

    void CapturePlayer()
    {
        if (CheckLineOfSight())
        {
            captureDelayTimer -= Time.deltaTime;
            saveTimer -= Time.deltaTime;
            //prevLightTarget = currentLightTarget;

            Debug.Log("Stop right there criminal scum!");
            nav.SetDestination(characterTarget.transform.position);
            if (nav.remainingDistance < 3f)
            {
                nav.velocity = Vector3.zero;
                nav.isStopped = true;
            }
            head.transform.DOLookAt(characterTarget.transform.position, .5f);
            if (captureDelayTimer <= 0)
            {
                captureDelayTimer = captureDelayTimerReset;
                characterTarget.GetComponent<TopDownMovement>()?.Captured();
            }

                if (saveTimer < 0)
            {
                captureDelayTimer = captureDelayTimerReset;
                CarryOn();
                
            }
        }
        else
        {
            head.transform.DOLookAt(Vector3.forward, .5f);
            characterTarget.GetComponent<TopDownMovement>()?.Escaped();
            captureDelayTimer = captureDelayTimerReset;
            CarryOn();
        }
            
    }

    void CarryOn()
    {
        currentState = prevState;
        //currentLightTarget = prevLightTarget;
        nav.isStopped = false;
        saveTimer = saveTimerReset;
        stunTimer = stunTimerReset;
    }

    bool CheckLineOfSight()
    {
        RaycastHit hit;
        Vector3 dir = characterTarget.transform.position - head.transform.position;
        Debug.DrawRay(transform.position, dir, Color.red, .1f);
        if (Physics.Raycast(transform.position, dir.normalized, out hit, detectPlayerRange))
        {
            if (hit.transform.gameObject == characterTarget)
                return true;
        }
        return false;

    }

    void GetHit()
    {
        Debug.Log("I'm hit!");
        currentState = RepairManState.Stunned;
    }

    void Stunned()
    {
        Debug.Log("I can't move!");
        particleSystem.SetActive(true);
        stunTimer -= Time.deltaTime;
        nav.isStopped = true;
        characterTarget.GetComponent<TopDownMovement>()?.Escaped();
        if (stunTimer <= 0)
        {
            particleSystem.SetActive(false);
            CarryOn();
        }

    }
    bool CheckAllLights()
    {
        for (int i = 0; i < lightPoles.Length; i++)
        {
            if (lightPoles[i].GetComponentInChildren<Light>() && !lightPoles[i].GetComponentInChildren<Light>().enabled)
            {
                if (!lightsToRepair.Contains(lightPoles[i]))
                    lightsToRepair.Add(lightPoles[i]);

                    
            }

        }
        if (lightsToRepair.Count == 0)
            return true;
        else
            return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentState != RepairManState.Saving)
        {
            if (other.gameObject.GetComponent<VillagerBehaviour_Maze>())
            {
                if (other.gameObject.GetComponent<VillagerBehaviour_Maze>().isHiding || other.gameObject.GetComponent<VillagerBehaviour_Maze>().isCaptured)
                {
                    Debug.Log("Don't Panic!");
                    characterTarget = other.gameObject;
                    prevState = currentState;
                    currentState = RepairManState.Saving;
                    SaveVillager();
                }
            }


            if (other.gameObject.GetComponent<TopDownMovement>())
            {
                if (currentState != RepairManState.Saving && currentState != RepairManState.Stunned)
                {
                    Debug.Log("You there!");
                    characterTarget = other.gameObject;
                    prevState = currentState;
                    currentState = RepairManState.Saving;
                    CapturePlayer();
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        var potentialRepair = other.gameObject.GetComponentInChildren<LightPoleBehaviour>();
        if (potentialRepair)
        {
            if (potentialRepair.lightGlobe.enabled == false && nav.remainingDistance <= 4 && currentState != RepairManState.Saving)
            {
                currentState = RepairManState.Repairing;
            }
                
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            GetHit();
        }
    }
    private void OnDisable()
    {
        if (lightPoles != null)
        {
            foreach (GameObject light in lightPoles)
            {
                var tmp = light.GetComponentInChildren<LightPoleBehaviour>();
                if (tmp != null)
                    light.GetComponentInChildren<LightPoleBehaviour>().lightOffEvent -= OnLightsOut;
            }
        }
    }

    private void OnBecameVisible()
    {
        isVisible = true;
        onCameraEvent?.Invoke();
    }
}
