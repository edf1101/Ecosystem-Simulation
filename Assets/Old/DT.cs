using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT : MonoBehaviour
{
    Delaunay DL=new Delaunay();
    List<int> XS = new List<int>();
    List<int> YS = new List<int>();
    Triangulation TL;
    public Texture2D tex;
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<NoiseGen>().Start();
         tex = GetComponent<NoiseGen>().combo;
       // GetComponent<Renderer>().material.mainTexture = tex;
        float time=Time.time;
       Vector2[] points= new PoissonSampling().GetPoissonPoints(1000, 80, 10000).ToArray();
     // Vector2[] points= new PoissonSampling().GetPoissonPoints(10000, 25, 10000).ToArray();
        Debug.Log("Poisson time");
        Debug.Log(Time.time-time);
     
        foreach(Vector2 p in points)
        {
            XS.Add((int)p.x);
            YS.Add((int)p.y);
        }
           

    
        TL=DL.Triangulate(XS,YS);
        Debug.Log("Triangulaion time");
        Debug.Log(Time.time - time);
        Mesh Mmesh = new DelaunayToMesh().TriangualationToMesh(TL);
        Debug.Log("tri to mesh time");
        Debug.Log(Time.time - time);


        Mesh Tmesh = Mmesh;
        Color[] cols = new Color[Tmesh.vertexCount];
        for (int i = 0; i < cols.Length / 3; i++)
        {
            Color c = Random.ColorHSV();
            cols[(i) * 3] = c;
            cols[(i * 3) + 1] = c;
            cols[(i * 3) + 2] = c;
        }
        Debug.Log("col time");
        Debug.Log(Time.time - time);
        Tmesh.colors = cols;
        GetComponent<MeshFilter>().mesh = Tmesh;
        GetComponent<MeshCollider>().sharedMesh = Tmesh;

    }

  
}
