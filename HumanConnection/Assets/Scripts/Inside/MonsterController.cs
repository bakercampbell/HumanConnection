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
    
    private Animator animator;
    private int monsterSprint, monsterAttack, monsterReturn;
    private Vector3 resetPos;
    private Vector3 offScreen;
    
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
        monsterReturn = Animator.StringToHash("Reset");
        
        resetPos = new Vector3(42, 0, 0);
        offScreen = new Vector3(60, 0, 0);
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
                monsterScriptableObject.health -= 10;
            }
        
    }

        private void BackToStart()
    {
            animator.Play(monsterReturn);
            
           
            {
                transform.DOMoveZ(0, 1f).OnComplete(() =>
              {
                  transform.DOMoveZ(8, .5f).OnComplete(() =>
                  {
                      transform.DOMoveY(-4, .5f).OnComplete(() =>
                      {
                          transform.DOMoveX(60, 3).OnComplete(() =>
                          {
                              transform.DOMove(offScreen, .5f).OnComplete(() =>
                              {
                                  transform.DOMove(resetPos, 1).OnComplete(() =>
                                  {
                                      transform.DOMove(resetPos, 1).OnComplete(() =>
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
            }
            
    }
}

    }