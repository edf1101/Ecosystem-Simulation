using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weatherManager : MonoBehaviour
{
   
   [SerializeField] biomeGenerator BG;

     Texture2D baseTemp;
     Texture2D baseHum;
     Vector2 offsetTemp;
     Vector2 offsetHum;
    public timeController TC;
    public float speed;
    float lastTemp;
    float lastHum;
     Texture2D perlTemp;
     Texture2D perlHum;
    public Texture2D comHum;
    public Texture2D comTemp;
    bool tempTurn;
    ParticleSystem[,] PSs;
  public GameObject PSPrefab;
   float lastPSUpdate;
    public Transform PSloc;
    public GameObject controller;
    public float curentTmp;
    bool doneStart;
    public float curentHum;
    public void StartWeather()
    {
        doneStart = true;
        PSs = new ParticleSystem[25, 25];
        baseTemp = BG.tempMap;
        baseHum = BG.humMap;
        offsetTemp = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));
        offsetHum = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));
        comHum = new Texture2D(25, 25);
        comTemp = new Texture2D(25, 25);
        
        for (int y=0;y<25;y++)
        {
            for (int x = 0; x < 25; x++)
            {
                GameObject temp = Instantiate(PSPrefab);
                temp.transform.position = Vector3.zero;
                temp.transform.parent = PSloc;
                var sh = temp.GetComponent<ParticleSystem>().shape;
                sh.position=new Vector3(12.5f + (x * 10), 75, 12.5f+(y*10));
                PSs[x,y]= temp.GetComponent<ParticleSystem>();

            }
        }

    }

   
    void Update()
    {
        if (doneStart)
        {
            if (Time.time - lastHum > 0.5f && !tempTurn)
            {
                tempTurn = true;
                lastHum = Time.time;

                perlHum = SinglePerlin(25, 2, offsetHum, 0.2f);
                Color[] perlCols = perlHum.GetPixels();
                Color[] baseCols = baseHum.GetPixels();
                Color[] comCols = new Color[25 * 25];
                for (int i = 0; i < 25 * 25; i++)
                {
                    float humVal = baseCols[i].r + (perlCols[i].r - 0.1f);
                    comCols[i] = new Color(humVal, humVal, humVal);
                }
                comHum.SetPixels(comCols);
                comHum.Apply();

            }
            if (Time.time - lastTemp > 0.5f && tempTurn)
            {
                lastTemp = Time.time;
                perlTemp = SinglePerlin(25, 2, offsetTemp, 0.2f);
                tempTurn = false;
                Color[] perlCols = perlTemp.GetPixels();
                Color[] baseCols = baseTemp.GetPixels();
                Color[] comCols = new Color[25 * 25];
                for (int i = 0; i < 25 * 25; i++)
                {
                    float humVal = baseCols[i].r + (perlCols[i].r - 0.1f);
                    comCols[i] = new Color(humVal, humVal, humVal);
                }
                comTemp.SetPixels(comCols);
                comTemp.Apply();
            }
            offsetHum += new Vector2(speed * TC.multiplier, speed * TC.multiplier * 0.5f * Time.deltaTime);
            offsetTemp += new Vector2(speed * TC.multiplier, speed * TC.multiplier * 0.5f * Time.deltaTime);

            if (Time.time - lastPSUpdate > 0.1f)
            {
                lastPSUpdate = Time.time;
                for (int y = 0; y < 25; y++)
                {
                    for (int x = 0; x < 25; x++)
                    {

                        if (Vector2.Distance(new Vector2(controller.transform.position.x, controller.transform.position.z), new Vector2((x * 10) + 12.5f, (y * 10) + 12.5f)) < 120)
                        {
                            curentTmp = comTemp.GetPixel((int)controller.transform.position.x / 10, (int)controller.transform.position.z / 10).r;
                            curentHum = comHum.GetPixel((int)controller.transform.position.x / 10, (int)controller.transform.position.z / 10).r;
                            if (TC.multiplier == 0 && !PSs[x, y].isPaused)
                            {
                                PSs[x, y].Pause();
                              //  print("tryoing");
                            }
                            else if(TC.multiplier != 0&&PSs[x,y].isPaused)
                            {
                                PSs[x, y].Play();
                            }
                            if (comTemp.GetPixel(x, y).r < 0.17f && comHum.GetPixel(x, y).r > 0.4f)
                            {
                                //snow
                                ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[PSs[x, y].main.maxParticles];
                                PSs[x, y].GetParticles(m_Particles);
                                for (int i = 0; i < m_Particles.Length; i++)
                                {
                                    m_Particles[i].startColor = new Color32(240, 240, 240, 240);
                                    m_Particles[i].SetMeshIndex(1);
                                    m_Particles[i].startSize3D = new Vector3(5, 5, 5);
                                }
                                PSs[x, y].SetParticles(m_Particles);
                                if(!PSs[x,y].isPaused)
                                PSs[x, y].Play();

                            }
                            else if (comTemp.GetPixel(x, y).r > 0.17f && comHum.GetPixel(x, y).r > 0.4f)
                            {
                                //rain
                                ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[PSs[x, y].main.maxParticles];
                                PSs[x, y].GetParticles(m_Particles);
                                for (int i = 0; i < m_Particles.Length; i++)
                                {
                                    m_Particles[i].startColor = new Color32(78, 124, 195, 166);
                                    m_Particles[i].startSize3D = new Vector3(4, 4, 10);
                                    m_Particles[i].SetMeshIndex(0);
                                }
                                PSs[x, y].SetParticles(m_Particles);
                                if (!PSs[x, y].isPaused)
                                    PSs[x, y].Play();

                            }
                            else
                            {
                                PSs[x, y].Stop();
                            }


                        }
                        else
                        {
                            PSs[x, y].Stop();
                        }
                    }
                }
            }
        }
    }



     Texture2D SinglePerlin(int size, float scale, Vector2 offset, float amplitude)
    {
        float[] octaveFrequencies = new float[] { scale };
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
}
