using UnityEngine;


// This script controls weather in the scene
public class weatherManager : MonoBehaviour
{
   // External references
    [SerializeField] private biomeGenerator BG;
    public timeController TC;
    public GameObject controller;

    //Textures
    private Texture2D baseTemp;// base conditions based on biome
    private Texture2D baseHum;
    private Texture2D perlTemp;// conditions based on perlin noise
    private Texture2D perlHum;
    public Texture2D comHum;//combined conditions
    public Texture2D comTemp;


    // Weather settings
    private Vector2 offsetTemp;
    private Vector2 offsetHum;
    public float speed;
    private float lastTemp;
    private float lastHum;

    //Particle systems
    private ParticleSystem[,] PSs;
    public GameObject PSPrefab;
    private float lastPSUpdate;
    public Transform PSloc;


    // Other
    private bool tempTurn;
    public float curentTmp;
    private bool doneStart;
    public float curentHum;

    // set up the weather object
    public void StartWeather()
    {
        doneStart = true; 
        PSs = new ParticleSystem[25, 25];

        // get the base maps for temp and hum
        baseTemp = BG.tempMap;
        baseHum = BG.humMap;

        // Random offset essentially seed so maps are different each time
        offsetTemp = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));
        offsetHum = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));
        comHum = new Texture2D(25, 25);
        comTemp = new Texture2D(25, 25);

        // particle systems will be chunked to save rendering every part of the 250x250m map
        // chunked into 10x10m sections

        for (int y=0;y<25;y++)
        {
            for (int x = 0; x < 25; x++)
            {// go through each section

                //create particle system prefab and assign its location and parent
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
        if (doneStart) // check its been set up
        {
            if (Time.time - lastHum > 0.5f && !tempTurn) // been .5s since last checked humidtiy
            {
                tempTurn = true;
                lastHum = Time.time;

                perlHum = SinglePerlin(25, 2, offsetHum, 0.2f);

                // Get perlin textures and base textures
                Color[] perlCols = perlHum.GetPixels();
                Color[] baseCols = baseHum.GetPixels();

                // combine the textures
                Color[] comCols = new Color[25 * 25];
                for (int i = 0; i < 25 * 25; i++)
                {
                    float humVal = baseCols[i].r + (perlCols[i].r - 0.1f);
                    comCols[i] = new Color(humVal, humVal, humVal);
                }
                comHum.SetPixels(comCols);
                comHum.Apply();

            }

            if (Time.time - lastTemp > 0.5f && tempTurn)// been .5s since last checked temp
            {
                lastTemp = Time.time;

                // Get perlin textures and base textures
                perlTemp = SinglePerlin(25, 2, offsetTemp, 0.2f);
                tempTurn = false;
                Color[] perlCols = perlTemp.GetPixels();
                Color[] baseCols = baseTemp.GetPixels();

                // combine the textures
                Color[] comCols = new Color[25 * 25];
                for (int i = 0; i < 25 * 25; i++)
                {
                    float humVal = baseCols[i].r + (perlCols[i].r - 0.1f);
                    comCols[i] = new Color(humVal, humVal, humVal);
                }
                comTemp.SetPixels(comCols);
                comTemp.Apply();
            }

            // add to the offset so the weather map moves with time
            offsetHum += new Vector2(speed * TC.multiplier, speed * TC.multiplier * 0.5f * Time.deltaTime);
            offsetTemp += new Vector2(speed * TC.multiplier, speed * TC.multiplier * 0.5f * Time.deltaTime);

            
            if (Time.time - lastPSUpdate > 0.1f)// update the particle system every .1secs
            {
                lastPSUpdate = Time.time;



                // go through each of the 10mx10m tiles
                for (int y = 0; y < 25; y++)
                {
                    for (int x = 0; x < 25; x++)
                    {

                        // check if the tile is within range if so render it
                        if (Vector2.Distance(new Vector2(controller.transform.position.x, controller.transform.position.z), new Vector2((x * 10) + 12.5f, (y * 10) + 12.5f)) < 120)
                        {
                            // get the current Temperature and humidity at that point
                            curentTmp = comTemp.GetPixel((int)controller.transform.position.x / 10, (int)controller.transform.position.z / 10).r;
                            curentHum = comHum.GetPixel((int)controller.transform.position.x / 10, (int)controller.transform.position.z / 10).r;

                            // if the time is paused pause the particles
                            if (TC.multiplier == 0 && !PSs[x, y].isPaused)
                            {
                                PSs[x, y].Pause();
                            }
                            // unpause particles when game unpaused
                            else if(TC.multiplier != 0&&PSs[x,y].isPaused)
                            {
                                PSs[x, y].Play();
                            }


                            // if temp is low and humidity high then make it snow
                            if (comTemp.GetPixel(x, y).r < 0.17f && comHum.GetPixel(x, y).r > 0.4f)
                            {
                                //snow
                                ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[PSs[x, y].main.maxParticles];
                                PSs[x, y].GetParticles(m_Particles);

                                // set all particles to white
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
                            // if temp is high and humidity high then make it rain
                            else if (comTemp.GetPixel(x, y).r > 0.17f && comHum.GetPixel(x, y).r > 0.4f)
                            {
                                //rain
                                ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[PSs[x, y].main.maxParticles];
                                PSs[x, y].GetParticles(m_Particles);

                                // set all particles to blue
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


    // Generates a single perlin noise texture
    private Texture2D SinglePerlin(int size, float scale, Vector2 offset, float amplitude)
    {
        float[] octaveFrequencies = new float[] { scale };
        float[] octaveAmplitudes = new float[] { amplitude };

        Color[] cols = new Color[size * size];

        // go through each pixel in texture

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float val = 0;
                // add perlin noise value for that coordinate
                val += octaveAmplitudes[0] * Mathf.PerlinNoise(offset.x + octaveFrequencies[0] / size * x + .3f, offset.y + octaveFrequencies[0] / size * y + .3f);
                val = Mathf.Clamp(val, 0, 1);
                cols[(y * size) + x] = new Color(val, val, val);
            }
        }

        Texture2D tex = new Texture2D(size, size);
        tex.SetPixels(cols);
        tex.Apply();
        return tex; // return texture
    }
}
