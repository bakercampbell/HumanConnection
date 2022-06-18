using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentController : MonoBehaviour
{
    GameObject monster;
    private MonsterController monsterController;
    [SerializeField] private Animator animator;
    private int monsterFlail, segmentIdle, segmentCharge;
    private float delay;
    public bool charging { get; set; }


    private void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
        monsterController = monster.GetComponent<MonsterController>();
        delay = Random.Range(0f, 2f);
        animator = GetComponent<Animator>();
        monsterFlail = Animator.StringToHash("Flailing");
        segmentIdle = Animator.StringToHash("Segment Idle");
        segmentCharge = Animator.StringToHash("SegmentCharge");
        StartCoroutine(DelayFlail());
    }



    private void Update()
    {
        if (monsterController.health <= 0)
        {
           
            StartCoroutine(DownTime());
        }
        if (charging == true)
        {
            StartCoroutine(Charge());
        }
    }

    IEnumerator DelayFlail()
    {
        Debug.Log("Iss gone flail");
        
        yield return new WaitForSeconds(delay);
        animator.Play(monsterFlail);
    }

    IEnumerator DownTime()
    {
        Debug.Log("Segments Are Vulnerable");
        animator.Play(segmentIdle);
        yield return new WaitForSeconds(7.5f);
        Debug.Log("Good job! But he's recharging!");
        animator.Play(segmentCharge);
        
    }

    IEnumerator Charge()
    {
        animator.Play(segmentCharge);
        yield return !charging;
        StartCoroutine(DelayFlail());
    }
}
