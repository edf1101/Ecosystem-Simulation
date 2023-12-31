using System.Collections.Generic;
using UnityEngine;

public class biomeGenerator : MonoBehaviour
{

    //Textures for differnet maps 
    public Texture2D baseColorMap;
    public Texture2D steepColorMap;
    public Texture2D slightColorMap;
    public Texture2D riverColorMap;

    public Dictionary<Vector2, Biome> biomeDict; // what biome is each tile

    public Texture2D biomeMap; // debug reasons
    public Texture2D tempMap;// base temperature
    public Texture2D humMap; // base humidity


    [System.Serializable]
    public class objectPlacement
    {
        public GameObject obj;
        public float radius;

    }

    // biome class
    [System.Serializable]
    public class Biome
    {
        //basic info
        public string name;
        public int Index;

        //colours for different conditions
        public Color baseColor;
        public Color steepColor;
        public Color slightColor;
        public Color riverColor;

        public float steepGradient;
        public float slightGradient;


        //object generation
        public GameObject[] smallObjs;
        public objectPlacement[] trees;
        public bool lotsOfGrass;
        public GameObject[] animals;

        //weather data
        public float biomeTemp;
        public float biomeHum;
    }

    public Biome[] biomes; // 0 - grass      1- winter   2-DEsert   3-Jungle   4- swamp


    // create the biomes
    public void generateBiomes()
    {
      
        PoissonSampling PS = new PoissonSampling();
        int count = 0;
        List<Vector2> Points=new List<Vector2>();

        while (count != 8)
        {
        Points = PS.GetPoissonPoints(10, 100, 500, protectEdge: false);
            count = Points.Count;
        } // create a set of poisson points around the world until we get one with 8 points in it


        // create colour maps
        baseColorMap = new Texture2D(500, 500);
        riverColorMap=new Texture2D(500, 500);
        slightColorMap=new Texture2D(500, 500);
        steepColorMap =new Texture2D(500, 500);
        biomeMap = new Texture2D(500, 500);
       
        int[] biomesInt = new int[8] { 0,0,0,1,1,2,3,4};

        //go through each tile in the map
        for (int y = 0; y < 500; y++)
        {
            for(int x = 0; x < 500; x++)
            {
                float[] distances = new float[] { Vector2.Distance(new Vector2(x, y), Points[0]), Vector2.Distance(new Vector2(x, y), Points[1]), Vector2.Distance(new Vector2(x, y), Points[2]), Vector2.Distance(new Vector2(x, y), Points[3]), Vector2.Distance(new Vector2(x, y), Points[4]), Vector2.Distance(new Vector2(x, y), Points[5]), Vector2.Distance(new Vector2(x, y), Points[6]), Vector2.Distance(new Vector2(x, y), Points[7]) };
                int[] orderTest = orderedArray(distances);

                 // get the bioem in each pixel and set the colour for each of its
                 // maps depending on the the biome
                baseColorMap.SetPixel(x, y, biomes[biomesInt[orderTest[0]]].baseColor);
                biomeMap.SetPixel(x, y, biomes[biomesInt[orderTest[0]]].baseColor);
                riverColorMap.SetPixel(x, y, biomes[biomesInt[orderTest[0]]].riverColor);
                slightColorMap.SetPixel(x, y, biomes[biomesInt[orderTest[0]]].slightColor);
                steepColorMap.SetPixel(x, y, biomes[biomesInt[orderTest[0]]].steepColor);


            }


        }

        // blur all the gaps between the biomes
        baseColorMap = new LinearBlur().Blur(baseColorMap, 10, 10);
        riverColorMap = new LinearBlur().Blur(riverColorMap, 10, 10);
        slightColorMap = new LinearBlur().Blur(slightColorMap, 10, 10);
        steepColorMap = new LinearBlur().Blur(steepColorMap, 10, 10);


        biomeMap.Apply();
        baseColorMap.Apply();
        riverColorMap.Apply();
        slightColorMap.Apply();
        steepColorMap.Apply();


        // create a dictionary of what goes where
        createBDict();


        // create temp and humidity base maps using biomes
        tempMap = new Texture2D(25, 25);
        humMap = new Texture2D(25, 25);
        for (int x = 0; x < 25; x++)
        {
            for(int y = 0; y < 25; y++)
            {
                float tempTemp = biomeDict[new Vector2(x * 20, y * 20)].biomeTemp;
                float tempHum = biomeDict[new Vector2(x * 20, y * 20)].biomeHum;
                tempMap.SetPixel(x, y, new Color(tempTemp, tempTemp, tempTemp));
                humMap.SetPixel(x, y,new Color(tempHum, tempHum,tempHum));
            }
        }
        tempMap.Apply();
        humMap.Apply();
       
      

    }


    // create a dictionary of what biome goes wehere according to vector2 pos
    public void createBDict()
    {
        biomeDict = new Dictionary<Vector2, Biome>();
        for (int y = 0; y < 500; y++)
        {
            for (int x = 0; x < 500; x++)
            { // go through each tile

                Color col = biomeMap.GetPixel(x, y); // get what biome
                
                for(int i = 0; i < biomes.Length; i++)
                {
                    if (Mathf.Abs(col.g - biomes[i].baseColor.g)<0.02f) // some reason not exact so do this
                    {
                        biomeDict.Add(new Vector2(x, y), biomes[i]);
                       // print("!");
                        break;
                    }
                }

            
    
            }


        }
      

    }


    // take in an unordered array and return in ordered
    private int[] orderedArray(float[] distances)
    {

        int[] ordered = new int[8];
        Dictionary<int, int> ignores = new Dictionary<int, int>();
      
        for (int t = 0; t < 8; t++)
        {
            float min = 1000000; // big number probably no items higher than that
            int minIndex = 0;

            for (int i = 0; i < 8; i++)
            {
                if (distances[i] < min && ignores.ContainsKey(i)==false)
                {
                    min = distances[i];
                    minIndex = i;
                }
            }
            ordered[t] = minIndex;
            ignores.Add(minIndex, minIndex);

        }
        return ordered;
    }
}
