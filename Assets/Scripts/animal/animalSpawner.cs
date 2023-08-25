using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* This script deals with putting the animals in the terrain it doesnt worry about
 * The animal stats / genetics thats done by the animal creator class
 */

public class animalSpawner : MonoBehaviour
{
   
    public Texture2D availibles; // where in the map is availible to spawn

    // references to other classes
    [SerializeField] private  PlacementManager PM;
    [SerializeField] private timeController TC;
    [SerializeField] private biomeGenerator BG;
    public weatherManager WM;

    // result of the all floodfilled areas this holds all the different 'islands'
    // which can be spawned in
    private Dictionary<int, RiverGen.riverData> spawns = new Dictionary<int, RiverGen.riverData>();

    // only has Terrain layer in so we use that for raycasts to find height of mesh
    private LayerMask layerMask;

    // holds all the acceptable places to spawn
    public Dictionary<Vector2, int> allSpawns= new Dictionary<Vector2, int>();

    [SerializeField] private int spawnCount; // how many animals we want to spawn
    
    private biomeGenerator.Biome[] possBiomes; // the biomes in the world

    // black and white version of availible spawns Texture
    public Texture2D nonColouredAvs;

    [SerializeField] private Grid grid; // reference to pathfinding grid

    [SerializeField] private Transform effectsLoc; // where in the heirachy to spawn effects

    // use this to make it serializeable easier for dubugging without performance drop
    [System.Serializable]
    public class biomeVECS{

     public  List<Vector2> poss; // list of where you can spawn in each biome
        
        public  biomeVECS() // constructor
        {
            poss = new List<Vector2>();
        }
    }

    public List<biomeVECS> biomePoss; // have a BiomeVecs class for each biome in a list



    // gets called when animals are ready to spawn in on the start
    public void StartAnimals()
    {
        // dictionary of what biome is in what tile
        Dictionary<Vector2, biomeGenerator.Biome> biomeDict = BG.biomeDict;

        // create a layer mask for raycasting
        layerMask = LayerMask.GetMask("islandBaseMesh");

        // get the availbile biomes and add them to a list
        possBiomes = BG.biomes;

        for (int i = 0; i < 5; i++)
            biomePoss.Add(new biomeVECS());


        // go through each tile 
        for (int y = 0; y < 250; y++)
        {
            for (int x = 0; x < 250; x++)
            {
                if (nonColouredAvs.GetPixel(x, y) != Color.black)// if tile availible
                {
                    // add it to that biomes list of possible spawns
                    biomePoss[BG.biomeDict[new Vector2(x * 2, y * 2)].Index].poss.Add(new Vector2(x, y));
                }
            }

        }

        // sum of all the possible spawn locations in all the biomes
        int totCount = biomePoss[0].poss.Count + biomePoss[1].poss.Count + biomePoss[2].poss.Count + biomePoss[3].poss.Count + biomePoss[4].poss.Count;


        // go through all biomes
        for (int i = 0; i < 5; i++)// go thru biomes
        {

            // This calcualtion in the for loop makes sure it does a correct ratio of animals in the biome
            // for how large the biome actaully is so you dont get 200 animals in a tiny island

            for (int i1 = 0; i1 < (int)(((float)biomePoss[i].poss.Count / (float)totCount) * spawnCount); i1++)
            {
                // array of the animals that could be in this biome
                GameObject[] animals = possBiomes[i].animals;
                GameObject animal;


                if (animals.Length != 0)
                    animal = animals[Random.Range(0, animals.Length)];
                else
                    animal = null;


                if (animal != null) // error checking
                {

                    GameObject thisAnimal = Instantiate(animal);

                    // were setting up the animal from scratch so we need to
                    // manuall set up all its parameters below
                    Vector2 spawnpos = biomePoss[i].poss[Random.Range(0, biomePoss[i].poss.Count)];
                    thisAnimal.transform.position = getMeshPos(spawnpos);
                    Vector2 temp = allSpawns.Keys.ToArray<Vector2>()[Random.Range(0, allSpawns.Count)];

                    thisAnimal.GetComponent<animalController>().TC = TC;
                    thisAnimal.GetComponent<animalController>().spawner = this;
                    thisAnimal.GetComponent<animalSight>().baseAvailibles = nonColouredAvs;
                    thisAnimal.GetComponent<animalEating>().setEffectsLoc(effectsLoc);
                    thisAnimal.GetComponent<animalController>().effectsLoc = effectsLoc;
                    thisAnimal.GetComponent<animalController>().BG = BG;
                    thisAnimal.GetComponent<AnimalCreator>().age = Random.Range(0, 14);
                    if(thisAnimal.GetComponent<AnimalCreator>()!=null)
                        thisAnimal.GetComponent<AnimalCreator>().initiate();

                    thisAnimal.transform.parent = transform;
                }
            }

        }
    }

