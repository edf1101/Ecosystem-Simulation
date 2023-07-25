using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture2D finalPerlin;
    NoiseGen NG;
   public Vector2[] points;
    public Mesh mesh;
    public int strength;
    public Mesh triangulatedMesh;
    public Triangulation TL;
    public SeaScript SG;
  //  bool fromScratch = false;

   

    public void generateTerrain()
    {
        GenerateNoise();
        GenExpensivePoints();
        TriangulateMesh();
        ApplyHeight();
        createRivers();
        createBiomes();
        assignCols();
        objTest();
        addTrees();
        addFlowers();
    }
    public void partialGenerateTerrain()
    {
        GenerateNoise();
       // GenExpensivePoints();
        //TriangulateMesh();
        ApplyHeight();
        createRivers();
        createBiomes();
        assignCols();
        objTest();
        addTrees();
        addFlowers();
    }
    public void addTrees()
    {
        GetComponent<PlacementManager>().addTrees();
    }
    public void addFlowers()
    {
        GetComponent<PlacementManager>().addFlowers();
    }


    public void objTest()
    {
        GetComponent<PlacementManager>().Begin();
    }
    public void seaGen()
    {
        SG.Begin();
    }
    public void assignCols()
    {
        ColorGenerator CG=GetComponent<ColorGenerator>();
        CG.assignColors();
    }
    public void createBiomes()
    {
        biomeGenerator BG = GetComponent<biomeGenerator>();
        BG.generateBiomes();
    }
    public void createRivers()
    {
        RiverGen RG=GetComponent<RiverGen>();
        RG.init(NG.riv2);
        RG.createRivers();
    }
    public void GenerateNoise()
    {
        if (NG == null)
        {
            NG = GetComponent<NoiseGen>();
        }
        NG.createMap();
        finalPerlin = NG.combo;
        //UpdateTex();
    }
    public void GenCheapPoints()
    {
         points = new PoissonSampling().GetPoissonPoints(1000, 80, 10000).ToArray();
    }
    public void GenExpensivePoints()
    {
         points = new PoissonSampling().GetPoissonPoints(10000, 25, 10000).ToArray();
    }
     void UpdateMesh()
    {
      GetComponent<MeshFilter>().mesh = mesh;
      GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    public void TriangulateMesh()
    {
        List<int> XS=new List<int>();
        List<int> YS=new List<int>();
        foreach (Vector2 p in points)
        {
            XS.Add((int)p.x);
            YS.Add((int)p.y);
        }
         TL = new Delaunay().Triangulate(XS, YS);
        mesh= new DelaunayToMesh().TriangualationToMesh(TL);
        triangulatedMesh = mesh;
        UpdateMesh();
    }
    public void ApplyHeight()
    {
        mesh = new DelaunayToMesh().ApplyHeight(triangulatedMesh, finalPerlin,  strength);
        UpdateMesh();
    }
   

}
