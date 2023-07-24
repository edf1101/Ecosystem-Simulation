using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inWater : MonoBehaviour
{
    // Start is called before the first frame update
   [SerializeField] NoiseGen NG;
    [SerializeField] TerrainGenerator TG;
    [SerializeField] float oceanHeight;
    public bool waterStatus = false;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < oceanHeight)
            waterStatus = true;
        else if (transform.position.y < NG.perl.GetPixel((int)transform.position.x * 4, (int)transform.position.z * 4).r * TG.strength * 0.025)
        {
           waterStatus = true;
        }
        else
            waterStatus = false;
    }
}
