using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadController : MonoBehaviour
{
    [SerializeField] private int health = 100;
    GameObject monster;
    MonsterController monsterController;
    Animator animator;
    AudioSource audioSource;
    [SerializeField]GameObject victoryScreen;
    private void Start()
    {
        
        monster = GameObject.FindGameObjectWithTag("Monster");
        monsterController = monster.GetComponent<MonsterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

    }

    private void Update()
    {
        if (monsterController.health <= 0)
        {
            StartCoroutine(DownTime());
        }
        
        Dead();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            health -= 2;

        }
    }

    IEnumerator DeathSequence()
    {
        Debug.Log("He dead, buddy.");
        victoryScreen.SetActive(true);
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("OutsideLab", LoadSceneMode.Single);
    }

    IEnumerator DownTime()
    {
        animator.Play("Spazzing");
        audioSource.Play(); 
        
        yield return new WaitForSeconds(7.5f);
        animator.Play("Head Hanging");
    }

    public void Dead()
    {
        if (health > 0.1)
        {
            return;
        }
        else
        health = 0;
        StartCoroutine(DeathSequence());
        
        
        
        return;

    }
}
