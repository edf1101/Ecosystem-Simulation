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
   public void StartAnimals()
    {
        layerMask = LayerMask.GetMask("islandBaseMesh");
        //genSpawns();
        for (int i = 0; i <400; i++)
        {
            GameObject test=Instantiate(Rabbit);
            test.transform.position=getMeshPos(allSpawns.Keys.ToArray<Vector2>()[Random.Range(0,allSpawns.Count)]);
            Vector2 temp= allSpawns.Keys.ToArray<Vector2>()[Random.Range(0, allSpawns.Count)];
            
            test.GetComponent<animalController>().TC=TC;
            test.GetComponent<animalController>().spawner = this;
            test.transform.parent = transform;
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
                    if (riverpoints.know.Count > 2000)
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
        }
        availibles.Apply();

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
