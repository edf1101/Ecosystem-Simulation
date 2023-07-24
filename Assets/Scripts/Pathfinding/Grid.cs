using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is used to get the grid at the start of what is walkable etc and create a node array from it.
// inspired from Seb Lague A* tutorial although very heavily modified for weighting and particular focus on 3D environments
// implementation into my environment also unique

public class Grid : MonoBehaviour
{


    public Texture2D weights;
    [SerializeField] TerrainGenerator TG;
    [SerializeField] AnimationCurve curve;
    Texture2D waterGridTex;

 public   Node[,] grid;
    public Node[,] waterGrid;
    public Vector2 gridWorldSize;
    [SerializeField] bool dispGizmos;
    float nodeDiameter;
    int gridSizeX = 250;
        int gridSizeY=250;
    [SerializeField] PlacementManager availiblesPM;
    public Texture2D canWalk;
    [SerializeField] waterPlaces WP;
    public void gridInit() // gets called in load manager
    {
        createWeightMap();
        createGrid();

    }

   
    void createWeightMap() // creates the weight map for nodes by looking at gradient of each point
    {
        weights = new Texture2D(250, 250);
        Mesh mesh = TG.mesh;
        Vector3[] verts = mesh.vertices;
        for (int y = 0; y < 250; y++)
        {
            for (int x = 0; x < 250; x++)
            {
                RaycastHit hit;

                if (Physics.Raycast(new Vector3(x, 200, y), Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("islandBaseMesh")))
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
                    float gradient = curve.Evaluate(Mathf.Abs((maxY - minY) / Mathf.Sqrt(Mathf.Pow(maxX - minX, 2) + Mathf.Pow(maxZ - minZ, 2))));

                    weights.SetPixel(x, y, new Color(gradient, gradient, gradient));
                }
            }
        }
        weights.Apply();
    }
    void createGrid() // create the grid of nodes based on what we learnt in weights and Placement manager availible maps
    {
        canWalk = new Texture2D(250, 250);
        Texture2D availibles = availiblesPM.movAv2;
        grid = new Node[gridSizeX, gridSizeY];
        waterGrid = new Node[gridSizeX, gridSizeY];
        for (int x=0; x<gridSizeX; x++)
        {
            for (int y=0; y<gridSizeY; y++)
            {
                // value for modifyig weighting
                int penalty = (int)(40* weights.GetPixel(x,y).r);
                Vector3 WorldPos=new Vector3(x,0,y);    
                bool walkable=availibles.GetPixel(x,y)!=Color.black;
                grid[x, y] = new Node(walkable, WorldPos,x,y,penalty);
                waterGrid[x, y] = new Node(walkable, WorldPos,x,y,penalty);
                canWalk.SetPixel(x,y,grid[x,y].walkable?Color.yellow:Color.black);
            }
        }
        canWalk.Apply();
    }
    public Node nodeFromWorld(Vector3 worldPos) // get a node from a point in the world
    {
        
        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.z);
        return grid[x, y];
    }


    public Node[] getNeighbours(Node node) // get the neighbours of a node
    {
        int arrSize = 8;
        if ((node.gridX == 249 || node.gridX == 0) && !(node.gridY == 249 || node.gridY == 0))
            arrSize = 5;
        if (!(node.gridX == 249 || node.gridX == 0) && (node.gridY == 249 || node.gridY == 0))
            arrSize = 5;
        if ((node.gridX == 249 || node.gridX == 0) && (node.gridY == 249 || node.gridY == 0))
            arrSize = 3;
        Node[] neighbours = new Node[arrSize];
        int count = 0;
        for(int x=-1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 & y == 0)
                    continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if(checkX>=0 && checkX<gridSizeX && checkY>=0 && checkY < gridSizeY)
                {
                    neighbours[count]=(grid[checkX,checkY]);
                    count++;
                }
            }
        }
     
        return neighbours;
    }
    

    

  
}
