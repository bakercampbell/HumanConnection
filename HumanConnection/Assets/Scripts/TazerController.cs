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
        if (rb.velocity != null)
        {
            Sway();
        }
    }
    private void Sway()
    {
        Debug.Log("TaySway");
        Vector3 wibble = new(0f, wobble, 0f);
        transform.DOMove(wibble, .5f * Time.deltaTime).OnComplete(() =>
        {
            transform.DOMove(-wibble, .5f * Time.deltaTime);
        });
    }

}
