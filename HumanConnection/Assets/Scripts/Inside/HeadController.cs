using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadController : MonoBehaviour
{
    [SerializeField] private int health = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            health -= 5;

        }
    }

    private IEnumerator DeathSequence()
    {

        SceneManager.LoadScene("OutsideLab", LoadSceneMode.Single);

        yield return new WaitForSeconds(4);

    }

    public void Dead()
    {
        if (health > 0.1)
        {
            return;
        }
        else
        health = 0;
        
        Debug.Log("He dead, buddy.");
        Destroy(gameObject);
        StartCoroutine(DeathSequence());
        return;
        
    }
}
