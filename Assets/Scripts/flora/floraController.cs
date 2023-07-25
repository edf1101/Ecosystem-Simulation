using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floraController : MonoBehaviour
{
    // Start is called before the first frame update 
    Vector3 baseSize;
    public float size = 1;
    [SerializeField] float regrowMult = 1;
    [SerializeField] float eatableThresh=0.65f;
    public timeController TC;
    public bool owner;
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
        transform.localScale = baseSize * size;
        if (size < 1)
        {
            size += Time.deltaTime * 0.06f*regrowMult*TC.multiplier;
        }
        if (size > 1)
            size = 1;

    }
    public void Eat()
    {
        size = 0;
        owner = false;
    }
}
