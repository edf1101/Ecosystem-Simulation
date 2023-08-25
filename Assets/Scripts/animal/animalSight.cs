using System.Collections.Generic;
using UnityEngine;


// This script maintains and returns lists of predators/ prey / mates around me

public class animalSight : MonoBehaviour
{

    // Lists of animals near
    public GameObject[] Sees;
    public GameObject[] predators;
    public GameObject[] prey;
    public GameObject[] mates;

    public Texture2D baseAvailibles; // texture of availible locations
    private float lastLooked;
    public Texture2D cheapMoves;
  

    void Update()
    {
        if (Time.time - lastLooked > 0.4f) // each.4s update lists
        {
            lastLooked = Time.time;
            prey = getPrey();
            predators = getPredators();
        }


    }

    // update the lists
    public void UpdatePrey()
    {
        prey = getPrey();
        predators = getPredators();
        mates = getMates();
    }


    // get nearby predators
    private GameObject[] getPredators()
    {
        // find all colliders in 10m radius
        Collider[] collisions = Physics.OverlapSphere(transform.position, 10);
        List<GameObject> behinds = new List<GameObject>();

        // go through each collider
        foreach (Collider col in collisions)
        {
            if (col.GetComponentInParent<animalController>() != null)// if it is an animal
            {
                // if its above me in food chain
                if (col.GetComponentInParent<animalController>().foodChain > GetComponent<animalController>().foodChain)
                    behinds.Add(col.GetComponentInParent<animalController>().gameObject);
            }

        }
        return behinds.ToArray();
    }

    // get nearby mates
    public GameObject[] getMates()
    {
        // find all colliders nearby
        Collider[] collisions = Physics.OverlapSphere(transform.position, GetComponent<animalController>().PM.getRange(GetComponent<animalController>().speciesName));
        List<GameObject> mates = new List<GameObject>();

        // go through each collider
        foreach (Collider col in collisions)
        {
            if (col.GetComponentInParent<animalController>() != null && col.GetComponentInParent<animalSight>() != this)// if it is an animal
            {
                // if opposite gender and of age and same species then add them to mates list
                if (col.GetComponentInParent<animalController>().canChild == 1 && col.GetComponentInParent<animalController>().age > GetComponent<animalController>().childWait && col.GetComponentInParent<animalController>().speciesName == GetComponent<animalController>().speciesName && col.GetComponentInParent<animalController>().male != GetComponent<animalController>().male)
                    mates.Add(col.GetComponentInParent<animalController>().gameObject);

            }

        }
        return mates.ToArray();

    }

    // get list of prey nearby
    public GameObject[] getPrey()
    {
        // find nearby colliders
        Collider[] collisions = Physics.OverlapSphere(transform.position, 10);
        List<GameObject> behinds = new List<GameObject>();
        float chance2D = GetComponent<animalController>().chanceAtt2D;
        foreach (Collider col in collisions)
        {
            if (col.GetComponentInParent<animalController>() != null)// if it is an animal
            {
                // if below me in food chain add to list
                if (col.GetComponentInParent<animalController>().foodChain == GetComponent<animalController>().foodChain-1 || (col.GetComponentInParent<animalController>().foodChain == GetComponent<animalController>().foodChain - 2 &&Random.value<chance2D))
                    behinds.Add(col.GetComponentInParent<animalController>().gameObject);

            }

        }
        return behinds.ToArray();


    }

}
