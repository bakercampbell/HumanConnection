using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TazerController : MonoBehaviour
{
    [SerializeField] private float wobble;

    private Vector3 still;
    private Rigidbody rb;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void Update()
    {
        if (rb.velocity.x != 0)
        {
            Sway();
        }
    }
    private void Sway()
    {
        Vector3 wibble = new(0, wobble, .2f);
        transform.DOLocalMove(wibble, .05f).OnComplete(() =>
        {
            transform.DOLocalMove(-wibble, -.2f);
        });
    }

}
