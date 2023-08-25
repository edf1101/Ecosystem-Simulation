using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/*
 * The aim of this project was to simulate an ecosystem, to do this well we dont want
 * any populations dieing off immediately. This script artificially maintains populations
 *  By either increasing the range of view to find mates or increasing the chance of 
 *  having large numbers of offspring
 * 
 */

public class populationManager : MonoBehaviour
{
    // total population
    public int popCount=0;
  
    public timeController TC; // time controller reference
    
    private Dictionary<string,animalTracker> animalTrackers=new Dictionary<string, animalTracker>();

    // arrays containing all animals species differences and populations... 
    public string[] animals;
    private float lastsec;
    private float[] animalDifs = new float[13];
    public int[] animalPops = new int[13];
    public float[] animalChanges = new float[13];
    private float lastPop;
    private float[] origPop = new float[13];


    private void Start()
    {
        // get timeController 
        TC = GameObject.Find("Simulation Manager").GetComponent<timeController>();

        // add all the different animal trackers
        animalTrackers.Add("Arctic Fox", new animalTracker("Arctic Fox",0));
        animalTrackers.Add("Arctic Rabbit", new animalTracker("Arctic Rabbit",1));
        animalTrackers.Add("Desert Rabbit", new animalTracker("Desert Rabbit",2));
        animalTrackers.Add("Fox", new animalTracker("Fox",3));
        animalTrackers.Add("Jungle Frog", new animalTracker("Jungle Frog",4));
        animalTrackers.Add("Jungle Warthog", new animalTracker("Jungle Warthog",5));
        animalTrackers.Add("Pig", new animalTracker("Pig",6));
        animalTrackers.Add("Rabbit", new animalTracker("Rabbit",7));
        animalTrackers.Add("Swamp Frog", new animalTracker("Swamp Frog",8));
        animalTrackers.Add("Swamp Warthog", new animalTracker("Swamp Warthog",9));
        animalTrackers.Add("Tiger", new animalTracker("Tiger",10));
        animalTrackers.Add("Wolf", new animalTracker("Wolf",11));
        animalTrackers.Add("Camel", new animalTracker("Camel",12));

        // getall the keys and set up their populatuon trackrs
        animals  =  animalTrackers.Keys.ToArray();
        foreach (string i in animals)
        {
            animalTrackers[i].setupPop();
        }

    
    }

   
    private void doPop()
    {
        // set all populations to 0 
        for (int i = 0; i < 13; i++)
            animalPops[i] = 0;

        // go through each animal and add them to their species populations
        Transform myT = transform;
        foreach (Transform child in myT)
            animalPops[animalTrackers[child.GetComponent<animalController>().speciesName].index]++;

    }


    //called once per frame
    private void Update()
    {
        if (TC.timeHours - lastPop > 0.2f) // update every .2secs
        {

            popCount = transform.childCount;// update total population

            lastPop = TC.timeHours;
            doPop(); // update species populations

            if (origPop.Sum() == 0)
            {
                for (int i = 0; i < 13; i++)
                    origPop[i] = animalPops[i];
            }

            // work out the change in population ratio for each species
            for (int i = 0; i < 13; i++)
            {
                if (origPop[i] != 0)
                {
                    animalChanges[i] = animalPops[i] / origPop[i];
                    animalTrackers[animals[i]].pop = animalPops[i];
                    animalTrackers[animals[i]].change = animalChanges[i];
                }
                else
                    animalChanges[i] = 0f;
            }
        }

        // update the difs and pops for each species every .3 secs
        if (TC.timeHours - lastsec > 0.3f)
        {
            lastsec = TC.timeHours;
           for (int i=0;i<13;i++)
            {
                animalTrackers[animals[i]].UpdatePop(animalPops[i]);
                animalDifs[i] = animalTrackers[animals[i]].getDif;
            } 




        }
       
    
    
    
    
    }

    // get and set births deaths and kills per sec for each species
    public void increaseBPS(string Species)
    {
        animalTrackers[Species].addBPS();
    }
    public void increaseDPS(string Species)
    {
        animalTrackers[Species].addDPS();
    }
    public void increaseKPS(string species)
    {
        animalTrackers[species].addKPS();
    }
    public int getKPS(string Species)
    {
        return animalTrackers[Species].lastKPS;
    }
    public float getBPS(string Species)
    {
        return animalTrackers[Species].getBPS();
    }

