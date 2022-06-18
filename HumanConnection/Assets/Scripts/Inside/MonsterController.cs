using System.Collections;
using UnityEngine;
using DG.Tweening;


public class MonsterController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private MonsterScriptableObject monsterScriptableObject;
    [SerializeField] private WaitForSeconds waitForSeconds = new(2);
    [SerializeField] private float chargeSpeed = 10;

    private Animator animator;
    private int monsterSprint, monsterAttack, monsterDouble, monsterReturn, monsterWasHit, monsterWait, monsterIdle, monsterCharge;

    GameObject segment;
    SegmentController segmentController;


    private bool wasHit = false;
    private bool tempInvincible = false;

    public int health;
    
    public bool behindBoss { get; set; }
    public bool isDefeated { get; set; }

    private void Start()
    {
        
        StartCoroutine(MonsterMoveToward());
        monsterScriptableObject.health = 100;
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
        monsterCharge = Animator.StringToHash("Charge");
        segment = GameObject.FindGameObjectWithTag("Segment");
        segmentController = segment.GetComponent<SegmentController>();

    }
    private void Update()
    {
        if(behindBoss)
        {
                BackToStart();
        }


            MonsterDown();
        if(transform.position.y > 1.5 && transform.position.x < 40 && transform.position.z == 0)
        {
            BackToStart();
        }
        else if (transform.position.y < .292f)
        {
            transform.Translate(new Vector3(transform.position.x, .292f, transform.position.z));
        }

        health = monsterScriptableObject.health;

        

        if (monsterScriptableObject.health <= 0)
        {

            StartCoroutine(MonsterDown());


        }
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
        Debug.Log("gettin up...");
        yield return new WaitForSeconds(1);
        animator.Play(monsterWait);
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

    IEnumerator MonsterDown()
    {
        animator.Play(monsterIdle);
        monsterScriptableObject.health += 125;
        yield return new WaitForSeconds(7.5f);
        
        BackToStart();
    }

    private void Charge()
    {
        segmentController.charging = true;
        Vector3 charge = Vector3.MoveTowards(transform.position, target.transform.position, monsterScriptableObject.speed * Time.deltaTime);

        transform.position = charge;
        transform.LookAt(target.transform.position);

        Return();
    }

    IEnumerator InvincibleCooldown()
    {
        Debug.Log("He's Invincible!!!");
        yield return new WaitForSeconds(3);
        tempInvincible = false;
        Debug.Log("Ok you can hit him again");
    }

    IEnumerator NormalAttack()
    {
        Debug.Log("Oo he mad..");
        yield return new WaitForSeconds(2);
        animator.Play(monsterAttack);
    }

    IEnumerator DoubleAttack()
    {
        Debug.Log("OOOOoo he REAL mad!");
        yield return new WaitForSeconds(2);
        animator.Play(monsterDouble);
    }




    private void BackToStart()
    {
        Vector3 reset = new Vector3(47, .3f, 0);
        animator.Play(monsterCharge);
        
            transform.DOMove(reset, 3f).OnComplete(() =>
            {
                Charge();
            });
        
    }

    private void Return()
    {
        Vector3 reset = new Vector3(47, 0, 0);
        animator.Play(monsterCharge);

        transform.DOMove(reset, 3f);
    }
}

