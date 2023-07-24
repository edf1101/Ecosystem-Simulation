using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour // this manages the gameplay menu
{
    
    public bool menuOut=true;
    public timeController TC;
    //  public fly Fly;
    float lastMulti=1;
    float menuX = -532;
    int going = 0;
    public RectTransform holder;
    public playerController PC;
    [SerializeField] statsManager SM;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Menu")&&!PC.Spectating) // stuff to lock mouse if menu in/ out
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
        if (going == 1) // lerps the menu position in and out of screen so smooth
        {
            menuX += 6000 * Time.deltaTime;
            if (menuX >= 0)
            {
                menuX = 0;
                going = 0;
            }
            holder.localPosition = new Vector3(menuX, 0, 0);
        }
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
    public void quit() // funcs below get mapped to buttons ie quit button start scrren button 
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
    IEnumerator waitToStats() // IENumerator so U wait 2s before going to stats menu so the StatisticsManager has time to create csv file
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Stats");

    }
}
