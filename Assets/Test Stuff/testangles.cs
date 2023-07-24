using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testangles : MonoBehaviour
{

    public SortedList<int, string> test = new SortedList<int, string>();

    // Start is called before the first frame update
    public GameObject target;
    public float ang;
    void Start()
    {
        test.Add(2, "hi");
       
    }

    // Update is called once per frame
    void Update()
    {// o/a    
        ang = Mathf.Rad2Deg* Mathf.Atan2(target.transform.position.z - transform.position.z, target.transform.position.x - transform.position.x);
    }
}
