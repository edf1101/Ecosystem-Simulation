using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
   public GameObject GO;
    void Start()
    {
    //   GO.GetComponent<NoiseGen>().Start();
        GetComponent<Renderer>().material.mainTexture= GO.GetComponent<NoiseGen>().combo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
