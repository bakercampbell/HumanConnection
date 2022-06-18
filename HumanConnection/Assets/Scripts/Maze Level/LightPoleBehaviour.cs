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
    [SerializeField]
    GameObject particles;
    [SerializeField]
    float particleTimer, particleTimerReset;


    private void Update()
    {
        if (!isOn)
        {
            particles.SetActive(true);
            particleTimer -= Time.deltaTime;
            if (particleTimer <= 0)
                particles.SetActive(false);

        }
    }
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
        particleTimer = particleTimerReset;
        lightOnEvent?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
            Interact();
    }

}
