using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera_Maze : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField, Range(1, 100)]
    float cameraHeight = 60;

    void Start()
    {
        
    }
    void Update()
    {
        transform.position = new Vector3(target.position.x, cameraHeight, target.position.z);
    }
}
