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
    TMP_Text textBox, flavorTextBox;
    [SerializeField]
    Color flavorTextColor;

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
    GameObject tutorialRepairMan, tutorialVillager;
    bool isFirstRepairMan, isRepairManVisible;
    [SerializeField]
    GameObject villager;
    bool isFirstVillager, isVillagerVisible;

    [SerializeField]
    float delayTimer, delayTimerReset, threshold;
    void Start()
    {

    }



    void Update()
    {
        var playerPos = player.transform.position;
        if (!isFirstLight && lightPole.GetComponentInChildren<LightPoleBehaviour>().isVisible)
        {
            if (Vector3.Distance(playerPos, lightPole.transform.position) < threshold)
            {
                tutorialVillager.SetActive(true);
                delayTimer -= Time.deltaTime;
                if (delayTimer <= 0)
                {
                    if (!player.isPaused) player.OnPause();
                    toolTipPanel.SetActive(true);
                    flavorTextBox.color = flavorTextColor;
                    flavorTextBox.alignment = TextAlignmentOptions.Left;
                    flavorTextBox.text = "Blasted bright lights! I have a better idea...";
                    textBox.text = "Deactivate the lights to recharge your stun gun. The blue bar is your charge.";
                    isFirstLight = true;
                    delayTimer = delayTimerReset;
                }
            }
        }

        if (!isFirstVillager && Vector3.Distance(playerPos, villager.transform.position) < threshold)
        {
            if (!player.isPaused) player.OnPause();
            toolTipPanel.SetActive(true);
            flavorTextBox.text = "Don't run away! Let me bring you inside...";
            textBox.text = "Capture the citizens and bring them back to your lab.";
            tutorialRepairMan.transform.position = new Vector3(3, .75f, 35);
            villager.GetComponent<NavMeshAgent>().enabled = true;
            isFirstVillager = true;
        }

        if (!isFirstRepairMan && RepairManVisible())
        {
            if (Vector3.Distance(playerPos, onScreenRepairMan.transform.position) < threshold + 5)
            {
                delayTimer -= Time.deltaTime;
                if (delayTimer <= 0)
                {
                    if (!player.isPaused) player.OnPause();
                    toolTipPanel.SetActive(true);
                    flavorTextBox.text = "I wasn't doing anything, honest!";
                    textBox.text = "Sentry tanks are on the look out for you! Stun them to run away.";
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
                flavorTextBox.text = "Oof. You're a big one!";
                textBox.text = "The other citizens don't make kidnapping easy. If the yellow bar fills up, you'll drop him.";
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
