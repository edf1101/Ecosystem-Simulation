using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class animalPath : MonoBehaviour
{
    
  public float speed = 1;
 public    Vector3[] path;
    int targetIndex;
LayerMask layerMask;
    public bool moving;
    Animator AN;
    Vector3 dir;
    bool innited;
    Vector2[] poss;
    public int animalDrinkINT = 0;
    float startDrink;
    public Vector3 animalDrinkVEC;
    public Vector3 animalDrinkVECorg;
    bool feeding;
    float lastMoveTo;
    bool aniMove;
   public float heightOffset;
    public bool movePath;
    public bool moveCheap;
    public bool debugME;
    public bool startedReq;
    bool started;
    private void init()
    {
        started = true;
        innited = true;
        AN = GetComponent<Animator>();
        layerMask = LayerMask.GetMask("islandBaseMesh");
        
        poss = GetComponent<animalController>().spawner.allSpawns.Keys.ToArray<Vector2>(); ;
        

    }
    public void moveTo(Vector3 _target,int _why)
    {
        
           // lastMoveTo = Time.time;
            if (!innited)
                init();
        
            
        moveCheap = _why == 1;
        movePath = _why == 0;
        if (movePath)
        {
            startedReq = true;
                reqManager.RequestPath(transform.position, _target, onPathFound,gameObject);

        }
        if (moveCheap)
        {


            onPathFound(new Vector3[] { _target }, true);
        }
        
    }
    public void animalDrink(Vector3 pos,bool _feeding)
    {
        feeding = _feeding;
        animalDrinkINT = 1;
        animalDrinkVEC = pos;
        animalDrinkVECorg=transform.position;
        
        //StopCoroutine("FollowPath");
        moving = false;
    }
   
    public void onPathFound(Vector3[] newPath,bool pathSuccess)
    {
        startedReq = false;
        
        if (pathSuccess)
        {
           
            if(newPath.Length == 0)
            {
                
                StopCoroutine("FollowPath");
                //  StopCoroutine("CheapMove");
                GetComponent<animalController>().badSpot = true;
                moving = false;
                return;
            }
            targetIndex = 0;
            path = newPath;
           // StopCoroutine("CheapMove");
           
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
            moving = true;
            aniMove = true;
        }
        else
        {
            
            moving = false;
            aniMove = false;
            GetComponent<animalController>().badSpot = true;
        }
           
    }

    private void Update()
    {
        if (started)
        {
            if (!innited)
                init();
            speed = GetComponent<animalController>().speed;



            //FOOD AND DRINK   
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
            if (animalDrinkINT == 2)
            {
                AN.SetBool("moving", false);
                startDrink = Time.time;
                AN.SetBool("consuming", true);

                animalDrinkINT = 3;


            }
            if (animalDrinkINT == 3 && Time.time - startDrink > 3f)
            {




                animalDrinkINT = 4;


            }
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
                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(animalDrinkVECorg.x, animalDrinkVECorg.z)) < 0.05f)

                {
                    AN.SetBool("moving", false);
                    transform.position = animalDrinkVECorg;
                    animalDrinkINT = 0;
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

   Vector2 closestAv()
    {
        Vector2[] poss = GetComponent<animalController>().poss;
        float min = 1000;
        Vector2 minPos=new Vector2();
        for(int i = 0; i < poss.Length; i++)
        {
            if(Vector2.Distance(new Vector2(transform.position.x,transform.position.z),poss[i]) < min)
            {
                min = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), poss[i]);
                minPos = poss[i];
            }
        }
        return minPos;
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            
            if (new Vector3(transform.position.x,currentWaypoint.y,transform.position.z) == currentWaypoint)
            {
                targetIndex++;
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

            Vector3 point3dBad = Vector3.MoveTowards(   new Vector3(transform.position.x,0,transform.position.z)     , new Vector3(currentWaypoint.x,0,currentWaypoint.z), speed*Time.deltaTime);

            Vector3 pastpos=transform.position;
            transform.position = new Vector3(point3dBad.x, getMeshPos(new Vector2(point3dBad.x, point3dBad.z)).y+heightOffset, point3dBad.z);
            dir=(transform.position - pastpos).normalized;
        // transform.position = point3dBad;
            yield return null;
        }
    }
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
