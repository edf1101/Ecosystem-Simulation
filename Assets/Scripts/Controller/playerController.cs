using UnityEngine;

public class playerController : MonoBehaviour
{

    [SerializeField] private float movementSpeed = 7f;
    private Rigidbody rb;
    private float prevMultiplier = 1;
    
    //spectating
    private bool specOut;
    public bool Spectating;
    private int specStage;

    // looking/ selection
    private bool looking;
    private GameObject selected;
    private float lastSelected;
    private Vector3 myCamPos;
    private GameObject followSelect;
    private Quaternion lastCamRot;

    // script refereences
    public menuManager menuM; 
    [SerializeField] private timeController TC;




    // called before first frame
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb =GetComponent<Rigidbody>();
    }

    // called each frame
    private void Update()
    {

        // target velocities based on input vectors
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal")*1.5f, Input.GetAxis("Depth")*1.5f, Input.GetAxis("Vertical"));
        Vector3 targetV=((transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") + transform.up * Input.GetAxis("Depth")) * movementSpeed);

        // if menus not open and mouse availible change camera rotation
        if(!looking && menuM.menuOut)
        transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"),0);
 
        if (movement == Vector3.zero)
            targetV = Vector3.zero;


        // smooth camera rotation too
        if(!looking)
            rb.velocity = Vector3.Lerp(rb.velocity, targetV, 0.99f * Time.deltaTime);
        else
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 10f * Time.deltaTime);



        // This can control when in inspection mode
        if (Input.GetButtonDown("Inspect") && menuM.menuOut) // start inspecting
        {
            if(TC.multiplier!=0)
                prevMultiplier = TC.multiplier;
            TC.multiplier = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            looking = true;
        }

        if (Input.GetButtonUp("Inspect")) // stop inspecting
        {
            
            TC.multiplier = prevMultiplier;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            looking = false;
        }

        // if in inspect mode get ready to find animals at mouse cursor
        if (looking)
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition ) ;
            RaycastHit hit;
            // send a ray from the camera

            // if mouse is not in menu area
            if ( (Input.mousePosition.x > 350 || !Spectating) && Physics.Raycast(ray, out hit, 300,~0,QueryTriggerInteraction.Collide))
            {
                if (hit.collider.gameObject.GetComponentInParent<animalController>()) // and hit an animal
                {
                    // select that animal
                    selected = hit.collider.gameObject;
                    lastSelected = Time.time;
                    // and highlight it
                    hit.collider.gameObject.GetComponentInParent<glowController>().highlighted();
                }
                
            }

            // when mouse down start spectating that animal
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

        // when its been .3s since mouse last touched animal stop selecting it 
        if (Time.time - lastSelected > 0.3f && !Spectating)
            selected = null;

        // if esc pressed stop spectating it 
        if (((Input.GetKey(KeyCode.Escape)) && Spectating))
        {
            specOut = true;
            Spectating = false;
            specStage = 3;
        }
        GetComponent<inspectDisplayController>().showCanvas = Spectating;

        // This is for spectation mechanism

        // when we start spectating lerp my current camera position and rotation to target
        if (specStage == 1 && Spectating)
        {
            // lerp position and rotation
            transform.position = Vector3.Lerp(transform.position, followSelect.GetComponent<animalController>().myHead.transform.position, 10 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, followSelect.GetComponent<animalController>().myHead.transform.rotation, 10 * Time.deltaTime);

            // once there move ot next stage
            if (Vector3.Distance(transform.position, followSelect.GetComponent<animalController>().myHead.transform.position)<0.2f)
            {
                specStage = 2;
                GetComponent<inspectDisplayController>().AC = followSelect.GetComponent<animalController>();
            }

            if (Vector3.Distance(transform.position, followSelect.GetComponent<animalController>().myHead.transform.position) < 0.8f)
            {
                followSelect.GetComponentInChildren<Renderer>().enabled = false;
            }
        }

        if (specStage == 2 && Spectating) // specating animal properly
        {
            if (followSelect.GetComponentInChildren<Renderer>().enabled)
                followSelect.GetComponentInChildren<Renderer>().enabled = false;

            // constantly set my camera to that animals head's position and rotation
            transform.position = followSelect.GetComponent<animalController>().myHead.transform.position;
            transform.rotation = followSelect.GetComponent<animalController>().myHead.transform.rotation;

        }

        // when were trying to stop spectating lerp back to original pos and rot
        if (specStage == 3 && specOut) 
        {

            //lerp postiiton
            transform.position = Vector3.Lerp(transform.position, myCamPos, 10 * Time.deltaTime);
            // lerp rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, lastCamRot, 10 * Time.deltaTime);
            followSelect.GetComponentInChildren<Renderer>().enabled = true ;
            // if at original pos then move to next stage
            if (Vector3.Distance(transform.position, myCamPos) < 0.2f)
            {
                specStage = 4;
            }
        }

        // once finished gettign back to original position can set all variables to finished
        if (specStage == 4 && specOut)
        {
            followSelect = null;
            selected = null;
            Spectating = false;
            specOut = false;
            specStage = 0;
            Cursor.lockState = CursorLockMode.Locked;
          looking = Input.GetButton("Inspect");

        }
    }

}

