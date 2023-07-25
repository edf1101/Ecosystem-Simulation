using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeController : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed; // needs to be equal to lighting speed;
    public float timeHours = 0;
    public float multiplier = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeHours += (Time.deltaTime) * (speed*multiplier);
    }
}
