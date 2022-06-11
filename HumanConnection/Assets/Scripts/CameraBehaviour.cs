using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField, Range(1, 100)]
    float cameraHeight = 60;

    [SerializeField]
    Transform target;
    void Start()
    {
        transform.position = new Vector3(target.position.x, cameraHeight, target.position.z);
    }

    void Update()
    {
        transform.position = new Vector3(target.position.x, cameraHeight, target.position.z);
    }
}
