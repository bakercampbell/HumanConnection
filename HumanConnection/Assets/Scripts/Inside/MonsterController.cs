using System.Collections;
using UnityEngine;
using DG.Tweening;


public class MonsterController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private MonsterScriptableObject monsterScriptableObject;
    [SerializeField] private WaitForSeconds waitForSeconds = new(5);
    [SerializeField] private float attackRange;
    [SerializeField] private float chargeSpeed = 10;

    private Animator animator;
    private int monsterSprint, monsterAttack, monsterDouble, monsterWasHit, monsterWait, monsterIdle;



    private AudioSource monsterIsHit;
    private bool wasHit = false;
    private bool tempInvincible = false;

    public int health;
    
    public bool behindBoss { get; set; }


    private void Start()
    {
        attackRange = monsterScriptableObject.monsterAttackType.attackRange;
        StartCoroutine(MonsterMoveToward());
        monsterScriptableObject.health = 100;
        monsterScriptableObject.speed = 1.25f;
    }

    private void Awake()
    {
        monsterIsHit = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        monsterAttack = Animator.StringToHash("wham");
        monsterSprint = Animator.StringToHash("Walk");        
        monsterWasHit = Animator.StringToHash("Was Hit");
        monsterWait = Animator.StringToHash("Stand Up");
        monsterIdle = Animator.StringToHash("Idle");
        monsterDouble = Animator.StringToHash("Double Wham");



    }
    private void Update()
    {


        MonsterDown();

        if (transform.position.y < .5f)
        {
            transform.Translate(new Vector3(transform.position.x, .5f, transform.position.z));
        }

        health = monsterScriptableObject.health;

        


        if (monsterScriptableObject.health <= 0)
        {
            StopAllCoroutines();
            


        }
    }

    private void LateUpdate()
    {
        if (monsterScriptableObject.health <= 0)
        StartCoroutine(MonsterDown());
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

        attackRange = 0;
        animator.Play(monsterAttack);
        StartCoroutine(StandUp());
        yield return new WaitForSeconds(chargeSpeed);
        StartCoroutine(MonsterMoveToward());
        attackRange = 2.5f;
    }

    IEnumerator StandUp()
    {
        Debug.Log("gettin up...");
        yield return new WaitForSeconds(chargeSpeed);
        animator.Play(monsterWait);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Bullet") && monsterScriptableObject.health > 1)
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
            monsterIsHit.Play();
            monsterScriptableObject.health -= 15;
            tempInvincible = true;
            animator.Play(monsterWasHit);
            wasHit = true;

            StartCoroutine(NormalAttack());
            StartCoroutine(InvincibleCooldown());
        }
        else if (!tempInvincible && wasHit)
        {
            monsterIsHit.Play();
            monsterScriptableObject.health -= 15;            
            tempInvincible = true;
            StartCoroutine(DoubleAttack());
            StartCoroutine(InvincibleCooldown());
        }

    }

    IEnumerator MonsterDown()
    {
        animator.Play(monsterIdle);
        monsterScriptableObject.speed = 0;
        monsterScriptableObject.health += 125;
        yield return new WaitForSeconds(7.5f);        
        monsterScriptableObject.speed = 1.25f;
        StartCoroutine(MonsterMoveToward());
    }



    IEnumerator InvincibleCooldown()
    {
        Debug.Log("He's Invincible!!!");
        yield return new WaitForSeconds(4);
        tempInvincible = false;
        Debug.Log("Ok you can hit him again");
    }

    IEnumerator NormalAttack()
    {
        Debug.Log("Oo he mad..");
        animator.Play(monsterAttack);
        yield return new WaitForSeconds(chargeSpeed);
        StartCoroutine(MonsterMoveToward());
    }

    IEnumerator DoubleAttack()
    {
        Debug.Log("OOOOoo he REAL mad!");
        animator.Play(monsterAttack);
        yield return new WaitForSeconds(chargeSpeed);
        animator.Play(monsterDouble);
        StartCoroutine(MonsterMoveToward());
    }



}

