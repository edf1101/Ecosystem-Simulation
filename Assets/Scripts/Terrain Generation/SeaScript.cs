using UnityEngine;

// generates a sea plane using premade delaunay plane
public class SeaScript : MonoBehaviour
{
    [SerializeField]   TerrainGenerator TG;
    [SerializeField] NoiseGen NG;
    [SerializeField] GameObject meshHolder;
 
    public void Begin()
    {
        Mesh delaunay2D = TG.triangulatedMesh;

        //add a bit of bumps to it
        Texture2D bumpMap = NG.SinglePerlin(1000, 1000, new Vector2(Random.Range(0, 100000), Random.Range(0, 10000)), 1);
        Mesh delaunay3D = new DelaunayToMesh().ApplyHeight(delaunay2D, bumpMap, 10);
        

        foreach (Transform t in meshHolder.transform)
        {
            t.gameObject.GetComponent<MeshFilter>().mesh = delaunay3D;
        }
 
    }
   

}
