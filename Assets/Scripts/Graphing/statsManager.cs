using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

// This script is run in the game scene and collects data on the animals each game hour

public class statsManager : MonoBehaviour
{
    // script references
    public populationManager PM;
    public timeController TC;

    // where all teh animals are help
    public Transform animalHolder;
    


    private float lastCheck;

    // dict holding stats for each species
    public Dictionary<string, animalStats> statsDict = new Dictionary<string, animalStats>();


    void Start()
    {
        // create a stats class for each species in the dict

        statsDict.Add("Arctic Fox", new animalStats("Arctic Fox"));
        statsDict.Add("Arctic Rabbit", new animalStats("Arctic Rabbit"));
        statsDict.Add("Desert Rabbit", new animalStats("Desert Rabbit"));
        statsDict.Add("Fox", new animalStats("Fox"));
        statsDict.Add("Jungle Frog", new animalStats("Jungle Frog"));
        statsDict.Add("Jungle Warthog", new animalStats("Jungle Warthog"));
        statsDict.Add("Pig", new animalStats("Pig"));
        statsDict.Add("Rabbit", new animalStats("Rabbit"));
        statsDict.Add("Swamp Frog", new animalStats("Swamp Frog"));
        statsDict.Add("Swamp Warthog", new animalStats("Swamp Warthog"));
        statsDict.Add("Tiger", new animalStats("Tiger"));
        statsDict.Add("Wolf", new animalStats("Wolf"));
        statsDict.Add("Camel", new animalStats("Camel"));
    }

    // Update is called once per frame
    void Update()
    {
        if (TC.timeHours - lastCheck > 1) // each game hour update the statistics
        {
            lastCheck = TC.timeHours;
            // go through each animal
            foreach( Transform child in animalHolder)
            {
                // add its current characteristics to the species stats controller
                animalController AC = child.GetComponent<animalController>();
                statsDict[AC.speciesName].addData(AC.minTemp, AC.maxTemp, AC.baseSpeed, child.GetComponent<AnimalCreator>().generation, AC.aggression, AC.chanceAtt1D, AC.chanceAtt2D, PM.getAnimalPop(AC.speciesName),AC.faunaMul,PM.getKPS(AC.speciesName),PM.getBPS(AC.speciesName) ,AC.nearbyPrey,PM.popCount );
            }

            // update the data for each species
            for (int i = 0; i < 13; i++)
            {
                statsDict[PM.animals[i]].updateData();
            }
        }

        
           
        

    }

    // for each species export a csv for it
    public void exportStats()
    {
        for (int i = 0; i < 13; i++)
        {
            statsDict[PM.animals[i]].exportCSV();
        }
    }
}

// class for statistics
public class animalStats{

    public string Species = "";

    // This holds the average data by hour for the species
    public List<int> popByTime = new List<int>();
    public List<float> minTByTime = new List<float>();
    public List<float> maxTByTime = new List<float>();
    public List<float> speedByTime = new List<float>();
    public List<float> genByTime = new List<float>();
    public List<float> aggByTime = new List<float>();
    public List<float> D1ByTime = new List<float>();
    public List<float> D2ByTime = new List<float>();
    public List<float> FMByTime = new List<float>();
    public List<float> KPSByTime = new List<float>();
    public List<float> BPSByTime = new List<float>();
    public List<float> nPByTime = new List<float>();
    public List<float> tPByTime = new List<float>();

    // this holds the cumulative data for the current hour. So the animal adds their
    // data to it each hour then it gets averaged and sent to the above lists
    public int cPop = 0;
    public List<float> cMinT = new List<float>();
    public List<float> cMaxT = new List<float>();
    public List<float> cSpeed = new List<float>();
    public List<float> cGen = new List<float>();
    public List<float> cAgg = new List<float>();
    public List<float> cD1 = new List<float>();
    public List<float> cD2 = new List<float>();
    public List<float> cFM = new List<float>();
    public List<float> cNP = new List<float>();
    public int cTPop;
    public int cKPS;
    public float cBPS;



    public animalStats(string _spec) // constructor
    {
        Species = _spec;
    }

    // add to the cumulatve lists
    public void addData(float _cMinT,float _cMaxT, float _cSpeed,float _cGen ,float _cAgg,float _cD1,float _cD2,int _cPop,float _cFM,int _cKPS, float _cDPS,int _cNP,int _cTPop)
    {
        cMinT.Add(_cMinT);
        cMaxT.Add(_cMaxT);
        cSpeed.Add(_cSpeed);
        cGen.Add(_cGen);
        cAgg.Add(_cAgg);
        cD1.Add(_cD1);
        cD2.Add(_cD2);
        cFM.Add(_cFM);
        cNP.Add(_cNP);
        cPop = _cPop;
        cKPS = _cKPS;
        cBPS = _cDPS;
        cTPop = _cTPop;


    }

    // add cumuative averages to the main lists
    public void updateData()
    {
        popByTime.Add(cPop);
        minTByTime.Add(cMinT.Average());
        maxTByTime.Add(cMaxT.Average());
        speedByTime.Add(cSpeed.Average());
        genByTime.Add(cGen.Average());
        aggByTime.Add(cAgg.Average());
        D1ByTime.Add(cD1.Average());
        D2ByTime.Add(cD2.Average());
        FMByTime.Add(cFM.Average());
        KPSByTime.Add(cKPS);
        BPSByTime.Add(cBPS);
        nPByTime.Add(cNP.Average());
        tPByTime.Add(cTPop);
    }

    // exports a csv of this species alone
    public void exportCSV()
    {

        string fileName = Application.dataPath + @"\AnimalData\" + Species + ".csv";

        // check the directory exists else make one
        if (!Directory.Exists(Application.dataPath + @"\AnimalData\"))
            Directory.CreateDirectory(Application.dataPath + @"\AnimalData\");


        //create the file
        TextWriter TW = new StreamWriter(fileName, false);
        TW.WriteLine("Hours,Pop,MinT,MaxT,Speed,Gen,Agg,D1,D2,FM,KPS,BPS,nPrey,TotPop");
        TW.Close();

        TW = new StreamWriter(fileName, true);
        // start adding in all the data to the csv
        for(int i=0; i < popByTime.Count; i++)
        {
            TW.WriteLine(i.ToString() + "," + Mathf.RoundToInt(popByTime[i]).ToString() + "," +  minTByTime[i].ToString() + "," + maxTByTime[i].ToString() + "," + speedByTime[i].ToString() + "," + Mathf.RoundToInt(genByTime[i]).ToString() + "," + aggByTime[i].ToString() + "," + D1ByTime[i].ToString() + "," + D2ByTime[i].ToString()+","+FMByTime[i].ToString()+","+KPSByTime[i].ToString()+","+BPSByTime[i].ToString()+","+nPByTime[i].ToString()+","+tPByTime[i].ToString()  );
        }
        TW.Close();
    }
}
