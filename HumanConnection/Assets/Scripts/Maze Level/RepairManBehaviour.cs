using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RepairManBehaviour : MonoBehaviour
{
    enum RepairManState {Patrolling, Checking, OnTheWayToRepair, Repairing, Saving, Stunned}
    [SerializeField]
    RepairManState currentState;
    [SerializeField]
    RepairManState prevState;

    public delegate void OnRescueDelegate();
    public event OnRescueDelegate onRescueEvent;
    
    NavMeshAgent nav;

    [SerializeField]
    GameObject[] lightPoles;
    [SerializeField]
    List<GameObject> lightsToRepair = new List<GameObject>();
    GameObject currentLightTarget;
    GameObject prevLightTarget;
    GameObject characterTarget;
    int lightTarget = 0;
    int repairTarget = 0;

    [SerializeField, Range(0, 30)]
    float checkingTimer, checkingTimerReset, repairTimer, repairTimerReset, saveTimer, saveTimerReset, stunTimer, stunTimerReset;
    [SerializeField]
    LayerMask selfMask;


    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        foreach (GameObject light in lightPoles)
        {
            light.GetComponentInChildren<LightPoleBehaviour>().lightOffEvent += OnLightsOut;
        }
        currentLightTarget = lightPoles[lightTarget];
        
    }

    void OnLightsOut()
    {
        if (!CheckAllLights())
        {
            currentState = RepairManState.OnTheWayToRepair;
            GoToRepair(lightsToRepair[repairTarget].transform.position);
        }
        else
            currentState = RepairManState.Patrolling;

    }

    void Update()
    {
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
        if (currentState == RepairManState.Checking)
        {
                checkingTimer -= Time.deltaTime;
                transform.LookAt(currentLightTarget.transform.position);
                if (checkingTimer < 0)
                {
                    currentState = RepairManState.Patrolling;
                    lightTarget++;
                    nav.SetDestination(NextLight());
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
        }
    }

    void GoToRepair(Vector3 lightToRepair)
    {
        nav.SetDestination(lightToRepair);
    }
    void Repair()
    {

        repairTimer -= Time.deltaTime;
        if (lightsToRepair[repairTarget] != null)
            transform.LookAt(lightsToRepair[repairTarget].transform.position);

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

    void SaveVillager()
    {
        Debug.Log(CheckLineOfSight());
        if (CheckLineOfSight())
        {
            saveTimer -= Time.deltaTime;
            //prevLightTarget = currentLightTarget;

            Debug.Log("I've come to save the day!");

            nav.isStopped = true;
            transform.LookAt(characterTarget.transform.position);

            if (saveTimer < 0)
            {

                onRescueEvent?.Invoke();
                CarryOn();

            }
        }
        else
            CarryOn();
    }

    void CapturePlayer()
    {
        if (CheckLineOfSight())
        {
            saveTimer -= Time.deltaTime;
            //prevLightTarget = currentLightTarget;

            Debug.Log("Stop right there criminal scum!");
            nav.SetDestination(characterTarget.transform.position);
            if (nav.remainingDistance < 2f)
                nav.isStopped = true;
            transform.LookAt(characterTarget.transform.position);
            characterTarget.GetComponent<TopDownMovement>()?.Captured();
            if (saveTimer < 0)
            {

                CarryOn();

            }
        }
        else
        {
            characterTarget.GetComponent<TopDownMovement>()?.Escaped();
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
        Vector3 dir = characterTarget.transform.position - transform.position;
        Debug.DrawRay(transform.position, dir, Color.red, .1f);
        if (Physics.Raycast(transform.position, dir.normalized, out hit, Mathf.Infinity))
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
        stunTimer -= Time.deltaTime;
        nav.isStopped = true;
        characterTarget.GetComponent<TopDownMovement>()?.Escaped();
        if (stunTimer <= 0)
        {
            CarryOn();
        }

    }
    bool CheckAllLights()
    {
        for (int i = 0; i < lightPoles.Length; i++)
        {
            if (!lightPoles[i].GetComponentInChildren<Light>().enabled)
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
}
