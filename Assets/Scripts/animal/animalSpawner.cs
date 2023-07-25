using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class animalSpawner : MonoBehaviour
{
    [SerializeField] GameObject Rabbit;
    // Start is called before the first frame update
    public Texture2D availibles;
    [SerializeField] PlacementManager PM;
    Dictionary<int, RiverGen.riverData> spawns = new Dictionary<int, RiverGen.riverData>();
   // public Texture2D tex;
    LayerMask layerMask;
    public Dictionary<Vector2, int> allSpawns= new Dictionary<Vector2, int>();
    [SerializeField] timeController TC;
    [SerializeField] int spawnCount;
    [SerializeField] biomeGenerator BG;
    biomeGenerator.Biome[] possBiomes;
   public Texture2D nonColouredAvs;
    [SerializeField] Grid grid;
    [SerializeField] Transform effectsLoc;
   public weatherManager WM;
    
    [System.Serializable]
  public  class biomeVECS{ // use this to make it serializeable easier for dubugging without performance drop
     public   List<Vector2> poss;
        
        public  biomeVECS()
        {
            poss = new List<Vector2>();
        }
    }
    public List<biomeVECS> biomePoss;
    Texture2D biomeMapp;
    int totalPositions;
    public void StartAnimals()
    {
        Dictionary<Vector2, biomeGenerator.Biome> biomeDict = BG.biomeDict;
        layerMask = LayerMask.GetMask("islandBaseMesh");
        possBiomes = BG.biomes;
        biomeMapp = BG.biomeMap;
        // biomeMapp = BG.biomeMap;

        for (int i = 0; i < 5; i++)
            biomePoss.Add(new biomeVECS());


        for (int y = 0; y < 250; y++)
        {
            for (int x = 0; x < 250; x++)
            {
                if (nonColouredAvs.GetPixel(x, y) != Color.black)
                {
                    biomePoss[BG.biomeDict[new Vector2(x * 2, y * 2)].Index].poss.Add(new Vector2(x, y));
                }
            }

        }

        int totCount = biomePoss[0].poss.Count + biomePoss[1].poss.Count + biomePoss[2].poss.Count + biomePoss[3].poss.Count + biomePoss[4].poss.Count;

        for (int i = 0; i < 5; i++)// go thru biomes
        {
          
            for (int i1 = 0; i1 < (int)(((float)biomePoss[i].poss.Count / (float)totCount) * spawnCount); i1++)
            {
                GameObject[] animals = possBiomes[i].animals;
                GameObject animal;
                if (animals.Length != 0)
                    animal = animals[Random.Range(0, animals.Length)];
                else
                    animal = null;
                if (animal != null)
                {
                    GameObject test = Instantiate(animal);
                  
                    Vector2 spawnpos = biomePoss[i].poss[Random.Range(0, biomePoss[i].poss.Count)];
                    test.transform.position = getMeshPos(spawnpos);
                    Vector2 temp = allSpawns.Keys.ToArray<Vector2>()[Random.Range(0, allSpawns.Count)];

                    test.GetComponent<animalController>().TC = TC;
                    test.GetComponent<animalController>().spawner = this;
                    test.GetComponent<animalSight>().baseAvailibles = nonColouredAvs;
                    test.GetComponent<animalChomp>().effectsLocation = effectsLoc;
                    test.GetComponent<animalController>().effectsLoc = effectsLoc;
                    test.GetComponent<animalController>().BG = BG;
                    test.GetComponent<AnimalCreator>().age = Random.Range(0, 14);
                    if(test.GetComponent<AnimalCreator>()!=null)
                    test.GetComponent<AnimalCreator>().initiate();
                //    test.GetComponent<animalPath>().grid = grid;
                    test.transform.parent = transform;
                }
            }

        }
    }


    public Vector3 getMeshPos(Vector2 pos)
    {
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(pos.x, 200, pos.y), Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            //  Debug.DrawRay(new Vector3(pos.x, 200, pos.y), Vector3.down*10, Color.green);
            return hit.point;
        }
        return Vector3.zero;
    }
   public void genSpawns()
    {
        nonColouredAvs = new Texture2D(250, 250);
        Color[] allBlack = new Color[250*250];
        for (int i = 0; i < 250 * 250; i++)
        {
            allBlack[i] = Color.black;
        }
        nonColouredAvs.SetPixels(allBlack);
        nonColouredAvs.Apply();

        availibles = new Texture2D(250, 250);
        availibles.SetPixels(PM.movAv2.GetPixels());
        availibles.Apply();
     //   tex = new Texture2D(250, 250);
        Color[] cols = availibles.GetPixels();

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].r == 1)
            {
                cols[i] = Color.white;
            }
        }
        availibles.SetPixels(cols);
        availibles.Apply();
        //  spawns.Clear();

        for (int x = 0; x < 250; x += 2)
        {
            for (int y = 0; y < 250; y += 2)
            {
                if (availibles.GetPixel(x, y).g == 1)
                {
                    RiverGen.riverData riverpoints = FloodFill(new Vector2(x, y));
                    if (riverpoints.know.Count > 6000)
                    {
                        spawns.Add(spawns.Count, riverpoints);
                        UpdateImg(riverpoints.know);
                    }
                }

            }
        }

        // print(spawns.Count);
        for (int i = 0; i < spawns.Count; i++)
        {
            for (int j = 0; j < spawns[i].know.Count; j++)
            {
                allSpawns.Add(spawns[i].know[j], 0);
            }
        }
      //  print("spawn count"+ allSpawns.Count.ToString());
    }
    void UpdateImg(Dictionary<int, Vector2> poin)
    {
        Color col = Color.HSVToRGB(Random.value, 1, 1);
        for (int i = 0; i < poin.Count; i++)
        {
            availibles.SetPixel((int)poin[i].x, (int)poin[i].y, col);
            nonColouredAvs.SetPixel((int)poin[i].x, (int)poin[i].y, Color.red);
        }
        availibles.Apply();
        nonColouredAvs.Apply();
    }
    RiverGen.riverData FloodFill(Vector2 start)
    {
        Queue<Vector2> queue = new Queue<Vector2>();
        queue.Enqueue(start);
        //   List<Vector2> know = new List<Vector2>();
        Dictionary<int, Vector2> know = new Dictionary<int, Vector2>();
        Dictionary<Vector2, int> knowkeys = new Dictionary<Vector2, int>();
        while (queue.Count > 0)
        {

            Vector2 temp = queue.Peek() + new Vector2(-1, 0);
            if (temp.x > 0 && !queue.Contains(temp) == true && availibles.GetPixel((int)temp.x, (int)temp.y).r == 1 && knowkeys.ContainsKey(temp) == false)
                queue.Enqueue(temp);

            temp = queue.Peek() + new Vector2(1, 0);
            if (temp.x < 250 && !queue.Contains(temp) == true && availibles.GetPixel((int)temp.x, (int)temp.y).r == 1 && knowkeys.ContainsKey(temp) == false)
                queue.Enqueue(temp);

            temp = queue.Peek() + new Vector2(0, 1);
            if (temp.y < 250 && !queue.Contains(temp) == true && availibles.GetPixel((int)temp.x, (int)temp.y).r == 1 && knowkeys.ContainsKey(temp) == false)
                queue.Enqueue(temp);

            temp = queue.Peek() + new Vector2(0, -1);
            if (temp.y > 0 && !queue.Contains(temp) == true && availibles.GetPixel((int)temp.x, (int)temp.y).r == 1 && knowkeys.ContainsKey(temp) == false)
                queue.Enqueue(temp);

            know.Add(know.Count, queue.Peek());
            knowkeys.Add(queue.Peek(), know.Count);
            queue.Dequeue();
        }
        RiverGen.riverData RD = new RiverGen.riverData();
        RD.know = know;
        RD.knowkeys = knowkeys;
        return RD;
    }

}
