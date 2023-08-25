using System.Collections.Generic;
using UnityEngine;


// this script calls all the functions to build terrain its also got lots
// of functions for debugging that can be run in editor
public class TerrainGenerator : MonoBehaviour
{

    public Texture2D finalPerlin;
    private NoiseGen NG;
   public Vector2[] points;
    public Mesh mesh;
    public int strength;
    public Mesh triangulatedMesh;
    public Triangulation TL;
    public SeaScript SG;


   
    // generate terrain from scratch
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

    // generate terrain using premade delaunay 2d plane
    public void partialGenerateTerrain()
    {
        GenerateNoise();
        ApplyHeight();
        createRivers();
        createBiomes();
        assignCols();
        objTest();
        addTrees();
        addFlowers();
    }


    // functions for various stages of terrain generation
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
