using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpotBehaviour : MonoBehaviour
{
    public bool isOccupied;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NPC")
            isOccupied = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "NPC")
            isOccupied = false;
    }
}