    // Get the Vector3 location from a Vector2 with correct height
    public Vector3 getMeshPos(Vector2 pos)
    {
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(pos.x, 200, pos.y), Vector3.down, out hit, Mathf.Infinity, layerMask))
            return hit.point;

        return Vector3.zero;
    }

    // This generates where animals can spawn
    public void genSpawns()
    {
        // create blank textures  for nonColouredAvs texture by creating texture
        // then creating an array of all black pixels and assigning it
        nonColouredAvs = new Texture2D(250, 250);
        Color[] allBlack = new Color[250*250];
        for (int i = 0; i < 250 * 250; i++)
        {
            allBlack[i] = Color.black;
        }
        nonColouredAvs.SetPixels(allBlack);
        nonColouredAvs.Apply();

        // For the main availibles texture set it to the data calculated in placement manager
        availibles = new Texture2D(250, 250);
        availibles.SetPixels(PM.movAv2.GetPixels());
        availibles.Apply();
  
        Color[] cols = availibles.GetPixels();

        for (int i = 0; i < cols.Length; i++) // go through array
        {
            if (cols[i].r == 1) // if placement manager texture is red pixel set that tile to be availible
            {
                cols[i] = Color.white;
            }
        }
        availibles.SetPixels(cols);
        availibles.Apply();



   
        // Go through the 250x250 map at an inteval of 2m
        for (int x = 0; x < 250; x += 2)
        {
            for (int y = 0; y < 250; y += 2)
            {

                if (availibles.GetPixel(x, y).g == 1) // if the tile is availible
                {
                    // floodfill that position in the availible map
                    RiverGen.riverData riverpoints = FloodFill(new Vector2(x, y));

                     // make sure the spawn location has 6000m^2 availible so
                     // the animal isnt stranded
                    if (riverpoints.know.Count > 6000)
                    {
                        spawns.Add(spawns.Count, riverpoints);
                        UpdateImg(riverpoints.know);
                    }
                }

            }
        }

        // add the spawns created into the allSpawns list
        for (int i = 0; i < spawns.Count; i++)
        {
            for (int j = 0; j < spawns[i].know.Count; j++)
            {
                allSpawns.Add(spawns[i].know[j], 0);
            }
        }
     
    }

    // Updates the availibles image with if the pixel is OK to spawn in or not
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


    // A slightly adjusted version of the floodfill algorithm used for lake generation
    RiverGen.riverData FloodFill(Vector2 start)
    {
        // initialise queue with first tile
        Queue<Vector2> queue = new Queue<Vector2>();
        queue.Enqueue(start);

        // dictionaries arent needed for floodfill itself but provide useful fast
        // data later for us hence why we return the data in a different struct
        Dictionary<int, Vector2> know = new Dictionary<int, Vector2>();
        Dictionary<Vector2, int> knowkeys = new Dictionary<Vector2, int>();

        // while theres more to search look in all directions around us
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
        // create a new riverData struct and put the dicts in there
        RiverGen.riverData RD = new RiverGen.riverData();
        RD.know = know;
        RD.knowkeys = knowkeys;
        return RD;
    }

}
