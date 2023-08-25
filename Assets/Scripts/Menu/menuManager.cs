using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


// this script controls the menu 
public class menuManager : MonoBehaviour
{
   
    public bool menuOut=true;
    public timeController TC;
    private float lastMulti=1;
    private float menuX = -532;
    private int going = 0;
    public RectTransform holder;
    public playerController myPlayer;

    [SerializeField] private statsManager SM;

    // Update is called once per frame
    void Update()
    {
        // if menu button pressed then toggle whether its in or out
        // also toggle mouse lock status
        if (Input.GetButtonDown("Menu")&&!myPlayer.Spectating)
        {
            if (menuOut)
            {
                menuOut = false;
                Cursor.lockState= CursorLockMode.None;
                Cursor.visible = true;
                lastMulti = TC.multiplier;
                TC.multiplier = 0;
                going = 1;
            }
            else
            {
                going = -1;
                menuOut = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                TC.multiplier = lastMulti;

            }

        }

        // If going out then lerp its position onto screen
        if (going == 1)
        {
            menuX += 6000 * Time.deltaTime;
            if (menuX >= 0)
            {
                menuX = 0;
                going = 0;
            }
            holder.localPosition = new Vector3(menuX, 0, 0);
        }
        // If going in then lerp its position onto screen
        if (going == -1)
        {
            menuX -= 6000 * Time.deltaTime;
            if (menuX <= -532)
            {
                menuX = -532;
                going = 0;
            }
            holder.localPosition = new Vector3(menuX, 0, 0);
        }
    }



    // public procedures that the buttons call

    public void quit()
    {
        Application.Quit();
    }
    public void gotoStartScreen()
    {
        SceneManager.LoadScene("Menu");

    }

    public void gotoStats()
    {
        if (TC.timeHours > 2)
        {
            SM.exportStats();
            StartCoroutine(waitToStats());
        }
    }
    IEnumerator waitToStats()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Stats");

    }
}
