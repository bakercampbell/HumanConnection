using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace baker { 
public class MonsterController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private MonsterScriptableObject monsterScriptableObject;
    [SerializeField] private WaitForSeconds waitForSeconds = new(5);


        //[SerializeField] private float inRange = 30f;
        //[SerializeField] private float inSight;
    private Animation anim;
    private SegmentController segmentController;
    private Animator animator;
    private int monsterSprint, monsterAttack, monsterReturn, monsterWasHit;
    private Vector3 resetPos;
    private Vector3 offScreen;
    private bool wasHit = false;
    private bool tempInvincible;
    private bool healthZero = false;
    
    private void Start()
    {
        StartCoroutine(MonsterMoveToward());
            monsterScriptableObject.health = 100;


    }
    
    private void Awake()
    {
        anim = GetComponent<Animation>();
        animator = GetComponent<Animator>();
        monsterAttack = Animator.StringToHash("wham");
        monsterSprint = Animator.StringToHash("Walk");
        monsterReturn = Animator.StringToHash("Return");
        monsterWasHit = Animator.StringToHash("Was Hit");
        
        resetPos = new Vector3(47f, 9f, 0f);
        offScreen = new Vector3(45f, 10f, 2.5f);
    }

   



    IEnumerator MonsterMoveToward()
    {
        animator.Play(monsterSprint);
        while (Vector3.Distance(transform.position, target.transform.position) > monsterScriptableObject.monsterAttackType.attackRange) 
        {
            Vector3 destination = Vector3.MoveTowards(transform.position, target.transform.position, monsterScriptableObject.speed * Time.deltaTime);
            destination.y = transform.position.y;
            transform.position = destination;
            transform.LookAt(target.transform.position);
            yield return null;
        }
        StartCoroutine(MonsterAttack());
    }

    IEnumerator MonsterAttack()
    {
        animator.Play(monsterAttack);
        if (wasHit == true)
            {
                anim.PlayQueued("Double Wham");
            }
        yield return waitForSeconds;
        StartCoroutine(MonsterMoveToward());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Reset Trigger"))
            {
                BackToStart();
            }
         else if (other.CompareTag("Bullet"))
            {
                
                Hit();

            }
        
    }
        private void Hit()
        {
            if (tempInvincible != false) return;
            if (tempInvincible == false)
            {
                monsterScriptableObject.health -= 25;
                tempInvincible =
                anim.PlayQueued("Was Hit", QueueMode.CompleteOthers);
                anim.PlayQueued("wham", QueueMode.CompleteOthers);
                anim.PlayQueued("Return", QueueMode.CompleteOthers);
                wasHit = true;
            }

            

        }
        private void BackToStart()
    {
            animator.Play(monsterReturn);
            
           
            {
                transform.DOMoveY(1, .5f).OnComplete(() =>
              {
                  transform.DOMoveZ(3.19f, .5f).OnComplete(() =>
                  {
                      transform.DOMoveY(3f, .25f).OnComplete(() =>
                      {
                          transform.DOMove(offScreen, .75f).OnComplete(() =>
                          {
                              transform.DOMove(resetPos, .5f).OnComplete(() =>
                              {
                                  transform.DOMove(resetPos, 1).OnComplete(() =>
                                  {
                                      transform.DOMoveY(1.6f, .5f).OnComplete(() =>
                                      {
                                          transform.DOMoveY(1.6f, 1f).OnComplete(() =>
                                          {
                                              Debug.Log("start move");
                                              StartCoroutine(MonsterMoveToward());
                                          });
                                      });
                                  });
                              });
                          });
                      });
                  });
              });
            }
            
    }
}

    }