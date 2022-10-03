using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private float damage = 50;

    AgentAI player;

    
    void Start()
    {
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            //if (collision.gameObject.tag == "floor") player.rewardSystem(-0.1f);
            //print(collision.gameObject.name);
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage);

            if (collision.gameObject.tag == "enemy"){
                Debug.Log("hit");
            }

            if (collision.gameObject.tag == "wall")
            {
                return;
            }
            
            //cloning explotion
            GameObject clone = GameObject.Instantiate(explosion, transform);
            clone.SetActive(true);
            clone.transform.position = transform.position; 
            clone.transform.parent = null;

            Destroy(clone.gameObject, 5f);
            Destroy(gameObject);
        }
    }
}
