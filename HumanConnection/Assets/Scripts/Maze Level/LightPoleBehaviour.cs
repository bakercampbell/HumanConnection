using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPoleBehaviour : MonoBehaviour, Interactable
{
    public delegate void OnLightOffDelegate();
    public event OnLightOffDelegate lightOffEvent;
    public delegate void OnLightOnDelegate();
    public event OnLightOnDelegate lightOnEvent;
    public Light lightGlobe;

    public void Interact()
    {
        lightGlobe.enabled = false;
        OnLightOff();
    }

    public void OnLightOff()
    {
        lightOffEvent?.Invoke();
    }

    public void OnLightOn()
    {
        lightOnEvent?.Invoke();
    }

}
