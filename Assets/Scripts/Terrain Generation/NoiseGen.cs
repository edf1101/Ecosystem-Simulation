using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoiseGen : MonoBehaviour
{
    
    public Texture2D perl;
    public Texture2D rivers;
  
    public AnimationCurve RiverCurve;
    public AnimationCurve TerrainCurve;
    public AnimationCurve riverHeightCurve;
    public AnimationCurve riverEffectCurve;
    public Texture2D combo ;
    public Texture2D riv2;
    public MapRating MR;
    public Texture2D riv3;
   // public Texture2D fallOff;



   [System.Serializable]
    public class MapRating
    {
        public float waterPer;
        public float highPer;
        public float maxed;
    }
   
    public void createMap()
    {
        rivers = new Texture2D(1000, 1000);
        riv2= new Texture2D(1000, 1000);
        riv3 = new Texture2D(1000, 1000);
        //   fallOff = makeFalloff();
        perl = applyFalloff( curve2Tex(PerlinTex(1000, 2.5f, new Vector2(Random.Range(0, 10000), Random.Range(0, 10000))), TerrainCurve));
        rivers = applyFalloff( curve2Tex(SinglePerlin(1000, 13, new Vector2(Random.Range(0, 10000), Random.Range(0, 10000)), 0.8f), RiverCurve),rivFalloff:true);
         MR = rateMap(perl, 0.07f, 0.5f);
        while ((MR.highPer > 0.015f && MR.waterPer > 0.22 && MR.waterPer < 0.5f&&MR.maxed<30) == false)
        {
            perl =applyFalloff( curve2Tex(PerlinTex(1000, 2.5f, new Vector2(Random.Range(0, 10000), Random.Range(0, 10000))), TerrainCurve));
            MR = rateMap(perl, 0.07f, 0.5f);
        }
        combo = avgTex(perl, rivers);
        riv2 = rivAddition(rivers, perl);
        riv3 = rivAddition2(rivers,perl);
    }
    
    Texture2D makeFalloff() { 
        Vector2 O = new Vector2(500, 500);
        Color[] cols = new Color[1000 * 1000];
        for (int y = 0; y <1000; y++) {
            for (int x = 0; x < 1000; x++)
            {
                float val=0;
                if (x >= 50 && y >= 50 && x <= 950 && y <= 950)
                    val = 1;
                float xdis=0;
                float ydis=0;
                if (x < 50)
                    xdis = x;
                if (y < 50)
                    ydis = y;
                if (x > 950)
                    xdis = 1000 - x;
                if (y >950)
                    ydis = 1000 - y;
                if (ydis == 0&&xdis!=0)
                    val = xdis/50f;
                else if (xdis == 0 && ydis!=0)
                    val = ydis/50f;
                if (xdis != 0 && ydis != 0)
                    val =( xdis / 50f)*(ydis/50f);


                cols[(y*1000)+x] = new Color(val,val,val);



            }
        
        }
        Texture2D tex = new Texture2D(1000, 1000);
        tex.SetPixels(cols);
        tex.Apply();
        return tex;
    }

    Texture2D makeRivFalloff()
    {
        Vector2 O = new Vector2(500, 500);
        Color[] cols = new Color[1000 * 1000];
        for (int y = 0; y < 1000; y++)
        {
            for (int x = 0; x < 1000; x++)
            {
                float val = 0;
                if (x >= 100 && y >= 100 && x <= 900 && y <= 900)
                    val = 1;
                else
                    val = 0;

                cols[(y * 1000) + x] = new Color(val, val, val);



            }

        }
        Texture2D tex = new Texture2D(1000, 1000);
        tex.SetPixels(cols);
        tex.Apply();
        return tex;
    }


    Texture2D rivAddition(Texture2D tex,Texture2D perl)
    {
        Texture2D nTex=new Texture2D(1000, 1000);
        float thresh=0.02f;
        Color[] col2 = perl.GetPixels();
        Color[] cols=tex.GetPixels();
        for (int i=0; i<cols.Length; i++)
        {
            float val= (cols[i].r * 0.12f * riverHeightCurve.Evaluate(col2[i].r) );
            if (val > thresh)
                cols[i] = new Color(1, 1, 1);
            else
                cols[i] = new Color(0, 0, 0);
        }
        nTex.SetPixels(cols);
        nTex.Apply();
        return nTex;
    }
    Texture2D rivAddition2(Texture2D tex, Texture2D perl)
    {
        Texture2D nTex = new Texture2D(1000, 1000);
       // float thresh = 0.02f;
        Color[] col2 = perl.GetPixels();
        Color[] cols = tex.GetPixels();
        for (int i = 0; i < cols.Length; i++)
        {
            float val = (cols[i].r * 0.12f * riverHeightCurve.Evaluate(col2[i].r));

            cols[i] = new Color(val,val,val);
        }
        nTex.SetPixels(cols);
        nTex.Apply();
        return nTex;
    }
    MapRating rateMap(Texture2D tex,float waterHeight, float highHeight)
    {
        int waterCount=0;
        int highCount=0;
        int maxed = 0;
        Color[] cols=tex.GetPixels();
        for (int i = 0; i < cols.Length; i++)
        {
            if(cols[i].r < waterHeight)
                waterCount++;
            if (cols[i].r > highHeight)
                highCount++;
            if (cols[i].r > 0.9f)
                maxed++;

        }
        
        MapRating retMap=new MapRating();
        retMap.waterPer=(float)waterCount/ (float)cols.Length;
        retMap.highPer= (float)highCount / (float)cols.Length;
        retMap.maxed= maxed;
        return retMap;
    }


   public Texture2D SinglePerlin(int size, float scale, Vector2 offset,float amplitude)
    {
        float[] octaveFrequencies = new float[] {scale};
        float[] octaveAmplitudes = new float[] { amplitude };
        Color[] cols = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float val = 0;
                for (int i = 0; i < octaveFrequencies.Length; i++)
                    val += octaveAmplitudes[i] * Mathf.PerlinNoise(offset.x + octaveFrequencies[i] / size * x + .3f, offset.y + octaveFrequencies[i] / size * y + .3f);
                val = Mathf.Clamp(val, 0, 1);
                cols[(y * size) + x] = new Color(val, val, val);
            }
        }
        Texture2D tex = new Texture2D(size, size);
        tex.SetPixels(cols);
        tex.Apply();
        return tex;
    }

    Texture2D PerlinTex(int size,float scale,Vector2 offset)
    {
        float[] octaveFrequencies = new float[] {2f, 5.5f, 11.5f, 23 };
        float[] octaveAmplitudes = new float[] { 0.9f, 0.15f, 0.1f, 0.05f };
        Color[] cols = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0;x < size; x++)
            {
                float val = 0;
                for (int i = 0; i < octaveFrequencies.Length; i++)
                    val += octaveAmplitudes[i] * Mathf.PerlinNoise(offset.x+octaveFrequencies[i]/size * x + .3f,offset.y+ octaveFrequencies[i]/size * y + .3f);
                val = Mathf.Clamp(val, 0, 1);
                cols[(y*size) + x] = new Color(val,val,val);
            }
        }
      

        Texture2D tex = new Texture2D(size, size);
        tex.SetPixels(cols);
        tex.Apply();
        return tex;
    }
    public Texture2D curve2Tex(Texture2D tex,AnimationCurve cur)
    {
        Color[] cols = tex.GetPixels();
        for (int i =0; i<cols.Length; i++)
        {
            float x = cur.Evaluate(cols[i].r);
            cols[i] = new Color(x, x, x);
        }
        tex.SetPixels(cols);
        tex.Apply();
        return tex;
    }

    Texture2D applyFalloff(Texture2D tex,bool rivFalloff=false)
    {
        Color[] col1=tex.GetPixels();
        Color[] col2;
        if (rivFalloff)
            col2 = makeRivFalloff().GetPixels();
        else
         col2=makeFalloff().GetPixels();
        Texture2D nTex = new Texture2D(1000, 1000);
        for (int i=0; i<col1.Length; i++)
        {
            float val = col1[i].r * col2[i].r;
            col1[i]=new Color(val,val,val);
        }
        nTex.SetPixels(col1);
        nTex.Apply();
        return nTex;
    }
    Texture2D avgTex(Texture2D tex1,Texture2D tex2)
    {
        Color[] col1=tex1.GetPixels();
        Color[] col2=tex2.GetPixels();
        Color[] fO=makeFalloff().GetPixels();
        Texture2D tex = new Texture2D(tex1.width, tex1.width);
        for (int i=0; i<col1.Length; i++)
        {

            float val = (col1[i].r-(col2[i].r*0.12f*riverHeightCurve.Evaluate(col1[i].r)  ))   *fO[i].r; // minuses river map (rivers made less effectove at higher hiehgts) then muiltiply on falloff map
            col1[i]=new Color(val,val,val);
          //  float rivval = 1-(col2[i].r * 0.09f * (0.6f - col1[i].r));
          //  col2[i]=new Color(rivval, rivval, rivval);
        }
        tex.SetPixels(col1);
        tex.Apply();
     
        return tex;
    }
   
}
