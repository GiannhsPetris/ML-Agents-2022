using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMachine : MonoBehaviour
{

    [Header("Lists")]

    [SerializeField] private List<GameObject> enemyTypeList = new List<GameObject>(){};

    [Header("Stats")]

    public int currentAlive;
    [SerializeField] private int currentMax = 5;
    [SerializeField] private int leftToSpawn;
    

    Enemy enemy;


    public void checkSpawn(List<Transform> goalList, List<Transform> spawnPointList)
    {
        leftToSpawn = currentMax - currentAlive;
        if (leftToSpawn > 0) StartCoroutine( Spawn(leftToSpawn, goalList, spawnPointList )) ;
        
    }

    private IEnumerator Spawn(int times, List<Transform> goalList, List<Transform> spawnPointList)
    {

        WaitForSeconds wait = new WaitForSeconds( 3.5f ) ;

        for (int i = 0; i < times; i++) {
            int enemyIndex = Random.Range(0, enemyTypeList.Count);
            int spawnIndex = Random.Range(0, spawnPointList.Count);

            GameObject clone = GameObject.Instantiate(enemyTypeList[enemyIndex], spawnPointList[spawnIndex]);

            enemy = clone.GetComponent<Enemy>();
            enemy.setGoal(goalList[spawnIndex]);
            currentAlive ++;

            yield return wait ;
        }
    }

    public void deathOccured()
    {
        currentAlive --;
    }
}
