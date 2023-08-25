using UnityEngine;
using UnityEngine.UI;

// This script creates the hover menu for the mouse when it goes over a datapoint
// in the graph


public class mouseScript : MonoBehaviour
{
    private GameObject look;
    public int hoursLook;
    public float dataLook;
    public Image im;
    public Text tex;


    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;

        // find nearby colliders to the mouse
        Collider2D cols= Physics2D.OverlapCircle(transform.position, 10f);

        if (cols != null) // if there is one nearby
        {
            // set the menu details to be enabled and to the point were looking ats data
            look = cols.gameObject;
            hoursLook = look.GetComponentInChildren<pointData>().hours;
            dataLook = look.GetComponentInChildren<pointData>().data;
            tex.enabled = true;
            im.enabled = true;
            tex.text = "Hours: " + hoursLook.ToString() + "\nData: " + dataLook.ToString();
        }
        else // if cant find any points disable the pop up menu
        {
            look = null;
            dataLook = 0;
            hoursLook = 0;

            tex.enabled = false;
            im.enabled = false;

        }
    }
}
