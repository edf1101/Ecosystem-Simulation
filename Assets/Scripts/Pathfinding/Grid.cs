using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{


    public Texture2D weights;
    [SerializeField] TerrainGenerator TG;
    [SerializeField] AnimationCurve curve;
    Texture2D waterGridTex;

 public   Node[,] grid;
    public Node[,] waterGrid;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    //  public LayerMask unwalkableMask;
    [SerializeField] bool dispGizmos;
    float nodeDiameter;
    int gridSizeX,gridSizeY;
    [SerializeField] PlacementManager availiblesPM;
    public Texture2D canWalk;
    [SerializeField] waterPlaces WP;
    public void gridInit()
    {
        
        createWeightMap();
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(  gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        createGrid();

    }

    void createWaterWalk()
    {
        waterGridTex = new Texture2D(250, 250);

    }
    void createWeightMap()
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
    void createGrid()
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
//bool Watwalkable=availibles.GetPixel(x,y)!=Color.black || WP.nearWater.GetPixel(x, y) != Color.black;
                //bool walkabke=availibles.GetPixel(x,)
                grid[x, y] = new Node(walkable, WorldPos,x,y,penalty);
                waterGrid[x, y] = new Node(walkable, WorldPos,x,y,penalty);
                //waterGrid[x]
                canWalk.SetPixel(x,y,grid[x,y].walkable?Color.yellow:Color.black);
            }
        }
        canWalk.Apply();
    }
    public Node nodeFromWorld(Vector3 worldPos)
    {
        
        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.z);
        return grid[x, y];
    }

    

    public List<Node> getNeighbours(Node node)
    {
        List<Node> neighbours = new List< Node>();
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
                    neighbours.Add(grid[checkX,checkY]);
                }
            }
        }
        return neighbours;
    }
    public List<Node> getNeighboursWater(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 & y == 0)
                    continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(waterGrid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public int MaxSize
    {
        get { return gridSizeX*gridSizeY; }
    }

  
}
