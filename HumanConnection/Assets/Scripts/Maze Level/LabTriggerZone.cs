using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabTriggerZone : MonoBehaviour
{
    public delegate void OnVillagerHarvested();
    public event OnVillagerHarvested onHarvestEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NPC")
        {
            other.gameObject.transform.SetParent(null);
            other.gameObject.SetActive(false);
            var player = FindObjectOfType<TopDownMovement>();
            player.isCarrying = false;
            onHarvestEvent?.Invoke();
        }
    }
}
