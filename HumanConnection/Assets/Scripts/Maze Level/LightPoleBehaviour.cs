using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPoleBehaviour : MonoBehaviour, Interactable
{
    public delegate void OnLightOffDelegate();
    public event OnLightOffDelegate lightOffEvent;
    [SerializeField]
    Light lightGlobe;

    public void Interact()
    {
        lightGlobe.enabled = false;
        OnLightOff();
    }

    public void OnLightOff()
    {
            lightOffEvent?.Invoke();
    }

}
