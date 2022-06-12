using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabTriggerZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NPC")
        {
            other.gameObject.transform.SetParent(null);
            other.gameObject.SetActive(false);
        }
    }
}
