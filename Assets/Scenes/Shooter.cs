using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shooter : MonoBehaviour
{
    public GameObject bullet;
    public GameObject player;
    public Transform castPosition;
    public GameObject camera2;
    public float power = 20f; 
    int timeShoot;
    // Start is called before the first frame update
    void Start()
    {
        timeShoot = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)){
        //if (Input.GetMouseButtonDown(0)){
            timeShoot++;
            GameObject clone = GameObject.Instantiate(bullet, transform);
            clone.transform.position = castPosition.position;
            clone.SetActive(true);
            //clone.transform.localPosition = new Vector3(0,0,0);
            clone.transform.parent = null;
            //clone.transform.position = transform.position; //100,0,0
            Destroy(clone, 5f);
            clone.GetComponent<Rigidbody>().AddForce(transform.forward * power);
        }
    }
}
