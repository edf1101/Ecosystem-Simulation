using UnityEngine;
using UnityEngine.SceneManagement;

public class quickExit : MonoBehaviour // simple script to go to main menu when click exit button in simulation pause menu
{
        
   public void exitNow() { SceneManager.LoadScene("Menu"); }
}
