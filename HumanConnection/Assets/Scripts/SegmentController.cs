using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentController : MonoBehaviour
{
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

    }

    IEnumerator DelayFlail()
    {
        yield return new WaitForSeconds(delay);
        animator.Play(monsterFlail);
        
    }
}
