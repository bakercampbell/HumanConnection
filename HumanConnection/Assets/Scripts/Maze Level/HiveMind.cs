using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveMind : MonoBehaviour
{
    public GameObject swarmTarget;
    void Start()
    {
        
    }

    public void OnSwarm()
    {
        gameObject.BroadcastMessage("Swarm", swarmTarget ,SendMessageOptions.DontRequireReceiver);
    }

    void Swarm()
    {

    }

}
