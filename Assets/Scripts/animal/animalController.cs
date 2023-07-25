using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class animalController : MonoBehaviour
{
    public Transform effectsLoc;
    public string speciesName;
    public timeController TC;
    public GameObject myHead;
    public float health = 100;
    public bool male;
    public float baseSpeed = 4;
    public float maxHealth = 100;
    public float hunger = 100;
    public float thirst = 100;
    public float age = 0;
    public int foodChain;
    public float hungerDepreciation = 4;
    public float thirstDepreciation = 5;
    public float minTemp;
    public float maxTemp;
    public Color myColor;
    public int reproductiveMulti = 1;
    public int nearbyPrey;
    [SerializeField] float myTemp;

    private float startHours;
    private const float healthDepreciation = 5f;
    bool alive = true;
    animalPath MoveScript;
    public animalSpawner spawner;

    public Dictionary<Vector2, int> allSpawns = new Dictionary<Vector2, int>();
    float hungerThresh = 30;
    float thirstThresh = 40;
    float lastTime;
    float lastSearched = 0;
    public Vector3 nearestWter;
    //  waterFind tempWF;
    public bool canWater;
    public bool debug;
    public Vector2[] poss;
    float lastThirstQ;
    findWaterRet myWaterPos;
    float lastAnD1;
    public int canFood;
    public Vector3 nearestFood;
    findFoodRet myFoodPos;
    public float speed;

    public int canWaterr = 0;
    float aniBaseSpeed;
    public floraController myFC;
    bool hunted;
    bool canEscape;
    public float lastHung;
    public bool following;
    public GameObject followingWho;
    public float startFollow;
    float lastMoved;
    List<Harrass> harrassList;
    float lastSeen;
    public bool badSpot;
    public List<GameObject> followers;
    Texture2D avails;
    bool canNicerConditions;
    public bool aggressive;
    public biomeGenerator BG;
    Dictionary<Vector2, biomeGenerator.Biome> biomeDict;
    public int happyBiomeIndex;
    public int myBiome;
    public float aggression;
    float smallHuntCool;
    bool started;
    public int childWait;
    public int canChild;
    float lastChild;
    public GameObject myMate;
    public Vector3 mateSpot;
    float lastMateTry;
    public class Harrass
    {
        public float time;
        public GameObject who;
        public Harrass(float _time, GameObject _who)
        {
            time = _time;
            who = _who;
        }
    }
    public GameObject heartPrefab;
    public bool pregnant;
    bool removed;
   public float foundMate;
    float lastOver;
    float lastMoveAttempt;
    public float chanceAtt1D = 1;
    public float chanceAtt2D = 0.2f;
    public float faunaMul = 1;

    public float depSlow = 1f;
    float myChance1;
    public float reproducability = 1;
    public void myStart()
    {
        //baseSpeed = speed;
        harrassList = new List<Harrass>();
        aniBaseSpeed = GetComponent<Animator>().speed;
        MoveScript = GetComponent<animalPath>();
        startHours = TC.timeHours;
        allSpawns = spawner.allSpawns;
        lastTime = Time.time;
        poss = allSpawns.Keys.ToArray<Vector2>();
        avails = spawner.nonColouredAvs;
        biomeDict = BG.biomeDict;
        PM = GameObject.Find("Animals").GetComponent<populationManager>();
        started = true;

    }
    public populationManager PM;

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            myChance1 = PM.Getchance1(speciesName);
            if (alive && Time.time - lastTime > 0.1f)
            {
                if (followingWho == null && following)
                    following = false;
                // BASE CHANGES
                if (following)
                    speed = (followingWho.GetComponent<animalController>().speed * TC.multiplier) * 0.9f;
                else
                    speed = baseSpeed * TC.multiplier;

                GetComponent<Animator>().speed = aniBaseSpeed * TC.multiplier;
                float dT = Time.time - lastTime;
                lastTime = Time.time;
                age = TC.timeHours - startHours + GetComponent<AnimalCreator>().age;
                hunger -= (dT * TC.speed * TC.multiplier * hungerDepreciation * depSlow);
                thirst -= (dT * TC.speed * TC.multiplier * thirstDepreciation * depSlow);
                if (thirst < 0)
                    thirst = 0;
                if (hunger < 0)
                    hunger = 0;
                if (thirst == 0)
                    health -= healthDepreciation * dT;
                if (hunger == 0)
                    health -= healthDepreciation * dT;
                if (age>GetComponent<AnimalCreator>().oldAge)
                    health -= 5 * dT;
                if (health < 0)
                    health = 0;
               /* if (!(myTemp > minTemp && myTemp < maxTemp))
                {
                    health -= 3 * dT;
                }*/

                    hunted = GetComponent<animalSight>().predators.Length != 0;
                myTemp = spawner.WM.comTemp.GetPixel((int)transform.position.x / 10, (int)transform.position.z / 10).r;

                myBiome = biomeDict[new Vector2((int)transform.position.x, (int)transform.position.z)].Index;



                //  MOVE IDLY
                if (health > 0 && !MoveScript.moving && (((thirst > thirstThresh || canWaterr == 1) && (hunger > hungerThresh || canFood == 1))) && Time.time - lastSearched > 0.2f && !following && !badSpot && canChild!=2)
                {
                    lastSearched = Time.time;
                    Vector3 temp = new Vector3();
                    nearbyPrey = GetComponent<animalSight>().getPrey().Length;
                     if (!(myTemp > minTemp && myTemp < maxTemp))
                     {
                         //get nearest nice point
                         List<Vector2> points = new List<Vector2>();
                         Texture2D tempMap = spawner.WM.comTemp;
                         for (int y = Mathf.Max(0, (int)transform.position.z - 30); y < Mathf.Min(250, transform.position.z + 30); y++)
                         {

                             for (int x = Mathf.Max(0, (int)transform.position.x - 30); x < Mathf.Min(250, transform.position.x + 30); x++)
                             {
                                 float tempTemp = tempMap.GetPixel(x / 10, y / 10).r;

                                 if (avails.GetPixel(x, y) != Color.black && !(tempTemp < minTemp || tempTemp > maxTemp))
                                 {
                                     points.Add(new Vector2(x, y));
                                 }

                             }

                         }


                         // Debug.Log(points.Count,gameObject);
                         canNicerConditions = true;
                         if (points.Count > 0)
                         {
                             temp = points[Random.Range(0, points.Count)];
                             if (debug)
                                 print("unhappy");
                         }
                         else
                             canNicerConditions = false;

                     }

                    if (((myTemp > minTemp && myTemp < maxTemp) || !canNicerConditions)&& canChild!=2)
                    //if ( canChild!=2)
                    {
                        temp = findRoam();
                        if(debug)
                        print("roam");
                    }
                  

                    if (debug)
                        print("move idle");
                    badSpot = true;
                    MoveScript.moveTo(new Vector3(temp.x, 0, temp.y), 0);
                    lastMoveAttempt = Time.time;


                }
                if (badSpot && !( canChild==2)&&Time.time-lastMoveAttempt>1.5f)
                {
                    lastMoveAttempt = Time.time;
                    if (debug)
                        Debug.Log("try", gameObject);

                   // bool done = false;
                    List<Vector3> poss = new List<Vector3>();
                    for (int x = Mathf.Max(20, (int)transform.position.x - 10); x < Mathf.Min(230, transform.position.x + 10); x++)
                    {

                        for (int y = Mathf.Max(20, (int)transform.position.z - 10); y < Mathf.Min(230, transform.position.z +10); y++)
                        {


                            if (avails.GetPixel(x, y) != Color.black && spawner.WM.comTemp.GetPixel(x / 10, y / 10).r<maxTemp && spawner.WM.comTemp.GetPixel(x / 10, y / 10).r > minTemp )
                            {
                                
                                if (debug)
                                    print("move fix");
                                poss.Add(new Vector3(x, 0, y));

                            }


                        }
                       
                    }
                    
                    if (poss.Count==0)
                    {
                        for (int x = Mathf.Max(20, (int)transform.position.x - 7); x < Mathf.Min(230, transform.position.x + 7); x++)
                        {

                            for (int y = Mathf.Max(20, (int)transform.position.z - 7); y < Mathf.Min(230, transform.position.z + 7); y++)
                            {


                                if (avails.GetPixel(x, y) != Color.black)
                                {
                                    poss.Add(new Vector3(x, 0, y));
                                    

                                }


                            }
                        
                        }
                    }
                    if (poss.Count != 0)
                    {
                        MoveScript.moveTo(poss[Random.Range(0, poss.Count - 1)], 1);
                        badSpot = false;
                    }
                    else
                    {
                        Debug.Log("bad",gameObject);
                    }

                }
                /*
                // move to nicer conditions
                if((myTemp<minTemp || myTemp>maxTemp) && !MoveScript.moving)
                {





                }
                */


                //hunting
                if (canChild==0 &&GetComponent<animalSight>().prey.Length > 0 && (((thirst > thirstThresh || canWaterr == 1) && (hunger > hungerThresh || canFood == 1))) && !following && aggression != 0 && Time.time - smallHuntCool > 2f&&hunger<92)
                {
                    //get victim
                    List<Harrass> temp = new List<Harrass>();
                    foreach (Harrass h in harrassList)
                    {
                        if (Time.time - h.time < 15) // 10s follow time + 10s cooldown
                        {
                            temp.Add(h);
                        }
                    }
                    harrassList = temp;
                    GameObject minPrey = null;
                    float minDis = 1000;

                    foreach (GameObject p in GetComponent<animalSight>().getPrey())
                    {
                        float pTemp = spawner.WM.comTemp.GetPixel((int)p.transform.position.x / 10, (int)p.transform.position.z / 10).r;
                        if (Vector3.Distance(p.transform.position, transform.position) < minDis && !haveIHarrassedThem(p) && pTemp < maxTemp && pTemp > minTemp)
                        {
                            minDis = Vector3.Distance(p.transform.position, transform.position);
                            minPrey = p;
                        }
                    }
                    if (minDis != 1000)
                    {
                        smallHuntCool = Time.time;
                        harrassList.Add(new Harrass(Time.time, followingWho));
                        if (Random.value < aggression)
                        {

                            followingWho = minPrey;
                            following = true;

                            followingWho.GetComponent<animalController>().followers.Add(gameObject);
                            startFollow = Time.time;
                        }

                    }



                }
                if ((((thirst > thirstThresh || canWaterr == 1) && (hunger > hungerThresh || canFood == 1))) && following && Time.time - lastMoved > 0.1f && canChild == 0)
                {
                    //follow victim
                    lastMoved = Time.time;
                    if (followingWho != null)
                        MoveScript.moveTo(new Vector3(followingWho.transform.position.x, 0, followingWho.transform.position.z), 1);
                    else
                        following = false;
                    if (debug)
                        print("move hunt");



                }
                if (following && canChild == 0 && (Time.time - startFollow > 10f) || !((((thirst > thirstThresh || canWaterr == 1) && (hunger > hungerThresh || canFood == 1)))))
                {
                    following = false;
                    if (followingWho != null)
                        followingWho.GetComponent<animalController>().followers.Remove(gameObject);
                    followingWho = null;



                }

                // reproducing
                if(canChild==0 && canFood==0 && canWaterr == 0 && age - lastChild > childWait)
                {
                    canChild = 1;
                }

                
                if (canChild == 1&&male && Time.time - lastMateTry > 1f)
                {
                    lastMateTry = Time.time;
               
                    GameObject[] mates = GetComponent<animalSight>().getMates();
                    if (mates.Length > 0)
                    {
                        myMate = mates[Random.Range(0, mates.Length - 1)];
                        myMate.GetComponent<animalController>().myMate = gameObject;
                        canChild = 2;

                        mateSpot = myMate.transform.position;
                        MoveScript.moveTo(new Vector3(mateSpot.x,0,mateSpot.z), 1);
                        myMate.GetComponent<animalController>().canChild = 2;
                        myMate.GetComponent<animalController>().mateSpot = mateSpot;
                        foundMate = Time.time;
                        myMate.GetComponent<animalController>().foundMate = Time.time;


                    }
                    else
                    {
                        canChild = 0;
                    }
                }
                if ( canChild == 2&&(Time.time - foundMate > 10  || myMate==null))
                {
                    canChild = 0;
                }
                /*if (myMate == null)
                {
                    myMate = null;
                    canChild = 0;

                }*/
                if (canChild == 2&&male)
                {

                    mateSpot = myMate.transform.position;
                    MoveScript.moveTo(new Vector3(mateSpot.x, 0, mateSpot.z), 1);
                    if (male)
                    {
                        if (Vector3.Distance(transform.position, myMate.transform.position) < 0.5f&&!removed)
                        {
                            removed = true;
                            StartCoroutine(removeMate());

                            if (Random.value < reproducability)
                            {
                                if (Random.value < PM.getchance4(speciesName))
                                {
                                    GetComponent<AnimalCreator>().haveChild(myMate);
                                    GetComponent<AnimalCreator>().haveChild(myMate);
                                    GetComponent<AnimalCreator>().haveChild(myMate);
                                    GetComponent<AnimalCreator>().haveChild(myMate);
                                }
                                else if (Random.value < PM.getchance3(speciesName))
                                {
                                    GetComponent<AnimalCreator>().haveChild(myMate);
                                    GetComponent<AnimalCreator>().haveChild(myMate);
                                    GetComponent<AnimalCreator>().haveChild(myMate);

                                }
                                else if (Random.value < PM.getchance2(speciesName))
                                {
                                    for (int i = 0; i <= reproductiveMulti; i++)
                                    {
                                        GetComponent<AnimalCreator>().haveChild(myMate);
                                        GetComponent<AnimalCreator>().haveChild(myMate);
                                    }
                                }
                                else if (Random.value < PM.Getchance1(speciesName))
                                {
                                    for (int i = 0; i <= reproductiveMulti; i++)
                                    {
                                        GetComponent<AnimalCreator>().haveChild(myMate);
                                    }
                                }
                            }
                            GameObject temp = Instantiate(heartPrefab);
                            temp.transform.parent = effectsLoc;
                            temp.transform.position = transform.position+new Vector3(0,1,0);




                        }
                    }
                }
                

                //                   deal with thirst
                if (thirst < thirstThresh && canChild == 0  && canWaterr != 3 && canWaterr != 2 && canFood == 0)
                {
                    myWaterPos = findWater1();
                    nearestWter = myWaterPos.pos;
                    if (nearestWter == Vector3.zero)
                        canWaterr = 1;
                    else canWaterr = 2;

                }
                if (!MoveScript.moving && canChild == 0 && thirst > thirstThresh  && canFood == 0)
                {
                    canWaterr = 0;

                }
                if (canWaterr == 2 && canChild == 0 && !MoveScript.moving && canFood == 0 && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(myWaterPos.pos.x, myWaterPos.pos.z)) > 2.5f)
                {
                    MoveScript.moveTo(nearestWter, 1);
                    if (debug)
                        print("move water");
                }
                if (!MoveScript.moving && canChild == 0 && canWaterr == 2 && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(myWaterPos.pos.x, myWaterPos.pos.z)) < 2.5f && canFood == 0)
                {
                    canWaterr = 3;
                    MoveScript.animalDrink(new Vector3(myWaterPos.watPos.x, 0, myWaterPos.watPos.y), false);

                }
                if (thirst > thirstThresh && canChild == 0 && canWaterr == 4 && Time.time - lastThirstQ > 0.5f)
                {
                    canWaterr = 0;
                }




                //              deal with hunger
                if (hunger < hungerThresh && canChild == 0 &&  canFood != 3 && canFood != 2 && canWaterr == 0)
                {
                    myFoodPos = findFood();
                    nearestFood = myFoodPos.pos;
                    if (nearestFood == Vector3.zero)
                        canFood = 1;
                    else
                    {
                        canFood = 2;
                        myFoodPos.FC.owner = true;
                        myFC = myFoodPos.FC;
                    }

                }
                if (!MoveScript.moving && canChild == 0 && hunger > hungerThresh && canFood == 4 && canWaterr == 0)
                {
                    canFood = 0;
                    myFoodPos.FC.Eat();
                }
                if (canFood == 2 && canChild == 0 && !MoveScript.moving && canWaterr == 0 && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(myFoodPos.pos.x, myFoodPos.pos.z)) > 2.5f)
                {
                    MoveScript.moveTo(nearestFood, 1);
                    if (debug)
                        print("move eat");
                }
                if (!MoveScript.moving && canChild == 0 && canChild == 0 && canFood == 2 && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(myFoodPos.pos.x, myFoodPos.pos.z)) < 2.5f && canWaterr == 0)
                {
                    canFood = 3;
                    MoveScript.animalDrink(new Vector3(myFoodPos.watPos.x, 0, myFoodPos.watPos.y), true);

                }
                if (hunger > hungerThresh && canChild == 0 && canFood == 4 && Time.time - lastThirstQ > 0.5f)
                {
                    canFood = 0;

                }


                /*
                //overpopulation/*
                if (Time.time - lastOver > 2 && canChild == 0)
                {
                    lastOver = Time.time;
                    Collider[] collisions = Physics.OverlapSphere(transform.position, 3);
                    int amount = 0;
                    foreach (Collider col in collisions)
                    {
                        if (col.GetComponentInParent<animalController>() != null)// if it is an animal
                        {

                            amount++;


                        }

                    }
                    if (amount > 6)
                    {
                        Vector3 temp = findFar();
                        if (temp!=Vector3.zero)
                         MoveScript.moveTo(new Vector3(temp.x,0,temp.y), 1);
                    }

                }*/
            }


            //DIE IF ALIVE
            if (health < 1)
            {
                Die();

            }
        }
    }


    IEnumerator removeMate()
    {
        yield return new WaitForSeconds(2);

        if (myMate.GetComponent<animalController>() != null)
        {
            myMate.GetComponent<animalController>().canChild = 0;
            lastChild = age;
            myMate.GetComponent<animalController>().lastChild = myMate.GetComponent<animalController>().age;
            canChild = 0;
            myMate.GetComponent<animalController>().myMate = null;
            myMate = null;
            removed = false;
        }
        //  return null;

    }
    void lostTarget()
    {
        following = false;

        followingWho = null;

        GetComponent<animalSight>().UpdatePrey();
    }
    bool haveIHarrassedThem(GameObject claimer)
    {
        foreach (Harrass h in harrassList)
        {
            if (h.who == claimer)
                return true;

        }
        return false;
    }
    Vector3 findRoam()
    {


        Vector2 pos = new Vector2((int)transform.position.x, (int)transform.position.z);
        List<Vector2> roam = new List<Vector2>();
        for (int i = 0; i < poss.Length; i++)
        {
            if (Mathf.Abs(pos.x - poss[i].x) > 5 && Mathf.Abs(pos.y - poss[i].y) > 5 && Mathf.Abs(pos.x - poss[i].x) < 15 && Mathf.Abs(pos.y - poss[i].y) < 20)
                roam.Add(poss[i]);

        }
        if (roam.Count > 0)
            return roam[Random.Range(0, roam.Count - 1)];
        else
            return Vector3.zero;
    }
    Vector3 findFar()
    {


        Vector2 pos = new Vector2((int)transform.position.x, (int)transform.position.z);
        List<Vector2> roam = new List<Vector2>();
        for (int i = 0; i < poss.Length; i++)
        {
            if (Mathf.Abs(pos.x - poss[i].x) > 5 && Mathf.Abs(pos.y - poss[i].y) > 5 && Mathf.Abs(pos.x - poss[i].x) < 15 && Mathf.Abs(pos.y - poss[i].y) < 20)
                roam.Add(poss[i]);

        }
        if (roam.Count > 0)
            return roam[Random.Range(0, roam.Count - 1)];
        else
            return Vector3.zero;
    }



    findFoodRet findFood()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, 20, LayerMask.GetMask("Marker"), QueryTriggerInteraction.Collide);
        float minDist = 1000;
        Collider pointMin = null;
        floraController FCmin = null;
        foreach (Collider coll in colls)
        {
            if (Vector3.Distance(transform.position, coll.transform.position) < minDist && coll.GetComponent<markerController>().type == "Flora" && !coll.GetComponent<markerController>().FC.owner && coll.GetComponent<markerController>().FC.eatable)
            {
                minDist = Vector3.Distance(transform.position, coll.transform.position);
                pointMin = coll;
                FCmin = coll.GetComponent<markerController>().FC;
            }
        }
        if (debug)
            print(pointMin.transform.position);
        findFoodRet _findWaterRet = new findFoodRet();
        if (pointMin == null)
            _findWaterRet.pos = Vector3.zero;
        else
            _findWaterRet.pos = pointMin.transform.position;
        if (pointMin == null)
            _findWaterRet.watPos = Vector3.zero;
        else
            _findWaterRet.watPos = pointMin.GetComponent<markerController>().nearWater;

        if (pointMin == null)
            _findWaterRet.FC = null;
        else
            _findWaterRet.FC = FCmin;
        return _findWaterRet;
    }
    findWaterRet findWater1()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, 20, LayerMask.GetMask("Marker"), QueryTriggerInteraction.Collide);
        float minDist = 1000;
        Collider pointMin = null;
        foreach (Collider coll in colls)
        {
            if (Vector3.Distance(transform.position, coll.transform.position) < minDist && coll.GetComponent<markerController>().type == "Water")
            {
                minDist = Vector3.Distance(transform.position, coll.transform.position);
                pointMin = coll;
            }
        }

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
    struct findWaterRet
    {
        public Vector3 pos;
        public Vector3 watPos;
    }
    struct findFoodRet
    {
        public Vector3 pos;
        public Vector3 watPos;
        public floraController FC;
    }
    void Die()
    {
        alive = false;
        foreach (GameObject follower in followers)
        {
            if (follower != null)
                follower.GetComponent<animalController>().lostTarget();
        }
        GetComponent<Animator>().speed = aniBaseSpeed * TC.multiplier;
        PM.increaseDPS(speciesName);
        Destroy(gameObject);

        //   StartCoroutine("fullyDie");
    }

}

