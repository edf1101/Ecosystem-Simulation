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
    private void init()
    {
        innited = true;
        AN = GetComponent<Animator>();
        layerMask = LayerMask.GetMask("islandBaseMesh");
        
        poss = GetComponent<animalController>().spawner.allSpawns.Keys.ToArray<Vector2>(); ;
        

    }
    public void moveTo(Vector3 _target,bool _forWater)
    {
        if(!innited)
            init();
        
        moving = true;
        reqManager.RequestPath(transform.position, _target, onPathFound, _forWater);
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
    Vector3 findClose()
    {


        Vector2 pos = new Vector2((int)transform.position.x, (int)transform.position.z);
        List<Vector2> roam = new List<Vector2>();
        for (int i = 0; i < poss.Length; i++)
        {
            if (Mathf.Abs(pos.x - poss[i].x) < 2 && Mathf.Abs(pos.y - poss[i].y) < 2)
                roam.Add(poss[i]);

        }
        return roam[Random.Range(0, roam.Count - 1)];
    }
    public void onPathFound(Vector3[] newPath,bool pathSuccess)
    {
        
        if (pathSuccess)
        {
           
            if(newPath.Length == 0)
            {
                
                StopCoroutine("FollowPath");
                moving = false;
                return;
            }
            targetIndex = 0;
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
            moving = true;
        }
        else
        {
            
            moving = false;
            
        }
           
    }

    private void Update()
    {
        if (!innited)
            init();
        if (animalDrinkINT == 1)
        {
            AN.SetBool("moving", true);
            Vector3 point3dBad = Vector3.MoveTowards(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(animalDrinkVEC.x, 0, animalDrinkVEC.z), speed * Time.deltaTime);

            Vector3 pastpos = transform.position;
            transform.position = new Vector3(point3dBad.x, getMeshPos(new Vector2(point3dBad.x, point3dBad.z)).y + 0.4f, point3dBad.z);
            dir = (transform.position - pastpos).normalized;
            if (new Vector3(transform.position.x,0,transform.position.z) == animalDrinkVEC)
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
            Vector3 lastpos=transform.position;
            transform.position = new Vector3(point3dBad.x, getMeshPos(new Vector2(point3dBad.x, point3dBad.z)).y + 0.4f, point3dBad.z);
          //  transform.position = point3dBad;// new Vector3()
            dir = (transform.position - lastpos).normalized;
            if (Vector2.Distance(new Vector2(transform.position.x,transform.position.z),new Vector2( animalDrinkVECorg.x,animalDrinkVECorg.z)) < 0.05f)

            {
                AN.SetBool("moving", false);
                transform.position = animalDrinkVECorg;
                animalDrinkINT = 0;
                if (feeding)
                {
                    GetComponent<animalController>().hunger = 100;
                    GetComponent<animalController>().canFood = 0;
                }
                   
                else
                {
                    GetComponent<animalController>().thirst = 100;
                    GetComponent<animalController>().canWaterr = 0;
                }
                  
                
            }
                
        }
        if(animalDrinkINT==0)
        AN.SetBool("moving", moving);
        float targAng = 360 - (Mathf.Rad2Deg * Mathf.Atan2(dir.z, dir.x)) - 90;
        transform.localEulerAngles =  new Vector3(0,Mathf.LerpAngle(transform.localEulerAngles.y,targAng,3f*Time.deltaTime),0);
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
                    yield break;
                }
                currentWaypoint = path[targetIndex];   
            }

            Vector3 point3dBad = Vector3.MoveTowards(   new Vector3(transform.position.x,0,transform.position.z)     , new Vector3(currentWaypoint.x,0,currentWaypoint.z), speed*Time.deltaTime);

            Vector3 pastpos=transform.position;
            transform.position = new Vector3(point3dBad.x, getMeshPos(new Vector2(point3dBad.x, point3dBad.z)).y+0.4f, point3dBad.z);
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
