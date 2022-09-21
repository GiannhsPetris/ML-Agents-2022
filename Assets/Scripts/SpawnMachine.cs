using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMachine : MonoBehaviour
{

    [Header("Lists")]

    [SerializeField] private List<GameObject> enemyTypeList = new List<GameObject>(){};
    [SerializeField] private List<Transform> goalList = new List<Transform>(){};
    [SerializeField] private List<Transform> spawnPointList = new List<Transform>(){};

    [Header("Stats")]

    public int currentAlive;
    [SerializeField] private int currentMax = 5;
    [SerializeField] private int leftToSpawn;
    [SerializeField] private float timeToSpawn;
    [SerializeField] private float currentTime = 0;

    Enemy enemy;



    // Update is called once per frame
    void Update()
    {
        if (currentTime > 0) {
            currentTime -= Time.deltaTime;
        }else{
            checkSpawn();
            currentTime = timeToSpawn;
        }
    }

    void checkSpawn(){
        leftToSpawn = currentMax - currentAlive;
        if (leftToSpawn > 0) StartCoroutine( Spawn(leftToSpawn) ) ;
    }

    private IEnumerator Spawn(int times){

        WaitForSeconds wait = new WaitForSeconds( 3.5f ) ;

        for (int i = 0; i < times; i++) {
            int enemyIndex = Random.Range(0, enemyTypeList.Count);
            int spawnIndex = Random.Range(0, spawnPointList.Count);

            GameObject clone = GameObject.Instantiate(enemyTypeList[enemyIndex], spawnPointList[spawnIndex]);
            clone.GetComponent<Transform>().localPosition = new Vector3(0,0,0);
            enemy = clone.GetComponent<Enemy>();
            enemy.setGoal(goalList[spawnIndex]);
            currentAlive ++;

            yield return wait ;
        }
    }

    public void deathOccured(){
        currentAlive --;
    }
}
