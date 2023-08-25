using System.Collections;
using UnityEngine;
using System.Linq;
public class animalPath : MonoBehaviour
{
    // Movement variables
    public float speed = 1;
    public Vector3[] path;
    private int targetIndex;
    public bool moving;
    private Vector2[] poss;
    public bool startedReq;
    private Vector3 dir;
    private bool aniMove;
    public float heightOffset;
    public bool movePath;
    public bool moveCheap;

    // Eating variables
    public int animalDrinkINT = 0;
    private float startDrink;
    public Vector3 animalDrinkVEC;
    public Vector3 animalDrinkVECorg;
    private bool feeding;


    //other
    private LayerMask layerMask;
    private Animator AN;
    private bool innited;
    private bool started;
    public bool debugME;

    // set up for the script not using standard start function as it
    // may not wait for other scripts to be set up 
    private void init()
    {
        started = true;
        innited = true;
        AN = GetComponent<Animator>();
        layerMask = LayerMask.GetMask("islandBaseMesh");
        
        poss = GetComponent<animalController>().spawner.allSpawns.Keys.ToArray<Vector2>(); ;
        

    }

    // public function to move to location
    public void moveTo(Vector3 _target,int _why)
    {
        
           // check that the script is set up
            if (!innited)
                init();
        
        // decide whetehr its a A* pathfinding or simple move directly
        moveCheap = _why == 1;
        movePath = _why == 0;

        // if doing A* pathfinding then request an A* path
        if (movePath)
        {
            startedReq = true;
            reqManager.RequestPath(transform.position, _target, onPathFound,gameObject);

        }

        // if not then create our own path straight to target
        if (moveCheap)
        {
            onPathFound(new Vector3[] { _target }, true);
        }
        
    }


    // called when the animal wants to drink
    public void animalDrink(Vector3 pos,bool _feeding)
    {
        // set up the data required to move to a point
        feeding = _feeding;
        animalDrinkINT = 1;
        animalDrinkVEC = pos;
        animalDrinkVECorg=transform.position;
        
        moving = false;
    }

    // when a path is set by us or A* algorithm
    public void onPathFound(Vector3[] newPath,bool pathSuccess)
    {
        startedReq = false;
        
        if (pathSuccess)
        {
           
            if(newPath.Length == 0)  // path invalid
            {
                
                StopCoroutine("FollowPath");
                GetComponent<animalController>().badSpot = true;
                moving = false;
                return;
            }

            // if path a success and has points in it
            targetIndex = 0;
            path = newPath;

            // start following that path
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
            moving = true;
            aniMove = true;
        }
        else // if not a success stop moving
        {
            
            moving = false;
            aniMove = false;
            GetComponent<animalController>().badSpot = true;
        }
           
    }

    // update each frame
    private void Update()
    {
        if (started)
        {
            if (!innited) // if not intitialise do it now
                init();

            speed = GetComponent<animalController>().speed;



            //FOOD AND DRINK

            // stage 1 move towards the position
            if (animalDrinkINT == 1)
            {
                AN.SetBool("moving", true);
                Vector3 eatpos;
                if (feeding)
                    eatpos = animalDrinkVEC + (new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(GetComponent<animalController>().myHead.transform.position.x, 0, GetComponent<animalController>().myHead.transform.position.z));
                else
                    eatpos = animalDrinkVEC;
                Vector3 point3dBad = Vector3.MoveTowards(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(eatpos.x, 0, eatpos.z), speed * Time.deltaTime);

                Vector3 pastpos = transform.position;
                transform.position = new Vector3(point3dBad.x, getMeshPos(new Vector2(point3dBad.x, point3dBad.z)).y + heightOffset, point3dBad.z);
                dir = (transform.position - pastpos).normalized;
                if (new Vector3(transform.position.x, 0, transform.position.z) == eatpos)
                    animalDrinkINT = 2;
            }

            //stage 2 stop moving and do the eating animation
            if (animalDrinkINT == 2)
            {
                AN.SetBool("moving", false);
                startDrink = Time.time;
                AN.SetBool("consuming", true);

                animalDrinkINT = 3;


            }


            // stop drinking/ eating after 3s
            if (animalDrinkINT == 3 && Time.time - startDrink > 3f)
                animalDrinkINT = 4;

            // once finished eating move back to last  safe position
            if (animalDrinkINT == 4)
            {
                if (GetComponent<animalController>().debug)
                    print("lerping");
                AN.SetBool("moving", true);
                AN.SetBool("consuming", false);
                Vector3 point3dBad = Vector3.MoveTowards(transform.position, animalDrinkVECorg, speed * Time.deltaTime);
                Vector3 lastpos = transform.position;
                transform.position = new Vector3(point3dBad.x, getMeshPos(new Vector2(point3dBad.x, point3dBad.z)).y + heightOffset, point3dBad.z);
                //  transform.position = point3dBad;// new Vector3()
                dir = (transform.position - lastpos).normalized;

                // if i am at correct position
                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(animalDrinkVECorg.x, animalDrinkVECorg.z)) < 0.05f)

                {
                    AN.SetBool("moving", false);
                    transform.position = animalDrinkVECorg;
                    animalDrinkINT = 0;

                    // reset food / thirst values to 100%
                    if (feeding)
                    {
                        GetComponent<animalController>().hunger = GetComponent<animalController>().hunger + (((100 - GetComponent<animalController>().hunger) * GetComponent<animalController>().myFC.size)*GetComponent<animalController>().faunaMul);
                        GetComponent<animalController>().canFood = 0;
                        GetComponent<animalController>().myFC.Eat(); 
                    }

                    else
                    {
                        GetComponent<animalController>().thirst = 100;
                        GetComponent<animalController>().canWaterr = 0;
                    }


                }

            }
            if (animalDrinkINT == 0)
                AN.SetBool("moving", aniMove);


            // LOOK IN DIRECTION
            float targAng = 360 - (Mathf.Rad2Deg * Mathf.Atan2(dir.z, dir.x)) - 90;
            transform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(transform.localEulerAngles.y, targAng, 3f * Time.deltaTime * GetComponent<animalController>().TC.multiplier), 0);






        }
    }

    // each frame do this follow path func if moving
    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            // if im at the waypoint 
            if (new Vector3(transform.position.x,currentWaypoint.y,transform.position.z) == currentWaypoint)
            {
                // move to next waypoint
                targetIndex++;


                // if i have reached the last point
                if(targetIndex >= path.Length)
                {
                    moving = false;
                    aniMove = false;
                    movePath= false;
                    GetComponent < animalController>().badSpot = false;
                    yield break;
                }
                currentWaypoint = path[targetIndex];   
            }

            // lerp to the next waypoint
            Vector3 point3dBad = Vector3.MoveTowards(   new Vector3(transform.position.x,0,transform.position.z)     , new Vector3(currentWaypoint.x,0,currentWaypoint.z), speed*Time.deltaTime);

            Vector3 pastpos=transform.position;
            transform.position = new Vector3(point3dBad.x, getMeshPos(new Vector2(point3dBad.x, point3dBad.z)).y+heightOffset, point3dBad.z);
            dir=(transform.position - pastpos).normalized;

            yield return null;
        }
    }

    // convert 2d point to 3d with correct height value
    public Vector3 getMeshPos(Vector2 pos)
    {
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(pos.x, 200, pos.y), Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
          //  Debug.DrawRay(new Vector3(pos.x, 200, pos.y), Vector3.down*10, Color.green);
            return hit.point;
        }
        return Vector3.zero;
    }
   
}
