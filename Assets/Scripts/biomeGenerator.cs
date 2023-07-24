using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class biomeGenerator : MonoBehaviour
{
    
    public Texture2D baseColorMap;
    public Texture2D steepColorMap;
    public Texture2D slightColorMap;
    public Texture2D riverColorMap;
    public Dictionary<Vector2, Biome> biomeDict;
    [System.Serializable]
    public class objectPlacement
    {
        public GameObject obj;
        public float radius;

    }
    [System.Serializable]
    public class Biome
    {
        public string name;

        public Color baseColor;
        public Color steepColor;
        public Color slightColor;
        public Color riverColor;

        public float steepGradient;
        public float slightGradient;

        public float humidity;
        public float temperature;
        public GameObject[] smallObjs;
        public objectPlacement[] trees;
        public bool lotsOfGrass;
    }
    public Biome[] biomes; // 0 - grass      1- winter   2-DEsert   3-Jungle   4- swamp
   
    public void generateBiomes()
    {
        biomeDict = new Dictionary<Vector2, Biome>();
        PoissonSampling PS = new PoissonSampling();
        int count = 0;
        List<Vector2> Points=new List<Vector2>();
        while (count != 8)
        {
        Points = PS.GetPoissonPoints(10, 100, 500, protectEdge: false);
            count = Points.Count;
        }

        // Debug.Log(Points.Count);
        baseColorMap = new Texture2D(500, 500);
        riverColorMap=new Texture2D(500, 500);
        slightColorMap=new Texture2D(500, 500);
        steepColorMap =new Texture2D(500, 500);
        int[] biomesInt = new int[8] { 0,0,0,1,1,2,3,4};
        for (int y = 0; y < 500; y++)
        {
            for(int x = 0; x < 500; x++)
            {
                float[] distances = new float[] { Vector2.Distance(new Vector2(x, y), Points[0]), Vector2.Distance(new Vector2(x, y), Points[1]), Vector2.Distance(new Vector2(x, y), Points[2]), Vector2.Distance(new Vector2(x, y), Points[3]), Vector2.Distance(new Vector2(x, y), Points[4]), Vector2.Distance(new Vector2(x, y), Points[5]), Vector2.Distance(new Vector2(x, y), Points[6]), Vector2.Distance(new Vector2(x, y), Points[7]) };
                int[] orderTest = orderedArray(distances);
             //   Debug.Log(distances[orderTest[0]]);
              //  Debug.Log(distances[orderTest[1]]);
                
                    baseColorMap.SetPixel(x, y, biomes[biomesInt[orderTest[0]]].baseColor);
                riverColorMap.SetPixel(x, y, biomes[biomesInt[orderTest[0]]].riverColor);
                slightColorMap.SetPixel(x, y, biomes[biomesInt[orderTest[0]]].slightColor);
                steepColorMap.SetPixel(x, y, biomes[biomesInt[orderTest[0]]].steepColor);

                biomeDict.Add(new Vector2(x, y), biomes[biomesInt[orderTest[0]]]);
            }


        }
       // baseColorMap = new LinearBlur().Blur(baseColorMap, 10, 10);
        riverColorMap = new LinearBlur().Blur(riverColorMap, 10, 10);
        slightColorMap = new LinearBlur().Blur(slightColorMap, 10, 10);
        steepColorMap = new LinearBlur().Blur(steepColorMap, 10, 10);

        baseColorMap.Apply();
        riverColorMap.Apply();
        slightColorMap.Apply();
        steepColorMap.Apply();
      
        



    }



    int[] orderedArray(float[] distances)
    {

        int[] ordered = new int[8];
        Dictionary<int, int> ignores = new Dictionary<int, int>();
       // List<int> ignores = new List<int>();
        for (int t = 0; t < 8; t++)
        {
            float min = 1000000;
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
