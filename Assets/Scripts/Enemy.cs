using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("NavMeshAgent")]

    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Transform goal;
    

    [Header("Stats")]

    [SerializeField] private float maxHealth = 100f;
    private float health;
    [SerializeField] private int damageBase;
    [SerializeField] private float speed;

    [Header("Particles and Animation")]
    
    [SerializeField] private ParticleSystem blood;
    [SerializeField] private ParticleSystem electricity;
    [SerializeField] private Animator animator;

    AgentAI aiAgent;
    SpawnMachine spawn;
       
    void Start () 
    {
        health = maxHealth;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.destination = goal.position; 
        aiAgent = GameObject.FindGameObjectWithTag("Player").GetComponent<AgentAI>();
        spawn = GameObject.FindGameObjectWithTag("Spawn").GetComponent<SpawnMachine>();
    }

    // Update is called once per frame
    void Update()
    {
         if (navAgent.enabled == true){
            if (navAgent.remainingDistance <= 1f){
                animator.Play("death");
                navAgent.enabled = false;
                electricity.Play();
                Die();
                aiAgent.insideBase(damageBase);
            }
        }
    }

    public void setGoal(Transform goTo){
        goal = goTo;
    }


    public void TakeDamage(float damage)
    {
        health -= damage;
        aiAgent.rewardSystem(0.5f);
        blood.Play();
        if (health<=0){
            aiAgent.enemyKilled();
            Die();
        }
    }

    void Die()
    {
        animator.Play("death");
        Destroy(GetComponent<UnityEngine.AI.NavMeshAgent>());
        GetComponent<Enemy>().enabled = false;
        Destroy(GetComponent<Collider>());
        this.enabled = false;
        Destroy(gameObject, 8f);
        spawn.deathOccured();
    }

}
