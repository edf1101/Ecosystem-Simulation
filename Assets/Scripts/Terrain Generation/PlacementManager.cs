using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public Texture2D availibles;
    [SerializeField] NoiseGen NG;
    [SerializeField ] float waterThresh;
    [SerializeField] int realSize=250;
    [SerializeField] float maxGrad;
    [SerializeField] Transform objHolder;
     Texture2D baseAvailibles;
    public Texture2D treeArea;
    [SerializeField] AnimationCurve treeCurve;
    public Texture2D treeAv;
    [SerializeField] private LayerMask layermask;
    [SerializeField] float maxTreeHeight;
    [SerializeField] GameObject controller;
     Texture2D moveAv;
    public Texture2D movAv2;
    public Texture2D movAv3;

    public Texture2D cheapAv;
    public Texture2D cheapAv2;
    public void Begin()
    {
        int childs = objHolder.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(objHolder.transform.GetChild(i).gameObject);
        }

        Texture2D perl = NG.combo;
        Texture2D rivers = NG.riv3;
        availibles = new Texture2D(1000, 1000);
        Vector3[] verts = GetComponent<TerrainGenerator>().mesh.vertices;
        treeArea =NG.curve2Tex( NG.SinglePerlin(500, 3, new Vector2(Random.Range(0, 100000), Random.Range(0, 100000)), 1),treeCurve);
        baseAvailibles=new Texture2D(1000, 1000);
        treeAv = new Texture2D(1000, 1000);
        moveAv = new Texture2D(1000, 1000);
        movAv2 = new Texture2D(250, 250);
        movAv3 = new Texture2D(250, 250);
        cheapAv = new Texture2D(1000, 1000);
        cheapAv2 = new Texture2D(250, 250);
        for (int y = 0; y < 1000; y++)
        {
            for(int x = 0; x < 1000; x++)
            {
                if (perl.GetPixel(x,y).r>waterThresh)
                    availibles.SetPixel(x, y,Color.red);
                else
                    availibles.SetPixel(x, y, Color.black);

                float thresh = 0.012f;
                if (rivers.GetPixel(x,y).r>  thresh )
                    availibles.SetPixel(x, y,Color.black);


                RaycastHit hit;

                if(Physics.Raycast(new Vector3(x/(1000/ realSize),200,y/ (1000 / realSize)), Vector3.down, out hit, Mathf.Infinity,layermask))
                {
                    int i = hit.triangleIndex;

                          Vector3 v1 = verts[i * 3];
                    Vector3 v2 = verts[(i * 3) + 1];
                    Vector3 v3 = verts[(i * 3) + 2];
                    float minY = Mathf.Min(Mathf.Min(v1.y, v2.y), v3.y);
                    float maxY = Mathf.Max(Mathf.Max(v1.y, v2.y), v3.y);
                    float minZ = Mathf.Min(Mathf.Min(v1.z, v2.z), v3.z);
                    float maxZ = Mathf.Max(Mathf.Max(v1.z, v2.z), v3.z);
                    float minX = Mathf.Min(Mathf.Min(v1.x, v2.x), v3.x);
                    float maxX = Mathf.Max(Mathf.Max(v1.x, v2.x), v3.x);
                    float gradient = (maxY - minY) / Mathf.Sqrt(Mathf.Pow(maxX - minX, 2) + Mathf.Pow(maxZ - minZ, 2));
                    if (gradient>maxGrad)
                        availibles.SetPixel(x, y, Color.black);
                }
            }
        }
        availibles.Apply();
        baseAvailibles.SetPixels(availibles.GetPixels());
        baseAvailibles.Apply();
        moveAv.SetPixels(baseAvailibles.GetPixels());
        cheapAv.SetPixels(baseAvailibles.GetPixels());
        cheapAv.Apply();
        moveAv.Apply();
        
    }
    

    public void addTrees()
    {
        treeAv.SetPixels(baseAvailibles.GetPixels());
        treeAv.Apply();
       
        int amount = 3000;
        int attempts = 1000;
        Dictionary<Vector2, biomeGenerator.Biome> biomeMap = GetComponent<biomeGenerator>().biomeDict;
        for (int i = 0; i < amount; i++)
        {
            
            bool good = false;
            int cAmm = 0;
            while (good == false)
            {
                cAmm++;
                if (cAmm > attempts)
                {
                  //  Debug.Log("too many");
                    break;
                }
                Vector2 testPoint = new Vector2(Random.Range(0, 1000), Random.Range(0, 1000));
                if (treeArea.GetPixel((int)testPoint.x, (int)testPoint.y).r > Random.value)
                {
                    good = true;
                  //  Debug.Log("not in wood area");


                }
                else
                {
                    biomeGenerator.Biome cBiome = biomeMap[new Vector2((int)testPoint.x / 2, (int)testPoint.y / 2)];
                    int tempNum = Random.Range(0, cBiome.trees.Length);
                    GameObject tree = cBiome.trees[tempNum].obj;
                    int rad = (int)cBiome.trees[tempNum].radius;
                    if (tree == null)
                    {
                        good = true;
                     //   Debug.Log("fake tree");
                        break;
                    }
                    if (possible(testPoint, rad)  )
                    {
                      //  Debug.Log("good");
                        good = true;

                        
                       

                       
                        
                        RaycastHit hit;
                        if (Physics.Raycast(new Vector3(testPoint.x / 4f, 200, testPoint.y / 4f), Vector3.down, out hit, Mathf.Infinity, layermask)&&hit.point.y>maxTreeHeight)
                        {
                            good = false;
                        }
                        else if (Physics.Raycast(new Vector3(testPoint.x / 4f, 200, testPoint.y / 4f), Vector3.down, out hit, Mathf.Infinity, layermask))
                        {
                            GameObject tempObj = Instantiate(tree);
                            tempObj.transform.parent = objHolder;
                            tempObj.transform.position = hit.point;
                            tempObj.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                            tempObj.transform.localScale = tempObj.transform.localScale * Random.Range(0.8f, 1.2f);
                            for (int y = Mathf.Max(0, (int)testPoint.y - (rad*3)); y < Mathf.Min(1000, testPoint.y + (rad*3)); y++)
                            {
                            for (int x = Mathf.Max(0, (int)testPoint.x - (3 * rad)); x < Mathf.Min(1000, testPoint.x + (3 * rad)); x++)
                            {
                                if (Vector2.Distance(new Vector2(x, y), testPoint) < rad*1.5f)
                                    treeAv.SetPixel(x, y, Color.green);
                                    if (Vector2.Distance(new Vector2(x, y), testPoint) < 5)
                                    {
                                        availibles.SetPixel(x, y, Color.green);
                                        moveAv.SetPixel(x, y, Color.black);
                                        cheapAv.SetPixel(x, y, Color.black);
                                        
                                    }

                            }
                            }
                            treeAv.Apply();
                            availibles.Apply();
                            moveAv.Apply();
                           cheapAv.Apply();
                            break;
                        }




                    }
                    else
                    {
                     //   Debug.Log("bad too closes");
                    }


                }
            }
            if (cAmm > attempts)
                break;




        }
    }

    public void addFlowers()
    {
        
        int amount = 5000;
        int attempts = 1000;
        Dictionary<Vector2, biomeGenerator.Biome> biomeMap = GetComponent<biomeGenerator>().biomeDict;
        for (int i = 0; i < amount; i++)
        {

            bool good = false;
            int cAmm = 0;
            while (good == false)
            {
                cAmm++;
                if (cAmm > attempts)
                {
                    //  Debug.Log("too many");
                    break;
                }
                Vector2 testPoint = new Vector2(Random.Range(0, 1000), Random.Range(0, 1000));
                
                
                    biomeGenerator.Biome cBiome = biomeMap[new Vector2((int)testPoint.x / 2, (int)testPoint.y / 2)];
                    int tempNum = Random.Range(0, cBiome.smallObjs.Length);
                    GameObject tree = cBiome.smallObjs[tempNum];
                    
                    if (tree == null)
                    {
                        good = true;
                        //   Debug.Log("fake tree");
                        break;
                    }
                    if (possible(testPoint, 3))
                    {
                      //  Debug.Log("good");
                        good = true;

                        for (int y = Mathf.Max(0, (int)testPoint.y - (1 * 3)); y < Mathf.Min(1000, testPoint.y + (1 * 3)); y++)
                        {
                            for (int x = Mathf.Max(0, (int)testPoint.x - (3 * 1)); x < Mathf.Min(1000, testPoint.x + (3 * 1)); x++)
                            {

                            if (Vector2.Distance(new Vector2(x, y), testPoint) < 4)
                            {
                                availibles.SetPixel(x, y, Color.blue);
                                if(tree.GetComponent<Collider>()!=null)
                                    moveAv.SetPixel(x, y, Color.black);
                                if(tree.GetComponent<isRock>()!=null)
                                    cheapAv.SetPixel(x, y, Color.black);

                            }
                        }
                        }


                        treeAv.Apply();
                        availibles.Apply();
                    moveAv.Apply();
                    cheapAv.Apply();
                        GameObject tempObj = Instantiate(tree);
                        tempObj.transform.parent = objHolder;
                        RaycastHit hit;
                       
                        if (Physics.Raycast(new Vector3(testPoint.x / 4f, 200, testPoint.y / 4f), Vector3.down, out hit, Mathf.Infinity, layermask))
                        {
                            tempObj.transform.position = hit.point;
                            tempObj.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                        tempObj.transform.localScale=tempObj.transform.localScale*Random.Range(0.8f,1.2f);
                      
                        break;
                        }




                    }
                    else
                    {
                        //   Debug.Log("bad too closes");
                    }


                
            }
            if (cAmm > attempts)
                break;




        }
        test();
    }
    void test()
    {
        movAv2 = new Texture2D(250, 250);
        cheapAv2 = new Texture2D(250, 250);
        for (int y=0; y < 250; y++)
        {
            for (int x=0; x < 250; x++)
            {
                float rAvg = 0;
                float rAvg2 = 0;
                
                for (int x1=0; x1<4; x1++)
                {
                    for (int y1=0; y1<4; y1++)
                    {
                        rAvg += moveAv.GetPixel((x * 4) + x1, (y * 4) + y1).r;
                        rAvg2 += cheapAv.GetPixel((x * 4) + x1, (y * 4) + y1).r;
                    }
                }
                rAvg = rAvg / 16f;
                rAvg2 = rAvg2 / 16f;

                if (rAvg==1)
                    movAv2.SetPixel(x, y, Color.red);
                else
                    movAv2.SetPixel(x, y,Color.black); 
                if (rAvg2==1)
                    cheapAv2.SetPixel(x, y, Color.red);
                else
                    cheapAv2.SetPixel(x, y,Color.black);
            }
        }
        movAv2.Apply();
        cheapAv2.Apply();
    }

    public bool possible(Vector2 pos,int radius)
    {
        for (int y = Mathf.Max(0,(int)pos.y - radius); y < Mathf.Min(1000,pos.y + radius); y++)
        { 
            for (int x = Mathf.Max(0, (int)pos.x - radius); x < Mathf.Min(1000,pos.x + radius); x++)
            {
                if (treeAv.GetPixel(x, y).r != 1)
                    return false;
            }

        }
        return true;
    }

}
