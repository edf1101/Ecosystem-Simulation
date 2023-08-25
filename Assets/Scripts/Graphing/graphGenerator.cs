using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


// This script creates the graph that shows statistics at the end
public class graphGenerator: MonoBehaviour
{
    public bool onIt;
    public Vector2 startP;
    public Vector2 scale;

    public float[] points;
    public GameObject pointPre;
    public GameObject linePre;
    public Transform place;

    public Color col;

    public Dropdown myAnimal;
    public Dropdown myAttr;
    public Text peakText;
    public float peak;
    public Text hourMark;


    void Start()
    {
        redoData();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    // this script reads the csv file for each animal and imports the data
    public void redoData()
    {
        string fileName = Application.dataPath + @"\AnimalData\" + myAnimal.options[myAnimal.value].text + ".csv";

        // depending on what characteristic set the max possible value
        switch (myAttr.value)
        {
            case 0:
                peak = 500;
                break;
            case 1:
                peak = 1;
                break;
            case 2:
                peak = 1;
                break;
            case 3:
                peak = 10;
                break;
            case 4:
                peak = 250;
                break;
            case 5:
                peak = 1;
                break;
            case 6:
                peak = 1;
                break;
            case 7:
                peak = 1;
                break;
            
            case 8:
                peak = 1;
                break;
            case 9:
                peak = 17;
                break;
            case 10:
                peak = 30;
                break;
            case 11:
                peak = 30;
                break;
            case 12:
                peak = 1700;
                break;


        }


        peakText.text = peak.ToString();
        TextReader TR = new StreamReader(fileName, true);
        List<float> temp = new List<float>();
        
        string line;
        // Read and display lines from the file until the end of
        // the file is reached.
        line = TR.ReadLine();
        while ((line = TR.ReadLine()) != null)
        {
            temp.Add(100f * ((float.Parse(line.Split(',')[myAttr.value + 1])) / peak));
        }
        points = temp.ToArray();
        TR.Close();



         // get how many hours there are
        hourMark.text = points.Length.ToString();


        //create graph iteslf 
        onIt = false;
        // destroy old markers
        foreach (Transform child in place)
        {
            GameObject.Destroy(child.gameObject);
        }


        float intervals = scale.x / (points.Length - 1); // calculate interaval size
        for (int i = 0; i < points.Length; i++)
        {
            // for line gradients calculate 2 adjacent points
            Vector2 thisPoint = startP + new Vector2(i * intervals, (points[i] / 100f) * scale.y);
            Vector2 nextPoint = Vector2.zero;


            if (i != points.Length - 1)// up to the n-1 th point
            {

                // create a line using y=mx+c 
                nextPoint = startP + new Vector2((i + 1) * intervals, (points[i + 1] / 100f) * scale.y);
                float dis = Vector2.Distance(thisPoint, nextPoint);
                float ang = Mathf.Rad2Deg * Mathf.Atan2(nextPoint.y - thisPoint.y, nextPoint.x - thisPoint.x);

                // instatniate line prefab and set it to correct location and position and length
                GameObject tempLine = Instantiate(linePre);
                tempLine.transform.localScale = new Vector3(dis, 3, 0);
                tempLine.transform.eulerAngles = new Vector3(0, 0, ang);
                tempLine.transform.localPosition = thisPoint;
                tempLine.transform.SetParent(place);
                tempLine.GetComponentInChildren<Image>().color = col;
            }

            // instantiate the dot for each hour too
            GameObject temp1 = Instantiate(pointPre);
            temp1.transform.localPosition = thisPoint;
            temp1.GetComponentInChildren<Image>().color = new Color(col.r * 0.8f, col.g * 0.8f, col.b * 0.8f);
            temp1.GetComponentInChildren<pointData>().data=(peak/100f)*points[i];
            temp1.GetComponentInChildren<pointData>().hours=i;
            temp1.transform.SetParent(place);
            
            
        }
    }
}


