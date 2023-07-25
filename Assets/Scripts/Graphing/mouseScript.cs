using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class mouseScript : MonoBehaviour
{
     GameObject look;
    public int hoursLook;
    public float dataLook;
    public Image im;
    public Text tex;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
        Collider2D cols= Physics2D.OverlapCircle(transform.position, 10f);
        if (cols != null)
        {
            look = cols.gameObject;
            hoursLook = look.GetComponentInChildren<pointData>().hours;
            dataLook = look.GetComponentInChildren<pointData>().data;
            tex.enabled = true;
            im.enabled = true;
            tex.text = "Hours: " + hoursLook.ToString() + "\nData: " + dataLook.ToString();
        }
        else
        {
            look = null;
            dataLook = 0;
            hoursLook = 0;

            tex.enabled = false;
            im.enabled = false;

        }
    }
}
