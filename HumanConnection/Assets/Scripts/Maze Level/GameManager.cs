using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    TopDownMovement player;
    [SerializeField]
    LabTriggerZone labZone;
    [SerializeField]
    TMP_Text TM_text;
    public int villagersCollected;
    [SerializeField, Range(1,10)]
    int villagerGoal;
    [SerializeField, Range (1,10)]
    float completionDelay;
    string score;

    [SerializeField]
    Image ammoBarRight, ammoBarLeft;
    [SerializeField, Range(0, 1)]
    float ammoFillDuration;

    void Start()
    {
        
    }

    void Update()
    {
        score = villagersCollected + "/" + villagerGoal;
        TM_text.text = score;
        villagersCollected = labZone.villagersCollected;
        if (villagersCollected == villagerGoal)
        {
            completionDelay -= Time.deltaTime;
            if (completionDelay <= 0)
                SceneManager.LoadScene(0);
        }

        UpdateAmmoBar();

    }

    void UpdateAmmoBar()
    {
        var ammoFill = Mathf.Clamp(player.shotsLeft / player.shotsMax, 0, 1f);
        ammoBarRight.DOFillAmount(ammoFill, ammoFillDuration);
        ammoBarLeft.DOFillAmount(ammoFill, ammoFillDuration);
    }
}
