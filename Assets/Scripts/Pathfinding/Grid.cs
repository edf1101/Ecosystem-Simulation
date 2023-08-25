using UnityEngine;

public class Grid : MonoBehaviour
{
    //External script references
    [SerializeField] private TerrainGenerator TG;
    [SerializeField] private PlacementManager availiblesPM;
    [SerializeField] private waterPlaces WP;

    // Node/grid settings
    public Node[,] grid;
    public Node[,] waterGrid;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;


    //other
    public Texture2D weights;
    [SerializeField] private AnimationCurve curve;
    public Texture2D canWalk;


    // start creating the grid
    public void gridInit()
    {
        
        createWeightMap();
        // node /grid setup
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(  gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        createGrid();

    }

    // create a map of weights for the world based on how steep the terrain is
    private void createWeightMap()
    {
        weights = new Texture2D(250, 250);
        Mesh mesh = TG.mesh;
        Vector3[] verts = mesh.vertices;

        // go through each m^2 in the scnene
        for (int y = 0; y < 250; y++)
        {
            for (int x = 0; x < 250; x++)
            {
                RaycastHit hit;

                if (Physics.Raycast(new Vector3(x, 200, y), Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("islandBaseMesh")))
                {
                    // get its triangle
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

                    // calculate max Gradient based on vertices
                    float gradient = curve.Evaluate(Mathf.Abs((maxY - minY) / Mathf.Sqrt(Mathf.Pow(maxX - minX, 2) + Mathf.Pow(maxZ - minZ, 2))));
                    // set weight to be the gradient
                    weights.SetPixel(x, y, new Color(gradient, gradient, gradient));
                }
            }
        }
        weights.Apply();// apply the texture
    }

    // create the grid itself
    private void createGrid()
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

    // Get the node from a vector3 position
    public Node nodeFromWorld(Vector3 worldPos)
    { 
        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.z);
        return grid[x, y];
    }

    // get the nearest node to a position
    public Node nearestNode(Node currentPos)
    {

        // look around my node in a radius of 2
        for(int x = Mathf.Max(0, currentPos.gridX-2); x < Mathf.Min(250, currentPos.gridX+2);x++)
        {

            for (int y = Mathf.Max(0, currentPos.gridY-2); y < Mathf.Min(250, currentPos.gridY+2); y++)
            {
                if(  grid[x,y].walkable) // if walkable then return in
                {
                    return grid[x,y];
                }


            }

        }
        return null;
    }

    // get neighbours to a node 
    public Node[] getNeighbours(Node node)
    {
        // use arrays code looks messier but its faster
        int arrSize = 8;
        if ((node.gridX == 249 || node.gridX == 0) && !(node.gridY == 249 || node.gridY == 0))
            arrSize = 5;
        if (!(node.gridX == 249 || node.gridX == 0) && (node.gridY == 249 || node.gridY == 0))
            arrSize = 5;
        if ((node.gridX == 249 || node.gridX == 0) && (node.gridY == 249 || node.gridY == 0))
            arrSize = 3;
        Node[] neighbours = new Node[arrSize];

        // fill in the array with neighbours either side
        int count = 0;
        for(int x=-1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 & y == 0) // this is my own position
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
     
        return neighbours; // return the array

    }
    

    public int MaxSize // max size of grid
    {
        get { return gridSizeX*gridSizeY; }
    }

  
}
