using UnityEngine;
using UnityEngine.SceneManagement;

public class quickMenu : MonoBehaviour // quick script to load into the stats scene from simulation side pause menu
{
    public void loadStats() { SceneManager.LoadScene("Stats"); }
}
