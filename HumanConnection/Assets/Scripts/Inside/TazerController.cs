using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;




public class TazerController : MonoBehaviour
{



    private Animator animator;
    private Animation anim;

    private int tazerJab;
    private int tazerWalk;
    private int tazerIdle;

    //public bool walking { get; set; }
    //public bool attacking { get; set; }


    private void Start()
    {
        anim = GetComponent<Animation>();
        animator = GetComponent<Animator>();
        tazerJab = Animator.StringToHash("Tazer Jab");
        tazerWalk = Animator.StringToHash("TazerWalk");
        tazerIdle = Animator.StringToHash("Tazer Idle");
    }

    private void Awake()
    {
        animator.Play(tazerIdle);
    }

    //private void Update()
    //{
    //    if (walking == true)
    //    {
    //        animator.Play(tazerWalk);
    //    }
    //    else if (attacking == true)
    //    {
    //        animator.Play(tazerJab);
    //    }
    //    else anim.PlayQueued("Tazer Idle");
    //}
}
