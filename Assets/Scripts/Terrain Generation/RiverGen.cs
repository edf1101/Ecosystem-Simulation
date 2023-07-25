using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGen : MonoBehaviour
{
    Texture2D river;
    Dictionary<int, riverData> rivers=new Dictionary<int,riverData>();
    [SerializeField] GameObject test;
    [SerializeField] GameObject riverHolder;
    [SerializeField] Material waterMat;
    Texture2D comboMap;
   public Texture2D riv1;
    [SerializeField] float lakeThresh;
    public class riverData
    {
       
      
       public Dictionary<int, Vector2> know = new Dictionary<int, Vector2>();
      public  Dictionary<Vector2, int> knowkeys = new Dictionary<Vector2, int>();
    }

    public void init(Texture2D tex)
    {
        river = tex;
        riv1 = new Texture2D(1000,1000);
        riv1.SetPixels(river.GetPixels());
        riv1.Apply();
        rivers.Clear();
        NoiseGen NG=GetComponent<NoiseGen>();
        comboMap = NG.combo;
        for (int x = 0; x < 1000; x += 2)
        {
            for (int y = 0; y < 1000; y += 2)
            {
                if(riv1.GetPixel(x, y).g == 1)
                {
                   riverData riverpoints= FloodFill(new Vector2(x, y));
                    if (riverpoints.know.Count > 200)
                    {
                        rivers.Add(rivers.Count, riverpoints);
                        UpdateImg(riverpoints.know);
                    }
                }

            }
        }
        int childs =riverHolder .transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(riverHolder.transform.GetChild(i).gameObject);
        }

    }
 
    public void createRivers()
    {
        for (int x = 0;x<rivers.Count;x++)
        createRiver(pointsForRiver(x),x);
        
        
    }


    public void createRiver(Dictionary<Vector2,int> rivKeys,int whichRiv)
    {
        GameObject myHolder = new GameObject("River: " + whichRiv.ToString());
        myHolder.layer = 4;
        myHolder.transform.parent = riverHolder.transform;
        myHolder.AddComponent<MeshFilter>();
        myHolder.AddComponent<MeshRenderer>();
        myHolder.AddComponent<MeshCollider>();
        myHolder.transform.localScale = Vector3.one;
        myHolder.GetComponent<Renderer>().material = waterMat;
        TerrainGenerator TG=GetComponent<TerrainGenerator>();
        Mesh mesh = TG.mesh;
        Dictionary<int, Vector3> newVerts= new Dictionary<int, Vector3>();
        Vector3[] vertsOld= mesh.vertices;
        for (int x = 0;x < vertsOld.Length; x++)
        {
            
            if (rivKeys.ContainsKey(new Vector2((float)(int)vertsOld[x].x, (float)(int)vertsOld[x].z)))
            {
                newVerts.Add(x, new Vector3(vertsOld[x].x, 0, vertsOld[x].z));
            }
        }
        int[] tris=mesh.triangles;
      //  Debug.Log(newVerts.Count);
        List<int> Ntris = new List<int>();
        for (int x = 0; x < tris.Length/3; x++)
        {
            if(rivKeys.ContainsKey(  new Vector2((float)(int)vertsOld[tris[(x*3)]].x, (float)(int)vertsOld[tris[(x * 3)]].z) ) || rivKeys.ContainsKey(new Vector2((float)(int)vertsOld[tris[1+(x * 3)]].x, (float)(int)vertsOld[tris[1+(x * 3)]].z)) || rivKeys.ContainsKey(new Vector2((float)(int)vertsOld[tris[2+(x * 3)]].x, (float)(int)vertsOld[tris[2+(x * 3)]].z)))
            {
                Ntris.Add(tris[(0 + (x * 3))]);
                Ntris.Add(tris[(1 + (x * 3))]);
                Ntris.Add(tris[(2 + (x * 3))]);
            }
        }

        //   Debug.Log(Ntris.Count);
         List<Vector3> NNverts = new List<Vector3>();
        List<int> NNtris=new List<int>();

        for (int i=0; i < Ntris.Count; i++)
        {
            NNverts.Add(vertsOld[Ntris[i]]);
            NNtris.Add(i);
        }
        Mesh nMesh=new Mesh();
        nMesh.vertices = NNverts.ToArray();
        nMesh.triangles = NNtris.ToArray();
        nMesh.RecalculateNormals();
        nMesh.RecalculateBounds();
        
        Vector3[] vert = nMesh.vertices;
        float min = 10000;
        for (int i = 0; i < nMesh.vertexCount; i++)
        {
            float val = (GetComponent<NoiseGen>().perl.GetPixel((int)vert[i].x / 10, (int)vert[i].z / 10).r * TG.strength);
            if (val < min)
                min = val;

        }
        Texture2D bumpMap = GetComponent<NoiseGen>().SinglePerlin(1000, 1000, new Vector2(Random.Range(0, 100000), Random.Range(0, 10000)), 1);
        for (int i = 0; i < nMesh.vertexCount; i++)
        {
            float val = min;
            vert[i] = new Vector3(vert[i].x, 0, vert[i].z) + new Vector3(0, val-10-bumpMap.GetPixel((int)vert[i].x / 10, (int)vert[i].z / 10).r *10 , 0);

        }
        Mesh newMesh = new Mesh();
        newMesh.vertices = vert;
        newMesh.triangles = nMesh.triangles;
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        nMesh= newMesh;
        myHolder.GetComponent<MeshFilter>().mesh = nMesh;
        myHolder.GetComponent<MeshCollider>().sharedMesh = nMesh;

    }

   public Dictionary<Vector2,int> pointsForRiver(int riverIndex)
    {
        TerrainGenerator TG=GetComponent<TerrainGenerator>();
        Vector2[] wholePoints= TG.points;
        //Debug.Log(wholePoints.Length);
        Dictionary<int, Vector2> rivPoints = new Dictionary<int, Vector2>();
        Dictionary<Vector2,int> rivKeys=new Dictionary<Vector2,int>();
        for (int i = 0; i < wholePoints.Length; i++)
        {
          //  Debug.Log(wholePoints[i].x / 10);
            if (rivers[riverIndex].knowkeys.ContainsKey(new Vector2((int)wholePoints[i].x/10,(int)wholePoints[i].y/10)))
            {
                rivPoints.Add(rivPoints.Count, wholePoints[i]);
                rivKeys.Add(new Vector2((float)(int)wholePoints[i].x,(float)(int)wholePoints[i].y),rivKeys.Count);
            }
        }
        for (int i = 0;i < rivers[riverIndex].know.Count; i++)
        {
            riv1.SetPixel((int)rivers[riverIndex].know[i].x, (int)rivers[riverIndex].know[i].y,Color.blue);
        }
        riv1.Apply();
        //   Debug.Log(rivPoints.Count);
        //Debug.Log( (rivPoints[0]).x);
        return rivKeys;
    }

    void UpdateImg(Dictionary<int, Vector2> poin)
    {

        for (int i=0; i < poin.Count; i++)
        {
            riv1.SetPixel((int)poin[i].x,(int) poin[i].y,Color.red);
        }
        riv1.Apply();
       
    }


    riverData FloodFill(Vector2 start)
    {
        Queue<Vector2> queue = new Queue<Vector2>();
        queue.Enqueue(start);
     //   List<Vector2> know = new List<Vector2>();
        Dictionary<int,Vector2>know=new Dictionary<int,Vector2>();
        Dictionary<Vector2,int> knowkeys=new Dictionary<Vector2,int>();
        while (queue.Count > 0)
        {

            Vector2 temp = queue.Peek()+new Vector2(-1,0);
            if (temp.x > 0 && !queue.Contains(temp)==true &&  river.GetPixel((int)temp.x, (int)temp.y).r == 1 &&knowkeys.ContainsKey(temp)==false )
                queue.Enqueue(temp);

             temp = queue.Peek() + new Vector2(1, 0);
            if (temp.x < 1000 && !queue.Contains(temp) == true && river.GetPixel((int)temp.x, (int)temp.y).r == 1 && knowkeys.ContainsKey(temp) == false)
                queue.Enqueue(temp);

            temp = queue.Peek() + new Vector2(0, 1);
            if (temp.y < 1000 && !queue.Contains(temp) == true && river.GetPixel((int)temp.x, (int)temp.y).r == 1 && knowkeys.ContainsKey(temp) == false)
                queue.Enqueue(temp);

            temp = queue.Peek() + new Vector2(0, -1);
            if (temp.y>0&&!queue.Contains(temp) == true && river.GetPixel((int)temp.x, (int)temp.y).r == 1 && knowkeys.ContainsKey(temp) == false)
                queue.Enqueue(temp);

            know.Add(know.Count,queue.Peek());
            knowkeys.Add(queue.Peek(), know.Count);
            queue.Dequeue();
        }
        riverData RD=new riverData();
        RD.know=know;
        RD.knowkeys = knowkeys;
        return RD;
    }
    
}
