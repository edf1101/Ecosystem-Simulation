using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterPlaces : MonoBehaviour
{
    public Texture2D wheresWater;
  public  Texture2D nearWater;
    public Texture2D near2;
    public Texture2D where2;
    [SerializeField] Transform markerHolder;
    [SerializeField] GameObject markerPrefab;
    public Texture2D floraPos;
  public  void findWater()
    {
        wheresWater = new Texture2D(250, 250);
        Texture2D avs = GetComponent<animalSpawner>().availibles;
        nearWater = new Texture2D(250, 250);
        int tempcount=0;
        for (int y=0; y < 250; y++)
        {
            for (int x=0; x < 250; x++)
            {
                nearWater.SetPixel(x, y, Color.black);
                int thresh = 16;

                RaycastHit hit;

                if (Physics.Raycast(new Vector3(x, 200, y), Vector3.down, out hit, Mathf.Infinity))
                {
                    //  Debug.DrawRay(new Vector3(pos.x, 200, pos.y), Vector3.down*10, Color.green);
                    if (hit.collider.gameObject.layer == 4)
                    {
                        wheresWater.SetPixel(x, y, Color.blue);
                        for(int x1 = Mathf.Max(16, x - 5); x1< Mathf.Min(234, x + 5); x1++)
                        {
                            for (int y1 = Mathf.Max(016, y- 5); y1< Mathf.Min(234,y + 5); y1++)
                            {
                                
                                if (avs.GetPixel(x1, y1) != Color.black)
                                {
                                    nearWater.SetPixel(x1, y1, Color.red);
tempcount++;
                                }
                                
                            }
                        }
                    }
                        
                    else
                        wheresWater.SetPixel(x,y, Color.black);
                }
                if(y<thresh || y>250-thresh || x<thresh || x>250-thresh)
                    wheresWater.SetPixel(x, y, Color.black);

            }
        }
       
        wheresWater.Apply();
        
        
        nearWater.Apply();
        near2 = new Texture2D(250, 250);
        for (int y = 0; y < 250; y++)
        {
            for (int x = 0; x < 250; x++)
            {
                if(nearWater.GetPixel(x, y) !=Color.black&& !(nearWater.GetPixel(x+1, y) != Color.black&& nearWater.GetPixel(x-1, y) != Color.black&&nearWater.GetPixel(x, y+1) != Color.black&&nearWater.GetPixel(x, y-1) != Color.black))
                {
                    near2.SetPixel(x, y, Color.red);
                  
                    
                }
                else
                    near2.SetPixel(x, y, Color.black);
            }
        }

            near2.Apply();
        nearWater.Apply();


        where2 = new Texture2D(250, 250);
        for (int y = 0; y < 250; y++)
        {
            for (int x = 0; x < 250; x++)
            {
                if (wheresWater.GetPixel(x, y) != Color.black && !(wheresWater.GetPixel(x + 1, y) != Color.black && wheresWater.GetPixel(x - 1, y) != Color.black && wheresWater.GetPixel(x, y + 1) != Color.black && wheresWater.GetPixel(x, y - 1) != Color.black))
                {
                    where2.SetPixel(x, y, Color.red);


                }
                else
                    where2.SetPixel(x, y, Color.black);
            }
        }

        where2.Apply();

        for (int y = 0; y < 250; y++)
        {
            for (int x = 0; x < 250; x++)
            {
                if (near2.GetPixel(x, y) != Color.black)
                {
                    //find nearest water
                    float minDis = 1000;
                    Vector2 minPos = Vector2.zero;
                    for (int x1 = Mathf.Max(16, x - 5); x1 < Mathf.Min(234, x + 5); x1++)
                    {
                        for (int y1 = Mathf.Max(016, y - 5); y1 < Mathf.Min(234, y + 5); y1++)
                        {

                            if (where2.GetPixel(x1, y1) != Color.black&& Vector2.Distance(new Vector2(x,y),new Vector2(x1,y1)) < minDis )
                            {
                                minDis = Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1));
                                minPos = new Vector2(x1, y1);
                            }

                        }
                    }
                    if (minPos != Vector2.zero)
                    {
                        RaycastHit hit;

                        if (Physics.Raycast(new Vector3(x, 200, y), Vector3.down, out hit, Mathf.Infinity,LayerMask.GetMask("islandBaseMesh")))
                        {
                            GameObject marker = Instantiate(markerPrefab);
                            marker.transform.position = hit.point;
                            marker.transform.parent = markerHolder;
                            marker.GetComponent<markerController>().nearWater = minPos;
                            marker.GetComponent<markerController>().type = "Water";
                        }
                    }
                }
            }
        }

        // put flora markers on


        floraPos = new Texture2D(250, 250);
        Color[] cols = new Color[250 * 250];
        for(int i = 0; i < 250 * 250; i++)
        {
            cols[i] = Color.black;
        }
        floraPos.SetPixels(cols);
       Collider[] colls= Physics.OverlapSphere(new Vector3(125, 0, 125), 200000, LayerMask.GetMask("Flora"),QueryTriggerInteraction.Collide);
      //  print(colls.Length);
        foreach (Collider coll in colls)
        {
            floraPos.SetPixel((int)coll.transform.position.x, (int)coll.transform.position.z, Color.red);
        }
        floraPos.Apply();


        for (int y = 0; y < 250; y++)
        {
            for (int x = 0; x < 250; x++)
            {
                if (floraPos.GetPixel(x, y) != Color.black)
                {
                    //find nearest water
                    float minDis = 1000;
                    Vector2 minPos = Vector2.zero;
                    for (int x1 = Mathf.Max(16, x - 5); x1 < Mathf.Min(234, x + 5); x1++)
                    {
                        for (int y1 = Mathf.Max(016, y - 5); y1 < Mathf.Min(234, y + 5); y1++)
                        {

                            if (avs.GetPixel(x1, y1) != Color.black && Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1)) < minDis)
                            {
                                minDis = Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1));
                                minPos = new Vector2(x1, y1);
                            }

                        }
                    }
                   
                    if (minPos != Vector2.zero)
                    {
                        RaycastHit hit;

                        if (Physics.Raycast(new Vector3(x, 200, y), Vector3.down, out hit, Mathf.Infinity,LayerMask.GetMask("islandBaseMesh")))
                        {
                            floraPos.SetPixel((int)minPos.x, (int)minPos.y, Color.blue);
                            GameObject marker = Instantiate(markerPrefab);
                            marker.transform.position = new Vector3(minPos.x,hit.point.y,minPos.y);
                            marker.transform.parent = markerHolder;
                            Collider[] precise=Physics.OverlapSphere(hit.point,5f, LayerMask.GetMask("Flora"), QueryTriggerInteraction.Collide);
                            Vector3 tempPos=precise[0].transform.position;
                            marker.GetComponent<markerController>().nearWater = new Vector2(tempPos.x, tempPos.z);
                            marker.GetComponent<markerController>().type = "Flora";
                        }
                    }
                }
            }
        }
        floraPos.Apply();

    }
    
    private void OnDrawGizmos()
    {
        for (int y = 0;y < 250; y++)
        {
            for(int x = 0;x < 250; x++)
            {
                Gizmos.color = near2.GetPixel(x, y);
                if (where2.GetPixel(x, y) != Color.black)
                    Gizmos.color = Color.blue;
                Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one     );
            }
        }
    }
}
