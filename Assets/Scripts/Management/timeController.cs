using UnityEngine;

public class timeController : MonoBehaviour
{
   
    public float speed; // needs to be equal to lighting speed;
    public float timeHours = 0;
    public float multiplier = 1;
    

   
    void Update() // each frame add a bit to the total time elapsed variable depending on speed and multiplier
    {
        timeHours += (Time.deltaTime) * (speed*multiplier);
    }
}
