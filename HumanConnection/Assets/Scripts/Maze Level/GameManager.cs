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
    Light[] scoreLights;
    [SerializeField]
    Color scoreLightcolor;

    [SerializeField]
    Image ammoBar, villagerBar, villagerOuterBar;
    [SerializeField, Range(0, 1)]
    float ammoFillDuration, villagerFillDuration, villagerBarFadeTime;

    void Start()
    {
        labZone.onHarvestEvent += UpdateLights;
    }

    void Update()
    {

        UpdateScore();
        UpdateAmmoBar();
        UpdateVillagerBar();

    }

    void UpdateScore()
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
    }
    void UpdateLights()
    {
        StartCoroutine(UpdateLightsCoRoute());
    }

    IEnumerator UpdateLightsCoRoute()
    {
        scoreLights[villagersCollected - 1].color = scoreLightcolor;
        yield return new WaitForSeconds(1f);
    }

    void UpdateAmmoBar()
    {
        var ammoFill = Mathf.Clamp(player.shotsLeft / player.shotsMax, 0, 1f);
        ammoBar.DOFillAmount(ammoFill, ammoFillDuration);
    }

    void UpdateVillagerBar()
    {
        if (player.isCarrying)
            villagerOuterBar.DOFade(1f, villagerBarFadeTime);
        else
            villagerOuterBar.DOFade(0f, villagerBarFadeTime);
        var villagerFill = Mathf.Clamp(player.swarmCounter / player.swarmLimit, 0, 1f);
        villagerBar.DOFillAmount(villagerFill, villagerFillDuration);
    }

    void BarJiggle(Image bar)
    {

    }
}
