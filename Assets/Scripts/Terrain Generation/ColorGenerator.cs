using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator : MonoBehaviour
{
    public float slightVal;
    public Texture2D test;
    public float steepVal;
    public float waterThresh;
    public float snowPeakThresh;
   public void assignColors()
    {
        test=new Texture2D(50,50);    
        TerrainGenerator TG=GetComponent<TerrainGenerator>();
        biomeGenerator BG=GetComponent<biomeGenerator>();
        NoiseGen NG=GetComponent<NoiseGen>();
        Texture2D perlMap = NG.combo;
        Texture2D rivMap = NG.rivers;
        Mesh Tmesh = TG.mesh;
        Color[] cols = new Color[Tmesh.vertexCount];
        Texture2D baseMap = BG.baseColorMap;
        Texture2D slightMap = BG.slightColorMap;
        Texture2D steepMap = BG.steepColorMap;
        Texture2D rivColMap = BG.riverColorMap;
      //  int a = 1;
        Vector3[] verts=Tmesh.vertices;
        Dictionary<Vector2, biomeGenerator.Biome> biomeMap = BG.biomeDict;
        for (int i = 0; i < Tmesh.vertexCount / 3; i++)
        {
            Vector3 v1 = verts[i * 3];
            Vector3 v2 = verts[(i * 3) + 1];
            Vector3 v3 = verts[(i * 3) + 2];
            float minY = Mathf.Min(Mathf.Min(v1.y, v2.y), v3.y);
            float maxY = Mathf.Max(Mathf.Max(v1.y, v2.y), v3.y);
            float minZ= Mathf.Min(Mathf.Min(v1.z, v2.z), v3.z);
            float maxZ = Mathf.Max(Mathf.Max(v1.z, v2.z),v3.z);
            float minX = Mathf.Min(Mathf.Min(v1.x, v2.x), v3.x);
            float maxX = Mathf.Max(Mathf.Max(v1.x, v2.x), v3.x);
            float gradient = (maxY - minY) / Mathf.Sqrt(Mathf.Pow(maxX - minX,2)+Mathf.Pow(maxZ-minZ,2));
            Vector3 avgPos = new Vector3((v1.x + v2.x + v3.x) / 3, (v1.y + v2.y + v3.y) / 3, (v1.z + v2.z + v3.z) / 3);
            Color c = baseMap.GetPixel((int)avgPos.x / 20, (int)avgPos.z / 20);
            if(gradient>slightVal)
                c= slightMap.GetPixel((int)avgPos.x / 20, (int)avgPos.z / 20);
            if (gradient>steepVal)
                c= steepMap.GetPixel((int)avgPos.x / 20, (int)avgPos.z / 20);
            if(rivMap.GetPixel((int)avgPos.x / 10, (int)avgPos.z / 10).r>0.2f|| perlMap.GetPixel((int)avgPos.x / 10, (int)avgPos.z / 10).r < waterThresh)
                c = rivColMap.GetPixel((int)avgPos.x / 10, (int)avgPos.z / 10);
            if (avgPos.y > snowPeakThresh && (biomeMap[new Vector2((int)avgPos.x / 20, (int)avgPos.z / 20)].name == "Grass" || biomeMap[new Vector2((int)avgPos.x / 20, (int)avgPos.z / 20)].name == "Swamp"))
                c = Color.white;


            test.SetPixel((int)avgPos.x/200,(int)avgPos.z/200,c);
            cols[(i) * 3] = c;
            cols[(i * 3) + 1] = c;
            cols[(i * 3) + 2] = c;
        }
        test.Apply();
        Texture2D Btest = new LinearBlur().Blur(test, 1, 1);

       for (int i = 0; i < 50; i++)
        {
            Btest = avgTex(test, Btest);
        }
        test =new LinearBlur().Blur( Btest,1,1);
        test.Apply();
        




        for (int i = 0; i < Tmesh.vertexCount / 3; i++)
        {
            Vector3 v1 = verts[i * 3];
            Vector3 v2 = verts[(i * 3) + 1];
            Vector3 v3 = verts[(i * 3) + 2];
            Vector3 avgPos = new Vector3((v1.x + v2.x + v3.x) / 3, (v1.y + v2.y + v3.y) / 3, (v1.z + v2.z + v3.z) / 3);
            Color c = test.GetPixel((int)avgPos.x / 200, (int)avgPos.z / 200);
            cols[(i) * 3] = c;
            cols[(i * 3) + 1] = c;
            cols[(i * 3) + 2] = c;
        }
            Tmesh.colors = cols;
        GetComponent<MeshFilter>().mesh = Tmesh;
    }
    Texture2D avgTex(Texture2D tex1, Texture2D tex2)
    {
        Color[] col1 = tex1.GetPixels();
        Color[] col2 = tex2.GetPixels();
        for (int i = 0; i < col1.Length; i++)
        {
            col1[i] = (col1[i] + col2[i]) / 2;
        }
         Texture2D tex3 = new Texture2D(tex1.width, tex1.height);
        tex3.SetPixels(col1);
        tex3.Apply();
        return tex3;
    }
}
