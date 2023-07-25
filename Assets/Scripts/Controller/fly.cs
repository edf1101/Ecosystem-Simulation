using UnityEngine;

public class fly : MonoBehaviour
{

    [SerializeField]  float movementSpeed = 7f;
    Rigidbody rb;
    [SerializeField] timeController TC;
    float prevMultiplier = 1;
    bool looking;
    GameObject selected;
    float lastSelected;
    Vector3 myCamPos;
  public  bool Spectating;
    int specStage;
    GameObject followSelect;
    Quaternion lastCamRot;
    bool specOut;

    public menuManager menuM;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb =GetComponent<Rigidbody>();
    }
    void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal")*1.5f, Input.GetAxis("Depth")*1.5f, Input.GetAxis("Vertical"));
       // movementSpeed = Mathf.Max(movementSpeed += Input.GetAxis("Mouse ScrollWheel"), 0.0f);
        Vector3 targetV=((transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") + transform.up * Input.GetAxis("Depth")) * movementSpeed);
        if(!looking && menuM.menuOut)
        transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"),0);
 
        if (movement == Vector3.zero)
            targetV = Vector3.zero;

        if(!looking)
            rb.velocity = Vector3.Lerp(rb.velocity, targetV, 0.99f * Time.deltaTime);
        else
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 10f * Time.deltaTime);

        if (Input.GetButtonDown("Inspect") && menuM.menuOut)
        {
            if(TC.multiplier!=0)
                prevMultiplier = TC.multiplier;
            TC.multiplier = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            looking = true;
        }


        if (Input.GetButtonUp("Inspect"))
        {
            
            TC.multiplier = prevMultiplier;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            looking = false;
        }

        if (looking)
        {
           // print(Input.mousePosition);
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition ) ;
            RaycastHit hit;
            if ( (Input.mousePosition.x > 350 || !Spectating) && Physics.Raycast(ray, out hit, 300,~0,QueryTriggerInteraction.Collide))
            {
                if (hit.collider.gameObject.GetComponentInParent<animalController>())
                {
                    // Debug.Log(hit.transform.name);
                    selected = hit.collider.gameObject;
                    lastSelected = Time.time;
                    hit.collider.gameObject.GetComponentInParent<glowController>().highlighted();
                }
                
            }

            if ((Input.mousePosition.x > 350 || !Spectating) && Input.GetMouseButtonDown(0) && selected != null)
            {
                Spectating = true;
                specStage = 1;
                if (followSelect == null)
                {
                    myCamPos = transform.position;
                    lastCamRot = transform.rotation;
                }
                if (followSelect != null)
                    followSelect.GetComponentInChildren<Renderer>().enabled = true;
                followSelect = selected;

            }

            
        }
        if (Time.time - lastSelected > 0.3f && !Spectating)
            selected = null;

        if (((Input.GetKey(KeyCode.Escape)) && Spectating))
        {
            specOut = true;
            Spectating = false;
            specStage = 3;
        }
        GetComponent<inspectDisplayController>().showCanvas = Spectating;

        if (specStage == 1 && Spectating)
        {
            transform.position = Vector3.Lerp(transform.position, followSelect.GetComponent<animalController>().myHead.transform.position, 10 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, followSelect.GetComponent<animalController>().myHead.transform.rotation, 10 * Time.deltaTime);
            if (Vector3.Distance(transform.position, followSelect.GetComponent<animalController>().myHead.transform.position)<0.2f)
            {
                specStage = 2;
               // GetComponent<inspectDisplayController>().showCanvas = true;
                GetComponent<inspectDisplayController>().AC = followSelect.GetComponent<animalController>();
            }
            if (Vector3.Distance(transform.position, followSelect.GetComponent<animalController>().myHead.transform.position) < 0.8f)
            {
                followSelect.GetComponentInChildren<Renderer>().enabled = false;
            }
        }
        if (specStage == 2 && Spectating)
        {
            if (followSelect.GetComponentInChildren<Renderer>().enabled)
                followSelect.GetComponentInChildren<Renderer>().enabled = false;
            transform.position = followSelect.GetComponent<animalController>().myHead.transform.position;
            transform.rotation = followSelect.GetComponent<animalController>().myHead.transform.rotation;

        }
        
        if (specStage == 3 && specOut)
        {
            transform.position = Vector3.Lerp(transform.position, myCamPos, 10 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, lastCamRot, 10 * Time.deltaTime);
            followSelect.GetComponentInChildren<Renderer>().enabled = true ;
            if (Vector3.Distance(transform.position, myCamPos) < 0.2f)
            {
                specStage = 4;
            }
        }
        if (specStage == 4 && specOut)
        {
            followSelect = null;
            selected = null;
            Spectating = false;
            specOut = false;
            specStage = 0;
            Cursor.lockState = CursorLockMode.Locked;
          //  GetComponent<inspectDisplayController>().showCanvas = false;
          looking = Input.GetButton("Inspect");

        }
    }

}

