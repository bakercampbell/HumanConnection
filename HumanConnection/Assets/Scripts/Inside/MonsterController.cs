using System.Collections;
using UnityEngine;
using DG.Tweening;


public class MonsterController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private MonsterScriptableObject monsterScriptableObject;
    [SerializeField] private WaitForSeconds waitForSeconds = new(5);







    private Animator animator;
    private int monsterSprint, monsterAttack, monsterDouble, monsterReturn, monsterWasHit, monsterWait, monsterIdle;
    
    private Vector3 resetPos;
    private Vector3 offScreen;
    
    private bool wasHit = false;
    private bool tempInvincible = false;

    public int health;
    
    public bool behindBoss { get; set; }
    public bool isDefeated { get; set; }

    private void Start()
    {
        StartCoroutine(MonsterMoveToward());
        monsterScriptableObject.health = 100;


        health = monsterScriptableObject.health;


    }

    private void Awake()
    {
        
        animator = GetComponent<Animator>();
        monsterAttack = Animator.StringToHash("wham");
        monsterSprint = Animator.StringToHash("Walk");
        monsterReturn = Animator.StringToHash("Return");
        monsterWasHit = Animator.StringToHash("Was Hit");
        monsterWait = Animator.StringToHash("Stand Up");
        monsterIdle = Animator.StringToHash("Idle");
        monsterDouble = Animator.StringToHash("Double Wham");
        resetPos = new Vector3(47f, 9f, 0f);
        offScreen = new Vector3(45f, 10f, 2.5f);
    }


    private void Update()
    {
        if(behindBoss)
        {
                BackToStart();
        }


            MonsterDown();
               
    }


    IEnumerator MonsterMoveToward()
    {
        animator.Play(monsterSprint);
        while (Vector3.Distance(transform.position, target.transform.position) > monsterScriptableObject.monsterAttackType.attackRange && monsterScriptableObject.health > 0)
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
            StartCoroutine(StandUp());
        yield return waitForSeconds;
        StartCoroutine(MonsterMoveToward());
    }

        IEnumerator StandUp()
        {
            animator.Play(monsterWait);
            yield return new WaitForSeconds(1);
        }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reset Trigger"))
        {
            BackToStart();
        }
        else if (other.CompareTag("Bullet") && monsterScriptableObject.health > 1)
        {
            
            Hit();

        }

    }
    private void Hit()
    {
            if (tempInvincible)
            {
                return;
            }
            if (!tempInvincible && !wasHit)
            {
                monsterScriptableObject.health -= 35;
                tempInvincible = true;
                animator.Play(monsterWasHit);
                wasHit = true;

                StartCoroutine(NormalAttack());
                StartCoroutine(InvincibleCooldown());
            }
            else if (!tempInvincible && wasHit)
            {
                monsterScriptableObject.health -= 35;
                tempInvincible = true;
                animator.Play(monsterAttack);
                StartCoroutine(DoubleAttack());

                StartCoroutine(InvincibleCooldown());
            }

    }

    private void MonsterDown()
    {
        if (monsterScriptableObject.health <= 0)
        {

            animator.Play(monsterIdle);


        }
    }

    IEnumerator InvincibleCooldown()
    {
        tempInvincible = false;
        yield return new WaitForSeconds(3);
    }

    IEnumerator NormalAttack()
    {
        animator.Play(monsterAttack);
        yield return new WaitForSeconds(2);

    }

    IEnumerator DoubleAttack()
    {
        animator.Play(monsterDouble);
        yield return new WaitForSeconds(2);
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
                                        transform.DOMoveY(0f, 1f).OnComplete(() =>
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

