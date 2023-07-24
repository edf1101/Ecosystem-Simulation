using UnityEngine;
using UnityEngine.UI;

public class popText : MonoBehaviour // this script displays how the populations of each animal species are faring over the simulation
{
  
    public populationManager PM;
    float lastUpdate;

    void Update()
    {
        if (Time.time - lastUpdate > 0.1f) // update every 0.1s 
        {
            string tex = "Total: "+PM.popCount.ToString()+"\n"+"Time Passed: "+PM.TC.timeHours+" hrs \n"; //print a string featuring animal name ,current population and % of original
            for(int i = 0; i < 13; i++)
            {
                tex += PM.animals[i] + " : " + PM.animalPops[i].ToString() + "   " + PM.animalChanges[i].ToString() + "\n";
            }
            GetComponent<UnityEngine.UI.Text>().text = tex;
            lastUpdate = Time.time;
        }
    }
}
