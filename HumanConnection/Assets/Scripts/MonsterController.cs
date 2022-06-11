using System.Collections;
using UnityEngine;

namespace baker { 
public class MonsterController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private MonsterScriptableObject monsterScriptableObject;
    [SerializeField] private WaitForSeconds waitForSeconds = new WaitForSeconds(3);
    [SerializeField] private float inRange = 30f;
    [SerializeField] private float inSight;
    
    private Animator animator;
    private int monsterIdle, monsterSprint, monsterAttack;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        monsterIdle = Animator.StringToHash("Idle");
        monsterAttack = Animator.StringToHash("MiningLoop");
        monsterSprint = Animator.StringToHash("Sprint");
    }

    private void Start()
    {
        StartCoroutine(MonsterMoveToward());
           
    }



    IEnumerator MonsterMoveToward()
    {
        animator.Play(monsterSprint);
        while (Vector3.Distance(transform.position, target.transform.position) > monsterScriptableObject.monsterAttackType.attackRange) 
        {
            Vector3 destination = Vector3.MoveTowards(transform.position, target.transform.position, monsterScriptableObject.speed * Time.deltaTime);
            destination.y = transform.position.y;
            transform.position = destination;
            transform.LookAt(target.transform);
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
}

}