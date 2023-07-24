using UnityEngine;

public class deleteAfter : MonoBehaviour // this script can be attached to a temporary object so it destroys itself x seconds after being instantiated
{
    [SerializeField] float time;

    void Start()
    {
        Destroy(gameObject, time);
    }

   
}
