using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tri
{
    public Dictionary<int,Vector3> vert = new Dictionary<int,Vector3>();
    //public Vector3[] vert=new Vector3[3];
   // public List<Tri> neighbour= new List<Tri>();
    public Dictionary<int,Tri> neighbour = new Dictionary<int,Tri>();
    public Dictionary<Tri,int> neighbourkey = new Dictionary<Tri,int>();
    public Vector3 p0;
    public float r2;
    public Tri(Vector3 v1,Vector3 v2,Vector3 v3)
    {
        this.vert[0] = v1;
        this.vert[1] = v2;
        this.vert[2] = v3;
        this.neighbour.Add(0,null);
        this.neighbour.Add(1,null);
        this.neighbour.Add(2,null);


        Vector3 a = this.vert[0] - this.vert[2];
        Vector3 b = this.vert[1] - this.vert[2];

        Vector3 z = Vector3.Cross(a, b);

         p0 = Vector3.Cross(Vector3.Dot(a, a) * b - Vector3.Dot(b, b) * a, z) * (0.5f / (Vector3.Dot(z, z) + 0.0000001f)) + this.vert[2];

         r2 = 0.25f * Vector3.Dot(a, a) * Vector3.Dot(b, b) * Vector3.Dot(a - b, a - b) / (Vector3.Dot(z, z) + 0.0000001f);

    }

    public void SetEdge(Vector3[] edge, Tri T)
    {
        Vector3[] tempVert=new Vector3[4];
        tempVert[0] = this.vert[0];
        tempVert[1] = this.vert[1];
        tempVert[2] = this.vert[2];
        tempVert[3] = this.vert[0];

        for (int i = 0; i < 4; i++)
        {
            if (edge[0]==tempVert[i] && edge[1] == tempVert[i + 1])
            {
                this.neighbour[(i + 2) % 3] = T;
                this.neighbourkey[T] = (i + 2) % 3;
                return;
            }
        }
    }
}

public class Point
{
    public float x = new float();
    public float y = new float();
    public float z = new float();  
    public Point(float xIN,float yIN)
    {
        this.x = xIN;
        this.y = yIN;
        this.z = 0;
    }
}

public class Bound
{
    public Vector3 v1;
    public Vector3 v2;
   public Tri tri;
}
public class TriHolder
{
    public int v1;
    public int v2;
    public int v3;
}
public class Triangulation
{
    public Vector3[] PS;
    public int[] tris;
}
public class Delaunay 
{
    List<Tri> triangles=new List<Tri>();

   public Delaunay()
    {
        Vector3 A = new Vector3(0,0,0);
        Vector3 B = new Vector3(10000, 0, 0);
        Vector3 C = new Vector3(10000, 10000, 0);
        Vector3 D = new Vector3(0, 10000, 0);
        Tri T1 = new Tri(A, D, B);
        Tri T2= new Tri(C, B, D);
        T1.neighbour[0] = T2;
        T1.neighbourkey[T2] = 0;
        T2.neighbour[0] = T1;
        T2.neighbourkey[T1] = 0;
        triangles.Add(T1);
        triangles.Add(T2);
    }


     bool IsInCircumcircleOf(Vector3 point, Tri T)
    {
       
        return Vector3.Dot(point - T.p0, point - T.p0) <= T.r2;
    }
   
