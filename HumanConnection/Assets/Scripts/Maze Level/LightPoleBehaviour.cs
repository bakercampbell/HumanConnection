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
    public bool isOn = true;

    public void Interact()
    {
        lightGlobe.enabled = false;
        OnLightOff();
    }

    public void OnLightOff()
    {
        isOn = false;
        lightOffEvent?.Invoke();
    }

    public void OnLightOn()
    {
        isOn = true;
        lightOnEvent?.Invoke();
    }

}
