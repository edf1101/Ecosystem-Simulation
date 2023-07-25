using System.Collections.Generic;
using UnityEngine;

public class animalSight : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] Sees;
    public GameObject[] predators;
    public Texture2D baseAvailibles;
    float lastLooked;
    public Texture2D cheapMoves;
    public GameObject[] prey;
    public GameObject[] mates;
    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastLooked > 0.4f)
        {
            lastLooked = Time.time;
            prey = getPrey();
            predators = getPredators();
            //  mates = getMates();
        }


    }
    public void UpdatePrey()
    {
        prey = getPrey();
        predators = getPredators();
        mates = getMates();
    }
    public Vector2[] getCheapMoves()
    {
        int detail = 20;
        int tryLength = 5;
        cheapMoves = new Texture2D(250, 250);
        cheapMoves.SetPixels(baseAvailibles.GetPixels());
        cheapMoves.Apply();
        List<Vector2> cheaps = new List<Vector2>();
        for (int ang = 0; ang < 360; ang += detail)
        {
            var dir = Quaternion.Euler(0, ang, 0) * new Vector3(transform.forward.x, 0, transform.forward.z);

            for (float dis = 0; dis < tryLength; dis += 0.5f)
            {
                Vector2 Pos = new Vector2(transform.position.x, transform.position.z) + new Vector2((dir.normalized * dis).x, (dir.normalized * dis).z);
                if (baseAvailibles.GetPixel((int)Pos.x, (int)Pos.y) != Color.black)
                {
                    cheapMoves.SetPixel((int)Pos.x, (int)Pos.y, Color.blue);
                    cheaps.Add(Pos);
                }
                else
                {
                    break;
                }

            }



        }
        cheapMoves.SetPixel((int)transform.position.x, (int)transform.position.z, Color.white);
        cheapMoves.Apply();
        return cheaps.ToArray();
    }


    GameObject[] getPredators()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, 10);
        List<GameObject> behinds = new List<GameObject>();
        foreach (Collider col in collisions)
        {
            if (col.GetComponentInParent<animalController>() != null)// if it is an animal
            {

                if (col.GetComponentInParent<animalController>().foodChain > GetComponent<animalController>().foodChain)
                    behinds.Add(col.GetComponentInParent<animalController>().gameObject);



            }

        }
        return behinds.ToArray();


    }
    public GameObject[] getMates()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, GetComponent<animalController>().PM.getRange(GetComponent<animalController>().speciesName));
        List<GameObject> mates = new List<GameObject>();
        foreach (Collider col in collisions)
        {
            if (col.GetComponentInParent<animalController>() != null && col.GetComponentInParent<animalSight>() != this)// if it is an animal
            {

                if (col.GetComponentInParent<animalController>().canChild == 1 && col.GetComponentInParent<animalController>().age > GetComponent<animalController>().childWait && col.GetComponentInParent<animalController>().speciesName == GetComponent<animalController>().speciesName && col.GetComponentInParent<animalController>().male != GetComponent<animalController>().male)
                    mates.Add(col.GetComponentInParent<animalController>().gameObject);



            }

        }
        return mates.ToArray();

    }
    public GameObject[] getPrey()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, 10);
        List<GameObject> behinds = new List<GameObject>();
        float chance2D = GetComponent<animalController>().chanceAtt2D;
        foreach (Collider col in collisions)
        {
            if (col.GetComponentInParent<animalController>() != null)// if it is an animal
            {

                if (col.GetComponentInParent<animalController>().foodChain == GetComponent<animalController>().foodChain-1 || (col.GetComponentInParent<animalController>().foodChain == GetComponent<animalController>().foodChain - 2 &&Random.value<chance2D))
                    behinds.Add(col.GetComponentInParent<animalController>().gameObject);



            }

        }
        return behinds.ToArray();


    }

}
