using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineHandler : MonoBehaviour
{
    [SerializeField]
    Outline[] outlines;

    private void Start()
    {
        foreach (Outline outline in outlines)
        {
            outline.enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Outline")
        {
            foreach (Outline outline in outlines)
            {
                outline.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Outline")
        {
            foreach (Outline outline in outlines)
            {
                outline.enabled = false;
            }
        }
    }
}
