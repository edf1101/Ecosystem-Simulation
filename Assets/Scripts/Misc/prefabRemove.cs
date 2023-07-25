using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prefabRemove : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float time;
    void Start()
    {
        Destroy(gameObject,time);  
    }

   
}
