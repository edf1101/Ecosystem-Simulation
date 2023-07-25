using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class quickExit : MonoBehaviour
{
        
   public void exitNow() { SceneManager.LoadScene("Menu"); }
}
