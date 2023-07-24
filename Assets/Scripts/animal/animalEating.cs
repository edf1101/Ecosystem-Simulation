using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animalEating : MonoBehaviour // this script is used to hekp animals eat each other
{

    animalController aC;
    float lastEnter;
    [SerializeField] GameObject bloodPrefab;
    public Transform effectsLocation;
    public float bloodOffset;
    bool inside;


    public void myStart() // get main animal controller on start 
    {
        aC = GetComponent<animalController>();
    }


    private void OnTriggerStay(Collider other) // if i enter the collider of an object
    {
        // print("x");
        if (other.gameObject == aC.followingWho && Time.time - lastEnter > 1f)// if its my target and i havent attacked in last second
        {
            lastEnter = Time.time;
            GameObject temp = Instantiate(bloodPrefab, effectsLocation); // create blood particles 
            temp.transform.position = transform.position + (transform.forward * bloodOffset);
            aC.followingWho.GetComponent<animalController>().health -= 20; // decrease their health
            aC.hunger += 10;// increase my food rating
            if (aC.hunger > 100)
                aC.hunger = 100;

        }
    }
}
