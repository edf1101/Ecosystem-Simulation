using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class animalController : MonoBehaviour
{
    public timeController TC;

    public float health = 100;
    public float speed = 2;
    public float maxHealth = 100;
    public float hunger = 100;
    public float thirst = 100;
    public float age = 0;

    float hungerDepreciation = 12;
    float thirstDepreciation = 15;

    private float startHours;
    private const float healthDepreciation = 5f;
    bool alive = true;
    animalPath MoveScript;
    public animalSpawner spawner;

    Dictionary<Vector2, int> allSpawns = new Dictionary<Vector2, int>();
    float hungerThresh = 30f;
    float thirstThresh = 25f;
    float lastTime;
    float lastSearched = 0;
    public Vector3 nearestWter;
  //  waterFind tempWF;
    public bool canWater;
 public  bool debug;
    Vector2[] poss;
    float lastThirstQ;
    findWaterRet myWaterPos;
    float lastAnD1;
  public  int canFood;
    public Vector3 nearestFood;
    findWaterRet myFoodPos;
 public    int canWaterr = 0;
    void Start()
    {
        MoveScript = GetComponent<animalPath>();
        startHours = TC.timeHours;
        allSpawns = spawner.allSpawns;
        lastTime = Time.time;
        poss = allSpawns.Keys.ToArray<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (alive && Time.time - lastTime > 0.1f)
        {
            float dT = Time.time - lastTime;
            lastTime = Time.time;
            age = TC.timeHours - startHours;
            hunger -= (dT * TC.speed * hungerDepreciation);
            thirst -= (dT * TC.speed * thirstDepreciation);
            if (thirst < 0)
                thirst = 0;
            if (hunger < 0)
                hunger = 0;
            if (thirst == 0)
                health -= healthDepreciation * dT;
            if (hunger == 0)
                health -= healthDepreciation * dT;
            if (health < 0)
                health = 0;

            //    
            if (!MoveScript.moving && (thirst > thirstThresh || canWaterr == 1) && (hunger > hungerThresh || canFood == 1) && Time.time - lastSearched > 0.2f)
            {
                lastSearched = Time.time;
                Vector2 temp=findRoam();
             //   waterFind tempWF = findWater();
                
                MoveScript.moveTo(new Vector3(temp.x, 0, temp.y),false);
              

            }



            //deal with thirst
            if ( thirst < thirstThresh&&Time.time-lastThirstQ>0.5f &&canWaterr!=3&&canWaterr!=2 && canFood == 0)
            {
                 myWaterPos = findWater1();
                nearestWter = myWaterPos.pos;
                if (nearestWter == Vector3.zero)
                    canWaterr = 1;
                else canWaterr = 2;

            }
            if (!MoveScript.moving && thirst > thirstThresh && Time.time - lastThirstQ > 0.5f && canFood == 0)
            {
                canWaterr =0;

            }
            if (canWaterr == 2&&!MoveScript.moving && canFood == 0)
            {
                MoveScript.moveTo(nearestWter,false);
            }
            if (!MoveScript.moving && canWaterr==2 && Vector2.Distance(new Vector2(transform.position.x,transform.position.z), new Vector2(myWaterPos.pos.x,myWaterPos.pos.z)) < 2f && canFood == 0)
            {
                canWaterr = 3;
                MoveScript.animalDrink(new Vector3(myWaterPos.watPos.x, 0, myWaterPos.watPos.y),false);

            }
            if (thirst > thirstThresh && canWaterr == 4 && Time.time - lastThirstQ > 0.5f)
            {
                canWaterr=0;
            }

            //deal with hunger
            if (hunger < hungerThresh && Time.time - lastThirstQ > 0.5f &&canFood!=3 &&canFood!=2 &&canWaterr==0)
            {
                myFoodPos = findFood();
                nearestFood = myFoodPos.pos;
                if (nearestFood == Vector3.zero)
                    canFood = 1;
                else canFood = 2;

            }
            if (!MoveScript.moving && hunger > hungerThresh && Time.time - lastThirstQ > 0.5f && canWaterr == 0)
            {
                canFood = 0;

            }
            if (canFood == 2 && !MoveScript.moving && canWaterr == 0)
            {
                MoveScript.moveTo(nearestFood, false);
            }
            if (!MoveScript.moving && canFood == 2 && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(myFoodPos.pos.x, myFoodPos.pos.z)) < 2.5f && canWaterr == 0)
            {
                canFood = 3;
                MoveScript.animalDrink(new Vector3(myFoodPos.watPos.x, 0, myFoodPos.watPos.y),true);

            }
            if (hunger > hungerThresh && canFood == 4 && Time.time - lastThirstQ > 0.5f)
            {
                canFood = 0;
            }


        }
    }

    Vector3 findRoam()
    {
        

        Vector2 pos = new Vector2((int)transform.position.x, (int)transform.position.z);
        List<Vector2> roam = new List<Vector2>();
        for (int i = 0; i < poss.Length; i++)
        {
            if (Mathf.Abs(pos.x - poss[i].x) > 5 && Mathf.Abs(pos.y - poss[i].y) > 5 && Mathf.Abs(pos.x - poss[i].x) < 10 && Mathf.Abs(pos.y - poss[i].y) < 10)
                roam.Add(poss[i]);

        }
        return roam[Random.Range(0, roam.Count - 1)];
    }
    findWaterRet findFood()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, 20, LayerMask.GetMask("Marker"), QueryTriggerInteraction.Collide);
        float minDist = 1000;
        Collider pointMin = null;
        
        foreach (Collider coll in colls)
        {
            if (Vector3.Distance(transform.position, coll.transform.position) < minDist && coll.GetComponent<markerController>().type == "Flora")
            {
                minDist = Vector3.Distance(transform.position, coll.transform.position);
                pointMin = coll;
            }
        }
        if (debug)
            print(pointMin.transform.position);
        findWaterRet _findWaterRet;
        if (pointMin == null)
            _findWaterRet.pos = Vector3.zero;
        else
            _findWaterRet.pos = pointMin.transform.position;
        if (pointMin == null)
            _findWaterRet.watPos = Vector3.zero;
        else
            _findWaterRet.watPos = pointMin.GetComponent<markerController>().nearWater;
        return _findWaterRet;
    }
    findWaterRet findWater1()
    {
       Collider[] colls= Physics.OverlapSphere(transform.position, 20, LayerMask.GetMask("Marker"), QueryTriggerInteraction.Collide);
        float minDist = 1000;
        Collider pointMin = null;
        foreach (Collider coll in colls)
        {
            if (Vector3.Distance(transform.position, coll.transform.position) < minDist&& coll.GetComponent<markerController>().type=="Water")
            {
                minDist = Vector3.Distance(transform.position,coll.transform.position);
                pointMin = coll;
            }
        }
       
        findWaterRet _findWaterRet;
        if (pointMin == null)
            _findWaterRet.pos = Vector3.zero;
        else
             _findWaterRet.pos= pointMin.transform.position;
        if (pointMin == null)
            _findWaterRet.watPos = Vector3.zero;
        else
            _findWaterRet.watPos = pointMin.GetComponent<markerController>().nearWater;
        return _findWaterRet;
    }
    struct findWaterRet
    {
        public Vector3 pos;
        public Vector3 watPos;
    }
    void Die()
    {
        alive = false;
    }
    
}
