using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Maze : MonoBehaviour
{
    [SerializeField, Range(0,100)]
    float range, rangeReset;

    Vector3 startPos;

    private void OnEnable()
    {
        range = rangeReset;
    }


    void Update()
    {
        range -= Time.deltaTime;
        if (range <= 0)
            gameObject.SetActive(false);

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        gameObject.SendMessage("GetHit", SendMessageOptions.DontRequireReceiver);
        gameObject.SetActive(false);

    }

    private void OnDisable()
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }


}
