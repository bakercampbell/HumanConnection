using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VillagerBehaviour_Maze : MonoBehaviour
{
    NavMeshAgent nav;

    enum VillagerState {Idle, Moving, Hiding};
    VillagerState currentState;
    [SerializeField]
    GameObject[] lightPoles;
    [SerializeField, Range(0, 100)]
    float moveTimer, moveTimerReset;
    
    Vector3 moveTarget;

    [SerializeField]
    bool hasRandomStartTime = true;


    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        moveTarget = NextMoveTarget();
        if (hasRandomStartTime)
        {
            moveTimer = Random.Range(2, 7);
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveTimer -= Time.deltaTime;

        if (moveTimer < 0)
        {
            currentState = VillagerState.Moving;
            moveTarget = NextMoveTarget();
            moveTimer = moveTimerReset;

        }

        switch (currentState)
        {
            case VillagerState.Moving:
                Move();
                break;
        }
    }

    Vector3 NextMoveTarget()
    {
        return lightPoles[Random.Range(0, lightPoles.Length)].transform.position;
    }

    void Move()
    {
        nav.SetDestination(moveTarget);
    }
}
