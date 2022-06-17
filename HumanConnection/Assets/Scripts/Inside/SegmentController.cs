using System.Collections;

using UnityEngine;


public class SegmentController : MonoBehaviour
{

    GameObject monster;
    private MonsterController monsterController;
    private Animator animator;
    private int monsterFlail, segmentIdle;
    private float delay;



    private void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
        monsterController = monster.GetComponent<MonsterController>();
        delay = Random.Range(0f, 2f);
        animator = GetComponent<Animator>();
        monsterFlail = Animator.StringToHash("Flailing");
        segmentIdle = Animator.StringToHash("Segment Idle");

    }



    private void Awake()
    {
         
   
        StartCoroutine (DelayFlail());      
            
    }

    private void Update()
    {
        if (monsterController.health <= 0)
        {
            Debug.Log("Segments Are Vulnerable");
            animator.Play(segmentIdle);
            StartCoroutine(DownTime());
        }

    }

    IEnumerator DelayFlail()
    {
        Debug.Log("Iss gone flail");
        animator.Play(monsterFlail);
        yield return new WaitForSeconds(delay);
    }

    IEnumerator DownTime()
    {
        monsterController.health += 125;
        yield return new WaitForSeconds(7.5f);
    }
}