    // get the birth inflation chances
    public float getRange(string Species)
    {
        return animalTrackers[Species].range;
    }
    public float Getchance1(string Species)
    {
        return animalTrackers[Species].chance1;
    }
    public float getchance2(string Species)
    {
        return animalTrackers[Species].chance2;
    }
    public float getchance3(string Species)
    {
        return animalTrackers[Species].chance3;
    }
    public float getchance4(string Species)
    {
        return animalTrackers[Species].chance4;
    }
    public int getAnimalPop(string Species)
    {
        return animalTrackers[Species].pop;
    }

}




// class for tracking all the animals
public class animalTracker
{
    public string species = "";

    private int DPS; // deaths per second
    private Queue<float> DPavgs = new Queue<float>(5);
    private float DPaverage;

    private int BPS; // births per sec
    private Queue<float> BPavgs = new Queue<float>(5);
    private float BPaverage;

    private int KPS; // kills per sec
     public int lastKPS;
 



    public int index; // of species

    // chance of having 1child  2 childs etc... 
    public float chance1;
    public float chance2;
    public float chance3;
    public float chance4;
    public float range;

    public int pop;
    public float change;

    //constructor
    public animalTracker(string _species,int _index)

    {
        index = _index;
        species = _species;
    }


    //setup the population
    public void setupPop()
    {
       
        BPavgs.Enqueue(0);
        BPavgs.Enqueue(0);
        BPavgs.Enqueue(0);
        BPavgs.Enqueue(0);
        BPavgs.Enqueue(0);
     
        DPavgs.Enqueue(0);
        DPavgs.Enqueue(0);
        DPavgs.Enqueue(0);
        DPavgs.Enqueue(0);
        DPavgs.Enqueue(0);
    }

    public void UpdatePop(int _pop)
    {

        // work out BPS DPS average

        pop =_pop;
        BPavgs.Dequeue();
        BPavgs.Enqueue(BPS);
        float[] BPavgList = BPavgs.ToArray();
        BPaverage = (BPavgList[0] + BPavgList[2] + BPavgList[1] + BPavgList[3] + BPavgList[4]) / 5f;
        BPS = 0;

        DPavgs.Dequeue();
        DPavgs.Enqueue(DPS);
        float[] DPavgList = DPavgs.ToArray();
        DPaverage = (DPavgList[0] + DPavgList[2] + DPavgList[1] + DPavgList[3] + DPavgList[4]) / 5f;
        DPS = 0;
        lastKPS = KPS;
        KPS = 0;


        // set the chances based on total populatuons and the change between original 
        if (pop < 10 || change < 0.3)
        {
            chance4 = 1;
            chance3 = 1f;
            chance2 = 1;
            chance1 = 1;
            range = 40;
        }
        if((pop<20)|| change < 0.55) // mega boost
        {
            chance4 = 0.6f;
            chance3 = 0.8f;
            chance2 = 1f;
            chance1 = 1;
            range = 20;

        }
        else if ((pop < 30 && pop>4) || (change < 0.9f&& change > 0.501f)) // boost
        {
            chance3 = 0.6f;
            chance2 = 0.7f;
            chance4 = 0;
            chance1 = 1;
            range = 15;
        }
        else  if (change>1.4f && pop > 30&& change<=2) {
            chance3 = 0.05f;
            chance2 = 0.1f;
            chance1 = 0.7f;
            range = 3.5f;
            chance4 = 0;
        }
        else if ((pop<=400&&pop>200) ||(change > 2f&&change<3 && pop > 30f))
        {
            chance3 = 0.0f;
            chance2 = 0.0f;
            chance1 = 0.3f;
            chance4 = 0;
            range = 2;
        }
        else if (change >= 3 || pop > 250)
        {
            chance3 = 0.0f;
            chance2 = 0.0f;
            chance1 = 0.0f;
            chance4 = 0;
            range = 2;
        }
        else
        {
            chance3 = 0.1f;
            chance2 = 0.3f;
            chance1 = 1f;
            range = 7;
            chance4 = 0;
        }

    }

    // setters for death, birth/ kills per unit time
    public void addDPS() { DPS++; }
    public void addBPS() { BPS++; }
    public void addKPS() { KPS++; }

    // getter for BPS
    public float getBPS()
    {
        return BPaverage;
    }

    // get difference between births and deaths per sec average
    public float getDif { get { return BPaverage - DPaverage; } }

}
