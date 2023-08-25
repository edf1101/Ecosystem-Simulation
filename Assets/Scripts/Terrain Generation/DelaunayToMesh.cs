using System.Collections.Generic;
using UnityEngine;


// converts delaunay triangulated points into a unity mesh
public class DelaunayToMesh
{

    // convert to mesh by importing verts and tris
    public Mesh TriangualationToMesh(Triangulation tria)
    {
        Mesh Mmesh = new Mesh(); // create mesh object

        List<Vector3> VertList = new List<Vector3>();
        Vector3[] vertArray = tria.PS;

        Vector3[] Vertices = vertArray;
        Mmesh.vertices = Vertices;

        Mmesh.triangles = tria.tris;
        Mmesh.RecalculateNormals();
        Mmesh.RecalculateTangents();
        Mmesh.RecalculateBounds();
        return Mmesh;
    }

    // add 3d hieght to a 2D plane
    public Mesh ApplyHeight(Mesh mesh, Texture2D HM, int strength,int Offset =0)
    {
        Vector3[] vert = mesh.vertices;
        for (int i = 0;i < mesh.vertexCount; i++)
        {
            float val = HM.GetPixel((int)vert[i].x/10, (int)vert[i].z/10).r *strength;
            vert[i] = new Vector3(vert[i].x,0,vert[i].z) + new Vector3(0, val+Offset, 0); 
            
        }
        Mesh newMesh = new Mesh();
        newMesh.vertices = vert;
        newMesh.triangles=mesh.triangles;
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        return newMesh;

    }

}
