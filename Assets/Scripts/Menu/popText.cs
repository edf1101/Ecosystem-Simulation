using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class popText : MonoBehaviour
{
    // Start is called before the first frame update
    public populationManager PM;
    float lastUpdate;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastUpdate > 0.1f)
        {
            string tex = "Total: "+PM.popCount.ToString()+"\n"+"Time Passed: "+PM.TC.timeHours+" hrs \n";
            for(int i = 0; i < 13; i++)
            {
                tex += PM.animals[i] + " : " + PM.animalPops[i].ToString() + "   " + PM.animalChanges[i].ToString() + "\n";
            }
            GetComponent<UnityEngine.UI.Text>().text = tex;
            lastUpdate = Time.time;
        }
    }
}
