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

    public void OutlineOn()
    {
        foreach (Outline outline in outlines)
        {
            outline.enabled = true;
        }
    }

    public void OutlineOff()
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
            OutlineOn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Outline")
        {
            OutlineOff();
        }
    }
}
