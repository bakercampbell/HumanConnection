using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RepairManBehaviour : MonoBehaviour
{
    enum RepairManState {Patrolling, Checking, OnTheWayToRepair, Repairing, Saving}
    [SerializeField]
    RepairManState currentState;
    NavMeshAgent nav;

    [SerializeField]
    GameObject[] lightPoles;
    [SerializeField]
    List<GameObject> lightsToRepair = new List<GameObject>();
    GameObject currentLightTarget;
    GameObject prevLightTarget;
    GameObject nextLightTarget;
    int lightTarget = 0;
    int repairTarget = 0;

    [SerializeField, Range(0, 30)]
    float checkingTimer, checkingTimerReset, repairTimer, repairTimerReset, saveTimer, saveTimerReset;


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
                SaveVillager();
                break;
        }

    }

    Vector3 NextLight()
    {
        if (lightTarget > lightPoles.Length - 1)
        {
            lightTarget = 0;
        }
        prevLightTarget = currentLightTarget;
        currentLightTarget = lightPoles[lightTarget];
        return lightPoles[lightTarget].transform.position;
    }

    void Checking()
    {
        //Animation, look around lightPole
        checkingTimer -= Time.deltaTime;
        transform.LookAt(currentLightTarget.transform.position);
        if (checkingTimer < 0)
        {
            lightTarget++;
            nav.SetDestination(NextLight());
            currentState = RepairManState.Patrolling;
            checkingTimer = checkingTimerReset;
        }
    }

    void Patrol()
    {
        if (nav.remainingDistance < 4f)
            currentState = RepairManState.Checking;
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

    }
    bool CheckAllLights()
    {
        for (int i = 0; i < lightPoles.Length; i++)
        {
            if (!lightPoles[i].GetComponentInChildren<Light>().enabled)
            {
                lightsToRepair.Add(lightPoles[i]);
            }

        }
        if (lightsToRepair.Count == 0)
            return true;
        else
            return false;
    }

    private void OnTriggerStay(Collider other)
    {
        var potentialRepair = other.gameObject.GetComponentInChildren<LightPoleBehaviour>();
        if (potentialRepair)
        {
            if (potentialRepair.lightGlobe.enabled == false)
            {
                currentState = RepairManState.Repairing;
            }
                
        }
    }
    private void OnDisable()
    {
        foreach (GameObject light in lightPoles)
        {
            light.GetComponentInChildren<LightPoleBehaviour>().lightOffEvent -= OnLightsOut;
        }
    }
}
