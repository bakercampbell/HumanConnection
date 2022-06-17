using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace baker 
{
public class MonsterController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private MonsterScriptableObject monsterScriptableObject;
    [SerializeField] private WaitForSeconds waitForSeconds = new(1);


    //[SerializeField] private float inRange = 30f;
    //[SerializeField] private float inSight;
        
    private Animation anim;
    private SegmentController segmentController;
    private PlayerController1 playerController;
    private Animator animator;
    private int monsterSprint, monsterAttack, monsterReturn, monsterWasHit;
    private Vector3 resetPos;
    private Vector3 offScreen;
    private int timesAttacked = 0;
    private bool wasHit = false;
    private bool tempInvincible;
    private bool healthZero = false;

    public bool behindBoss { get; set; }

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


    private void Update()
    {
        if(behindBoss)
        {
            anim.PlayQueued("Return", QueueMode.CompleteOthers);
        }

        if (monsterScriptableObject.health <= 0)
        {

        }        
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

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reset Trigger"))
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
        if (tempInvincible) return;
        if (wasHit)
        {
            anim.PlayQueued("Double Wham");
            timesAttacked += 1;
        }

        else if (!tempInvincible && !wasHit)
        {
            monsterScriptableObject.health -= 5;
            tempInvincible = true;
            anim.PlayQueued("Was Hit", QueueMode.CompleteOthers);
            anim.PlayQueued("wham", QueueMode.CompleteOthers);
            anim.PlayQueued("Stand Up", QueueMode.CompleteOthers);
            anim.PlayQueued("Return", QueueMode.CompleteOthers);
            wasHit = true;
            timesAttacked += 1;
        }
        else if (!tempInvincible && wasHit)
        {
            monsterScriptableObject.health -= 5;
            tempInvincible = true;
            anim.PlayQueued("Was Hit", QueueMode.CompleteOthers);
            anim.PlayQueued("wham", QueueMode.CompleteOthers);
            anim.PlayQueued("Double Wham", QueueMode.CompleteOthers);
            wasHit = !true;
            timesAttacked += 1;
            StartCoroutine(InvincibleCooldown());
        }

    }

    IEnumerator InvincibleCooldown()
    {
        tempInvincible = false;
        yield return new WaitForSeconds(3);
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