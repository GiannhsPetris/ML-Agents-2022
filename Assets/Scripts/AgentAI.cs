using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;

public class AgentAI : Agent
{
    [SerializeField] GameObject spawn;
    SpawnMachine spawnMachine;

    [Header("Stats")]
    [SerializeField] private float moveSpeed; 
    [SerializeField] private int health;
    [SerializeField] private int currentHealth;
    [SerializeField] private float force;
    private float fireElapsedTime = 0;
    [SerializeField] private float fireDelay = 0.2f;
    private float manaElapsedTime = 0;
    [SerializeField] private float manaDelay = 2f;
    [SerializeField] private int manaSize;
    [SerializeField] private int currentMana;
    [SerializeField] private int killCounter;
    [SerializeField] private int killMax;
    [SerializeField] private float timeToSpawn = 15;
    [SerializeField] private float currentTime = 0;

    [SerializeField] private List<Transform> goalList = new List<Transform>() { };
    [SerializeField] private List<Transform> spawnPointList = new List<Transform>() { };


    [Header("GameObjects Through Inspector")]
    [SerializeField] private Transform castPosition;
    [SerializeField] private GameObject particles;
    [SerializeField] private Transform goal1, goal2, goal3;
    [SerializeField] private GameObject forceField;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject bullet;


    [Header("UI")]
    [SerializeField] Text runs;
    [SerializeField] Text kills;
    [SerializeField] Text healthUi;
    [SerializeField] Text wins;
    [SerializeField] Text loses;


    private Animator animator;
    private AudioSource audioSource;
    private bool lockG;

    int loseCounter = 0;
    int winCounter = 0;
    int runCounter = 0;
    float total = 0;
    float totalwall = 0;

    




    public override void OnEpisodeBegin()
    {
        healthUi.text = health.ToString();
        kills.text = "0 / " + killMax.ToString();
        total = 0;

        lockG = true;
        particles.SetActive(false);
        currentHealth = health;
        currentMana = manaSize;
        killCounter = 0;
        //Debug.Log("new episode");

    }

    //call check spawn and update times
    public void FixedUpdate()
    {
        fireElapsedTime += Time.deltaTime;
        manaElapsedTime += Time.deltaTime;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            spawnMachine.checkSpawn(goalList, spawnPointList);
            currentTime = timeToSpawn;
        }
    }


    public void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spawnMachine = spawn.GetComponent<SpawnMachine>();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(currentHealth);
        sensor.AddObservation(currentMana);
        sensor.AddObservation(lockG);
        sensor.AddObservation(spawnMachine.currentAlive);
        sensor.AddObservation(goal1.position);
        sensor.AddObservation(goal2.position);
        sensor.AddObservation(goal3.position);
    }

    


    public override void Heuristic(in ActionBuffers actionsOut){
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
       
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"))) {
            case -1: discreteActions[0] = 1; break;
            case 0: discreteActions[0] = 0; break;
            case +1: discreteActions[0] = 2; break;
        }

      
        discreteActions[1] = Input.GetKey(KeyCode.M) ? 1 : 0;
        discreteActions[2] = Input.GetKey(KeyCode.N) ? 1 : 0;

    }


    // AI actions
    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveX = actions.DiscreteActions[0];
        
        Vector3 addForce = new Vector3(0, 0, 0);

        switch(moveX) {
            case 0: addForce.x = 0f; break;
            case 1: addForce.x = -1f; break;
            case 2: addForce.x = +1f; break;
        }

        rb.velocity = addForce * moveSpeed + new Vector3(0, 0, 0);
       
        bool isShoot = actions.DiscreteActions[1] == 1;

        if(isShoot && lockG) shoot();

        bool isMana = actions.DiscreteActions[2] == 1;

        if(isMana && lockG) getMana();
        
    }

    

    

    public void shoot(){

         if (fireElapsedTime >= fireDelay && currentMana > 0){
            fireElapsedTime = 0;
            currentMana = currentMana - 1 ;
            if (currentMana < 0) currentMana = 0;
        

            animator.Play("cast", 0, 0.25f);
            audioSource.Play();

            GameObject clone = GameObject.Instantiate(bullet, transform);
            clone.transform.position = castPosition.position;
            clone.SetActive(true);
            
            clone.transform.parent = null;
            
            Destroy(clone, 5f);
            clone.GetComponent<Rigidbody>().AddForce(transform.forward * force);
         }
    }


    public void getMana(){
        if (manaElapsedTime >= manaDelay)
        {
            manaElapsedTime = 0;
            currentMana = manaSize;
            animator.Play("mana");
        }
        
    }


    public void lockGun()
    {
        lockG = false;
        //Debug.Log("lock");
        particles.SetActive(true);
    }

    public void unlockGun()
    {
        lockG = true;
        //Debug.Log("unlock");
        particles.SetActive(false);
    }


    public void insideBase(int dmg)
    {
        currentHealth = currentHealth - dmg;
        healthUi.text = currentHealth.ToString();

        rewardSystem(-0.5f);
        if (currentHealth <= 0) 
        {
            loseCounter ++;
            loses.text = loseCounter.ToString();

            runCounter++;
            runs.text = runCounter.ToString();

            SetReward(-2f);
            EndEpisode();
        }
        
        
    }

    public void enemyKilled()
    {
        killCounter ++;
        kills.text = killCounter.ToString() + " / " + killMax.ToString();

        rewardSystem(0.5f);
        if (killCounter >= killMax) {
            runCounter++;
            runs.text = runCounter.ToString();

            winCounter ++;
            wins.text = winCounter.ToString();

            SetReward(2f);
            EndEpisode();
        }
    }


    public void rewardSystem(float amount)
    {
        AddReward(amount);
    }

    void OnCollisionEnter(Collision collision)
    {
        //move agent if he touches the wall

        if (collision.gameObject.tag == "wallRight") 
        {
            gameObject.transform.position += new Vector3(-0.1f,0,0);
        }

        if (collision.gameObject.tag == "wallLeft")
        {
            gameObject.transform.position += new Vector3(0.1f, 0, 0);
        }
    }   

}
