using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class glowController : MonoBehaviour
{
    // Start is called before the first frame update
     bool highlight;
    float lastHighlight;
   public float highlightStage;
    Material[] mats;
    List<Material> Oldmats;
    bool started;
   public void myStart()
    {
        mats = GetComponentInChildren<Renderer>().materials;
        Oldmats = new List<Material>();
       // GetComponentInChildren<Renderer>().GetMaterials(Oldmats);
        //print("!");
        for (int i=0; i < mats.Length; i++)
        {
            Material temp = new Material(mats[0].shader);

            temp.color = new Color(mats[i].color.r, mats[i].color.g, mats[i].color.b);
            Oldmats.Add(temp);
        }
        started = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            if (Time.time - lastHighlight > 0.1f)
            {
                highlight = false;
            }

            if (highlight)
            {
                highlightStage += Time.deltaTime * 10;
                if (highlightStage > 1)
                    highlightStage = 1;
            }
            if (!highlight)
            {
                highlightStage -= Time.deltaTime * 10;
                if (highlightStage < 0)
                    highlightStage = 0;
            }

            if (highlightStage != 0)
            {
                mats = GetComponentInChildren<Renderer>().materials;
                for (int i = 0; i < mats.Length; i++)
                {


                    Color tempCol = Color.Lerp(Oldmats[i].color, Color.white, 0.7f);

                    mats[i].color = Color.Lerp(Oldmats[i].color, tempCol, highlightStage);
                }
            }
        }
    }
    public void highlighted()
    {
        highlight = true;
        lastHighlight = Time.time;
    }
}
