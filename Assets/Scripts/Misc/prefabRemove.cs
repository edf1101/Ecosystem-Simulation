using UnityEngine; 
// the same as deleteAfter

// it deletes the object it is attatched to after x seconds

public class prefabRemove : MonoBehaviour
{
    
    [SerializeField] float time;
    void Start()
    {
        Destroy(gameObject,time);  
    }

   
}
