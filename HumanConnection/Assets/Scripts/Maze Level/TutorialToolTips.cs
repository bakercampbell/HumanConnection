using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;


public class TutorialToolTips : MonoBehaviour
{
    public delegate void OnCamera();
    public event OnCamera onCameraEvent;

    [SerializeField]
    GameObject toolTipPanel;
    [SerializeField]
    TMP_Text textBox;

    [SerializeField]
    TopDownMovement player;
    [SerializeField]
    GameObject lightPole;
    bool isFirstLight, isLightVisible;
    [SerializeField]
    GameObject AmmoBar, VillagerBar;
    bool isFirstAmmoBar, isAmmoBarVisible, isFirstVillagerBar, isVillagerBarVisible;
    [SerializeField]
    RepairManBehaviour[] repairMen;
    GameObject onScreenRepairMan;
    [SerializeField]
    GameObject tutorialRepairMan;
    bool isFirstRepairMan, isRepairManVisible;
    [SerializeField]
    GameObject villager;
    bool isFirstVillager, isVillagerVisible;

    [SerializeField]
    float delayTimer, delayTimerReset, threshold;
    void Start()
    {
        toolTipPanel.SetActive(false);
    }



    void Update()
    {
        var playerPos = player.transform.position;
        if (!isFirstLight && lightPole.GetComponentInChildren<LightPoleBehaviour>().isVisible)
        {
            if (Vector3.Distance(playerPos, lightPole.transform.position) < threshold)
            {
                delayTimer -= Time.deltaTime;
                if (delayTimer <= 0)
                {
                    if (!player.isPaused) player.OnPause();
                    toolTipPanel.SetActive(true);
                    textBox.text = "This is a test";
                    isFirstLight = true;
                    delayTimer = delayTimerReset;
                }
            }
        }

        if (!isFirstVillager && Vector3.Distance(playerPos, villager.transform.position) < threshold)
        {
            if (!player.isPaused) player.OnPause();
            toolTipPanel.SetActive(true);
            textBox.text = "Villager test.";
            tutorialRepairMan.SetActive(true);
            villager.GetComponent<NavMeshAgent>().enabled = true;
            isFirstVillager = true;
        }

        if (!isFirstRepairMan && RepairManVisible())
        {
            if (Vector3.Distance(playerPos, onScreenRepairMan.transform.position) < threshold + 1)
            {
                delayTimer -= Time.deltaTime;
                if (delayTimer <= 0)
                {
                    if (!player.isPaused) player.OnPause();
                    toolTipPanel.SetActive(true);
                    textBox.text = "This is another test";
                    isFirstRepairMan = true;
                    delayTimer = delayTimerReset;
                }
            }
        }



        if (!isFirstVillagerBar && player.isCarrying)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0)
            {
                if (!player.isPaused) player.OnPause();
                toolTipPanel.SetActive(true);
                textBox.text = "First Victim";
                isFirstVillagerBar = true;
                delayTimer = delayTimerReset;
            }
        }
    }

    bool RepairManVisible()
    {
        foreach (RepairManBehaviour repairMan in repairMen)
        {
            if (repairMan.gameObject.GetComponent<RepairManBehaviour>().isVisible)
            {
                onScreenRepairMan = repairMan.gameObject;
                return true;
            }
        }

        return false;
    }
}
