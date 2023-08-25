using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this script makes animals glow / highlighted when mouse goes over them
public class glowController : MonoBehaviour
{
    // Start is called before the first frame update
    private bool highlight; // is it highlighed
    private float lastHighlight;
    public float highlightStage;

    private Material[] mats;
    private List<Material> Oldmats; // old materials for the animal
    private bool started;

    // use our own start func so we can decide when to call it
    public void myStart()
    {
        // get the original materials for the object
        mats = GetComponentInChildren<Renderer>().materials;
        Oldmats = new List<Material>();
        for (int i=0; i < mats.Length; i++)
        {
            Material temp = new Material(mats[0].shader);

            temp.color = new Color(mats[i].color.r, mats[i].color.g, mats[i].color.b);
            Oldmats.Add(temp);
        }


        started = true; // so we know its been initilised
    }

    // Update is called once per frame
    void Update()
    {
        if (started) // check myStart has run
        {
            // if highlighted hasnt been called in last .1s turn it off
            if (Time.time - lastHighlight > 0.1f)
            {
                highlight = false;
            }

            // if highlighted lerp and clamp highlight value towards 1.0f
            if (highlight)
            {
                highlightStage += Time.deltaTime * 10;
                if (highlightStage > 1)
                    highlightStage = 1;
            }

            // if not highlighted lerp towards 0
            if (!highlight)
            {
                highlightStage -= Time.deltaTime * 10;
                if (highlightStage < 0)
                    highlightStage = 0;
            }

            // if highlighting then set colour to lerp more towards white depending on highlightstage

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

    // set externally to highlighted
    public void highlighted()
    {
        highlight = true;
        lastHighlight = Time.time;
    }
}
