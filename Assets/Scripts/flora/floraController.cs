using UnityEngine;

// This script manages plant growth
public class floraController : MonoBehaviour
{
    
    Vector3 baseSize;
    public float size = 1;
    [SerializeField] float regrowMult = 1;
    [SerializeField] float eatableThresh=0.65f;
    public timeController TC;
    public bool owner;

    //return whether a plant is big enought to be eatable
    public bool eatable
    {
        get { return size > eatableThresh; }

    } 

    void Start()
    {
        eatableThresh = 0.75f;
        baseSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        // if smaller than size 1 regrow it 
        transform.localScale = baseSize * size;
        if (size < 1)
        {
            size += Time.deltaTime * 0.06f*regrowMult*TC.multiplier;
        }
        if (size > 1)
            size = 1;

    }

    // set size to 0 when eaten
    public void Eat()
    {
        size = 0;
        owner = false;
    }
}
