using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentController : MonoBehaviour
{
    [SerializeField] private int health = 100;

    private Animator animator;
    private int monsterFlail;
    private float delay;

    private void Awake()
    {
        delay = Random.Range(0f, 1f);
        animator = GetComponent<Animator>();
        monsterFlail = Animator.StringToHash("Flailing");
        StartCoroutine (DelayFlail());        
    }

    private void Update()
    {
        if (health <= 0)
        {
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tazer"))
        {
            health -= 10;
        }
    }

    IEnumerator DelayFlail()
    {
        yield return new WaitForSeconds(delay);
        animator.Play(monsterFlail);
        
    }
}
