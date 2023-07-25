using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testMove : MonoBehaviour
{
    [SerializeField] float speed;

    float ang = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ang += Time.deltaTime * speed;
        transform.localEulerAngles=new Vector3(0,ang,0);    
    }
}
