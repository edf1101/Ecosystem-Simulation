using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaScript : MonoBehaviour
{
    [SerializeField]   TerrainGenerator TG;
    [SerializeField] NoiseGen NG;
    [SerializeField] GameObject meshHolder;
    //[SerializeField] Material seaMat;
    // bool done=false;
    //  public bool doo;
   // bool begun;
    public void Begin()
    {
        Mesh delaunay2D = TG.triangulatedMesh;
        Texture2D bumpMap = NG.SinglePerlin(1000, 1000, new Vector2(Random.Range(0, 100000), Random.Range(0, 10000)), 1);
        Mesh delaunay3D = new DelaunayToMesh().ApplyHeight(delaunay2D, bumpMap, 10);
     //   GetComponent<MeshFilter>().mesh = delaunay3D;

        foreach (Transform t in meshHolder.transform)
        {
            t.gameObject.GetComponent<MeshFilter>().mesh = delaunay3D;
        }
      //  begun = true;
    }
   

}
