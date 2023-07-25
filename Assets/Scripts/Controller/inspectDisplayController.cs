using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class inspectDisplayController : MonoBehaviour
{
    public bool mouseOver;
    [SerializeField] GameObject resizer;
    [SerializeField] GameObject canvas;
    public bool showCanvas;


    public Text health;
    public Text thirst;
    public Text hunger;
    public Text minTemp;
    public Text maxTemp;
    public Text speed;
    public Text gen;
    public Text agg;

    public Slider healthSlid;
    public Slider thirstSlid;
    public Slider hungerSlid;
    public Slider minTempSlid;
    public Slider maxTempSlid;
    public Slider speedSlid;
    public Slider aggSlid;


    public animalController AC;
    public Text name;
    void Start()
    {
        resizer.GetComponent<RectTransform>().localPosition = new Vector3(0, (canvas.GetComponent<RectTransform>().rect.height/2)-350, 0); 
    }

    // Update is called once per frame
    void Update()
    {

        canvas.SetActive(showCanvas);
        if (AC != null)
        {
            health.text = "Health: " + Mathf.RoundToInt(AC.health).ToString() + "%";
            thirst.text = "Thirst: " + Mathf.RoundToInt(AC.thirst).ToString() + "%";
            hunger.text = "Hunger: " + Mathf.RoundToInt(AC.hunger).ToString() + "%";
            minTemp.text = "Min Temp: " + Mathf.RoundToInt(100f*AC.minTemp).ToString() + "°C";
            maxTemp.text = "Max Temp: " + Mathf.RoundToInt(100f * AC.maxTemp).ToString()  + "°C";
            speed.text = "Speed: " + (AC.speed).ToString() ;
            gen.text = "Generation: " + Mathf.RoundToInt(AC.GetComponent<AnimalCreator>().generation).ToString() ;
            agg.text = "Aggression: " + Mathf.RoundToInt(100f * AC.aggression).ToString() + "%";

            

            name.text = "Name: " + AC.gameObject.name;
        }
    }

    public void healthSet() { AC.health = healthSlid.value; }
    public void thirstSet() { AC.thirst = thirstSlid.value; }
    public void hungerSet() { AC.hunger = hungerSlid.value; }
    public void minSet() { AC.minTemp = minTempSlid.value; }
    public void maxSet() { AC.maxTemp = maxTempSlid.value; }
    public void speedSet() { AC.speed = speedSlid.value; }
    public void aggSet() { AC.aggression = aggSlid.value; }
    //public void speedSet() { AC.health = healthSlid.value; }
}