    Bound[] Boundary(List<Tri> bads)
    {
        Tri T = bads[0];
        int edge = 0;
      //  List<Bound> bounds =new List<Bound>();
        Dictionary<int,Bound> bounds = new Dictionary<int,Bound>();
        int attempts = 0;
        while (true){
            attempts++;
           
                //   Debug.Log("!");
                if (bounds.Count > 1)
            {
                if (bounds[0].v1 == bounds[bounds.Count-1].v1 && bounds[0].v2 == bounds[bounds.Count - 1].v2 && bounds[0].tri == bounds[bounds.Count - 1].tri)
                {
             //       Debug.Log("done");
                    break;
                }
            }
           // Debug.Log("P");
            //  Debug.Log(attempts);
         //   Debug.Log(bounds.ToString());
          //  Debug.Log("BL");
          //  Debug.Log(bounds.Count);
            if (bads.Contains(T.neighbour[edge]))
            {
               Tri last = T;
                T=T.neighbour[edge];
               // T.neighbour.val
                edge = (T.neighbourkey[last] + 1) % 3;
              //  Debug.Log(edge);
              //  break;
            }
            else
            {//
              //  Debug.Log("FOUND");
                Bound tempv=new Bound();
               tempv.v1=(T.vert[(edge+1) % 3]);
                tempv.v2=(T.vert[(edge + 2) % 3]);
                tempv.tri=(T.neighbour[(edge)]);
                bounds.Add(bounds.Count,tempv);
                edge=(edge+1)%3;
                

              
            }
        }
        int temp1 = bounds.Count - 1;
        Bound[] boundsBarOne = new Bound[temp1];
        for (int i = 0; i < temp1; i++)
        {
           // boundsBarOne.Add(bounds[i]);
            boundsBarOne[i] = bounds[i];
        }
      //  Debug.Log("ACCDONE");
        return boundsBarOne;

    }

    void AddPoint(Vector3 p)
    {
        List<Tri> bads = new List<Tri>();
    //    Debug.Log(triangles.Count);
        for (int i = 0; i < triangles.Count; i++)
        {
        //    Debug.Log(IsInCircumcircleOf(p, triangles[i]));
            if (IsInCircumcircleOf(p,triangles[i]) ==true)
            {
             bads.Add(triangles[i]);   
            }
        }
      //  Debug.Log("");
        Bound[] bounds = Boundary(bads);

   

        for (int i = 0;i < bads.Count; i++)
        {
            triangles.Remove(bads[i]);
        }

        List<Tri> newtris= new List<Tri>();
        for (int i = 0; i < bounds.Length; i++)
        {
            Bound edge=bounds[i];
            Tri tt = new Tri(p, bounds[i].v1, bounds[i].v2);
            tt.neighbour[0] = bounds[i].tri;
            if (bounds[i].tri!=null)
            tt.neighbourkey[bounds[i].tri] = 0;

            if (tt.neighbour[0] != null)
            {

                tt.neighbour[0].SetEdge(new Vector3[] {edge.v2,edge.v1 }, tt);
            }

           newtris.Add(tt); 
        }
        
        int N = newtris.Count;
    //    Debug.Log(N);
        for (int i = 0; i < N; i++)
        {
            Tri T = newtris[i];
       //     Debug.Log((N+i-1)%N);
            T.neighbour[2]=newtris[(N+i-1)%N];
            T.neighbourkey[newtris[(N + i - 1) % N]] = 2;
            T.neighbour[1]=newtris[(N+i+1)%N];
            T.neighbourkey[newtris[(N + i + 1) % N]] = 1;
        }
      //  Debug.Log("pt2");

        for (int i =0; i<newtris.Count; i++)
        {
            triangles.Add(newtris[i]);
        }
        
    }

    public Triangulation Triangulate(List<int> xs, List<int> ys)
    {
        for (int i = 0;i < xs.Count; i++)
        {
            AddPoint(new Vector3(xs[i], ys[i], 0));
        }

        Vector3[] ps=new Vector3[triangles.Count*3];
        int[] tris=new int[triangles.Count*3];

      //  Color[] colors =tex.GetPixels();
     for (int i = 0; (i < triangles.Count); i++)
        {
            tris[(i * 3)] = (i * 3);
            tris[1+(i * 3)] = 1+(i * 3);
            tris[2+(i * 3)] = 2+(i * 3);
            Vector3 temp= triangles[i].vert[0];
           // int strength = 1700;
            ps[(i * 3)] = new Vector3(temp.x, 0, temp.y);

            temp = triangles[i].vert[1];
            ps[1 + (i * 3)] = new Vector3(temp.x, 0, temp.y);

            temp = triangles[i].vert[2];
            ps[2 + (i * 3)] = new Vector3(temp.x, 0, temp.y);
            
        }
    

        
        Triangulation tempTriangulation=new Triangulation();
        tempTriangulation.tris = tris;
        tempTriangulation.PS = ps;
        //Debug.Log("finsied");
        return tempTriangulation;
    }




}
