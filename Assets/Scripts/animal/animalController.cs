using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This is the brain for each animal it controls when it eats drinks
// hunts reproduces dies etc..

public class animalController : MonoBehaviour
{

    // Transforms / locations
    public Transform effectsLoc; // where in heirachy to put effects
    public GameObject myHead;



    // script references
    public timeController TC;
    public animalSpawner spawner;
    public floraController myFC;
    public biomeGenerator BG;
    public populationManager PM;
    private animalPath MoveScript;

    // Animal stats

    public string speciesName; 
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
    private float hungerThresh = 30;
    private float thirstThresh = 40;
    private const float healthDepreciation = 5f;// health depreciation if starving
    private bool alive = true; // am I alive
    public float aggression;
    public float chanceAtt1D = 1;
    public float chanceAtt2D = 0.2f;
    public float faunaMul = 1;
    public float depSlow = 1f;
    public float reproducability = 1;
    public float myChance1;
    public bool aggressive;
    public float speed;


    //Hunting varaibles
    public int nearbyPrey; // nearby prey number
    private bool hunted;
    private bool canEscape;
    public float lastHung;
    public bool following;
    public GameObject followingWho;
    public float startFollow;
    private float lastMoved;
    private List<Harrass> harrassList;
    private float lastSeen;
    public List<GameObject> followers;
    private float smallHuntCool;

    //Spawning
    private float startHours; // time when spawned in
    public Dictionary<Vector2, int> allSpawns = new Dictionary<Vector2, int>();



    // Moving
    [SerializeField] private float myTemp; // current temp so I know wehther to leave biome
    private float lastTime;
    private float aniBaseSpeed;
    public bool badSpot;
    private Texture2D avails;
    private bool canNicerConditions;
    private Dictionary<Vector2, biomeGenerator.Biome> biomeDict;
    public int happyBiomeIndex;
    public int myBiome;
    private float lastMoveAttempt;

    // food and drink
    private float lastSearched = 0;
    public Vector3 nearestWter;
    public bool canWater;
    public Vector2[] poss;
    private float lastThirstQ;
    private findWaterRet myWaterPos;
    private float lastAnD1;
    public int canFood;
    public Vector3 nearestFood;
    private findFoodRet myFoodPos;
    public int canWaterr = 0;

    //Reproduction
    private bool started;
    public int childWait;
    public int canChild;
    private float lastChild;
    public GameObject myMate;
    public Vector3 mateSpot;
    private float lastMateTry;
    public GameObject heartPrefab;
    public bool pregnant;
    private bool removed;
    public float foundMate;


    // Other
    public bool debug;


    // Gets called before first frame to set up animal
    public void myStart()
    {
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
    

    // Update is called once per frame
    void Update()
    {
        if (started) // if the animal has been set up
        {

            myChance1 = PM.Getchance1(speciesName);


            if (alive && Time.time - lastTime > 0.1f) // if its alive
            {
                // if not follwoing anyone or they died remove following variable
                if (followingWho == null && following)
                    following = false;

                // if following someone cut down my speed a bit so its more fair
                if (following)
                    speed = (followingWho.GetComponent<animalController>().speed * TC.multiplier) * 0.9f;
                else
                    speed = baseSpeed * TC.multiplier; // otherwise normal speed

                // update animation speed according to my speed
                GetComponent<Animator>().speed = aniBaseSpeed * TC.multiplier;


                float dT = Time.time - lastTime; // my version of time.deltaTime
                lastTime = Time.time;

                // increase age, decrease thirst and hunger
                age = TC.timeHours - startHours + GetComponent<AnimalCreator>().age;
                hunger -= (dT * TC.speed * TC.multiplier * hungerDepreciation * depSlow);
                thirst -= (dT * TC.speed * TC.multiplier * thirstDepreciation * depSlow);

                // clamp thirst, hunger andhealth and do starving/ thirsty health depreciations
                // if values too low
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

                // set whether Im being hunted or not
                hunted = GetComponent<animalSight>().predators.Length != 0;

                // current temperature 
                myTemp = spawner.WM.comTemp.GetPixel((int)transform.position.x / 10, (int)transform.position.z / 10).r;
                // current biome
                myBiome = biomeDict[new Vector2((int)transform.position.x, (int)transform.position.z)].Index;


                // call all the required update scripts
                movementUpdate();
                huntingUpdate();
                reproductionUpdate();
                thirstHungerUpdate();

            }


            // if the animal has no health left kill it off
            if (health < 1)
            {
                Die();

            }
        }
    }

    //  deal with thirst
    private void thirstHungerUpdate()
    {

        // if below thirst threshold go and look for more water
        if (thirst < thirstThresh && canChild == 0 && canWaterr != 3 && canWaterr != 2 && canFood == 0)
        {
            myWaterPos = findWater1();
            nearestWter = myWaterPos.pos;
            if (nearestWter == Vector3.zero)
                canWaterr = 1;
            else canWaterr = 2;

        }

        // if not moving and not thirsty stop lookign for water
        if (!MoveScript.moving && canChild == 0 && thirst > thirstThresh && canFood == 0)
        {
            canWaterr = 0;

        }

        // If found water source then move towards it
        if (canWaterr == 2 && canChild == 0 && !MoveScript.moving && canFood == 0 && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(myWaterPos.pos.x, myWaterPos.pos.z)) > 2.5f)
        {
            MoveScript.moveTo(nearestWter, 1);
            if (debug)
                print("move water");
        }

        // If I am at the water source call the animal drink command 
        if (!MoveScript.moving && canChild == 0 && canWaterr == 2 && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(myWaterPos.pos.x, myWaterPos.pos.z)) < 2.5f && canFood == 0)
        {
            canWaterr = 3;
            MoveScript.animalDrink(new Vector3(myWaterPos.watPos.x, 0, myWaterPos.watPos.y), false);

        }
        // if not thirsty stop looking for water
        if (thirst > thirstThresh && canChild == 0 && canWaterr == 4 && Time.time - lastThirstQ > 0.5f)
        {
            canWaterr = 0;
        }




        // Basically the same but dealing with hunger instead


        // if hungry and not doing anythign else go and search for food
        if (hunger < hungerThresh && canChild == 0 && canFood != 3 && canFood != 2 && canWaterr == 0)
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

        // if not moving and not hungry then stop looking for food
        if (!MoveScript.moving && canChild == 0 && hunger > hungerThresh && canFood == 4 && canWaterr == 0)
        {
            canFood = 0;
            myFoodPos.FC.Eat();
        }
        // if foudn location of food go to it
        if (canFood == 2 && canChild == 0 && !MoveScript.moving && canWaterr == 0 && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(myFoodPos.pos.x, myFoodPos.pos.z)) > 2.5f)
        {
            MoveScript.moveTo(nearestFood, 1);
            if (debug)
                print("move eat");
        }
        // If at the food location then eat it
        if (!MoveScript.moving && canChild == 0 && canChild == 0 && canFood == 2 && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(myFoodPos.pos.x, myFoodPos.pos.z)) < 2.5f && canWaterr == 0)
        {
            canFood = 3;
            MoveScript.animalDrink(new Vector3(myFoodPos.watPos.x, 0, myFoodPos.watPos.y), true);

        }
        // if not moving and not hungry then stop looking for food
        if (hunger > hungerThresh && canChild == 0 && canFood == 4 && Time.time - lastThirstQ > 0.5f)
        {
            canFood = 0;

        }
    }

    // update for reprocucition
    private void reproductionUpdate()
    {

        // decides whether I can have a child or not
        if (canChild == 0 && canFood == 0 && canWaterr == 0 && age - lastChild > childWait)
        {
            canChild = 1;
        }

        // If i can have a child & male and havent tried in the last second
        if (canChild == 1 && male && Time.time - lastMateTry > 1f)
        {
            lastMateTry = Time.time;

            //find nearby mates
            GameObject[] mates = GetComponent<animalSight>().getMates();
            if (mates.Length > 0)
            {
                // choose a random mate nearby
                myMate = mates[Random.Range(0, mates.Length - 1)];

                myMate.GetComponent<animalController>().myMate = gameObject;
                canChild = 2;

                // move towards the mate
                mateSpot = myMate.transform.position;
                MoveScript.moveTo(new Vector3(mateSpot.x, 0, mateSpot.z), 1);
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
        // if my mate has died or cant find them then stop
        if (canChild == 2 && (Time.time - foundMate > 10 || myMate == null))
        {
            canChild = 0;
        }

        if (canChild == 2 && male) // if i have foudn the mate
        {

            mateSpot = myMate.transform.position;
            MoveScript.moveTo(new Vector3(mateSpot.x, 0, mateSpot.z), 1);

            if (male)
            {
                // when Im close enough
                if (Vector3.Distance(transform.position, myMate.transform.position) < 0.5f && !removed)
                {
                    removed = true;
                    StartCoroutine(removeMate()); // get rid of the mate variable

                    if (Random.value < reproducability) // if birth is a success
                    {

                        // What are the chances of having 4 childs, 3 childs etc...
                        // then spawn those children
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

                    // create heart particle effect
                    GameObject temp = Instantiate(heartPrefab);
                    temp.transform.parent = effectsLoc;
                    temp.transform.position = transform.position + new Vector3(0, 1, 0);




                }
            }
        }
    }

    // update for hunting
    private void huntingUpdate()
    {
        //hunting
        if (canChild == 0 && GetComponent<animalSight>().prey.Length > 0 && (((thirst > thirstThresh || canWaterr == 1) && (hunger > hungerThresh || canFood == 1))) && !following && aggression != 0 && Time.time - smallHuntCool > 2f && hunger < 92)
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
            // find the closest prey thats in a accessible biome to me and I havent hunted recently
            foreach (GameObject p in GetComponent<animalSight>().getPrey())
            {
                float pTemp = spawner.WM.comTemp.GetPixel((int)p.transform.position.x / 10, (int)p.transform.position.z / 10).r;
                if (Vector3.Distance(p.transform.position, transform.position) < minDis && !haveIHarrassedThem(p) && pTemp < maxTemp && pTemp > minTemp)
                {
                    minDis = Vector3.Distance(p.transform.position, transform.position);
                    minPrey = p;
                }
            }

            
            if (minDis != 1000)// If i have found an acceptable prey
            {
                smallHuntCool = Time.time;
                harrassList.Add(new Harrass(Time.time, followingWho));
                if (Random.value < aggression) // random chance I dont attack
                {
                     // set them as the prey!
                    followingWho = minPrey;
                    following = true;

                    followingWho.GetComponent<animalController>().followers.Add(gameObject);
                    startFollow = Time.time;
                }

            }
        }

        // If im not thirst hungry etc then start following my victim
        if ((((thirst > thirstThresh || canWaterr == 1) && (hunger > hungerThresh || canFood == 1))) && following && Time.time - lastMoved > 0.1f && canChild == 0)
        {
            //follow victim
            lastMoved = Time.time;

            //  If I do have a target to follow then move to them
            if (followingWho != null)
                MoveScript.moveTo(new Vector3(followingWho.transform.position.x, 0, followingWho.transform.position.z), 1);
            else
                following = false;
            if (debug)
                print("move hunt");



        }

        // if its been 10s since I started following them then give up 
        if (following && canChild == 0 && (Time.time - startFollow > 10f) || !((((thirst > thirstThresh || canWaterr == 1) && (hunger > hungerThresh || canFood == 1)))))
        {
            following = false;
            if (followingWho != null)
                followingWho.GetComponent<animalController>().followers.Remove(gameObject);
            followingWho = null;



        }
    }

    // update moving
    private void movementUpdate() {


        //  MOVE IDLY

        // If im not hungry thirsty etc then move idly
        if (health > 0 && !MoveScript.moving && (((thirst > thirstThresh || canWaterr == 1) && (hunger > hungerThresh || canFood == 1))) && Time.time - lastSearched > 0.2f && !following && !badSpot && canChild != 2)
        {
            lastSearched = Time.time;

            Vector3 temp = new Vector3();
            // calculate nearby prey
            nearbyPrey = GetComponent<animalSight>().getPrey().Length;

            // if im in a point thats not a good temperature for me then find a better space
            if (!(myTemp > minTemp && myTemp < maxTemp))
            {
                //get nearest nice point
                List<Vector2> points = new List<Vector2>();
                Texture2D tempMap = spawner.WM.comTemp;

                // go through points in radus 30 to find somewhere that is a good location
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

                // if I cant find anywhere to move thats a good location take note
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

            // if its in a good temperateure then find a position to move to
            if (((myTemp > minTemp && myTemp < maxTemp) || !canNicerConditions) && canChild != 2)

            {
                temp = findRoam();
                if (debug)
                    print("roam");
            }

            //debug notice
            if (debug)
                print("move idle");


            badSpot = true;
            MoveScript.moveTo(new Vector3(temp.x, 0, temp.y), 0);
            lastMoveAttempt = Time.time;


        }

        if (badSpot && !(canChild == 2) && Time.time - lastMoveAttempt > 1.5f)
        {
            lastMoveAttempt = Time.time;
            if (debug)
                Debug.Log("try", gameObject);


            // if im in a bad lcoation (not A* pathfindable then find somewhere within 10m radius
            // that is a good location and do cheap pathfinding to get there
            List<Vector3> poss = new List<Vector3>();
            for (int x = Mathf.Max(20, (int)transform.position.x - 10); x < Mathf.Min(230, transform.position.x + 10); x++)
            {

                for (int y = Mathf.Max(20, (int)transform.position.z - 10); y < Mathf.Min(230, transform.position.z + 10); y++)
                {


                    if (avails.GetPixel(x, y) != Color.black && spawner.WM.comTemp.GetPixel(x / 10, y / 10).r < maxTemp && spawner.WM.comTemp.GetPixel(x / 10, y / 10).r > minTemp)
                    {

                        if (debug)
                            print("move fix");
                        poss.Add(new Vector3(x, 0, y));

                    }


                }

            }

            // if couldnt find any A* pathfindable points then expand radius
            if (poss.Count == 0)
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
            // move ot that point
            if (poss.Count != 0)
            {
                MoveScript.moveTo(poss[Random.Range(0, poss.Count - 1)], 1);
                badSpot = false;
            }
            else
            {
                Debug.Log("bad", gameObject);
            }

        }


    }


    // wait for 2s then remove mate
    private IEnumerator removeMate()
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

    }
    // when a target gets lost
    private void lostTarget()
    {
        following = false;

        followingWho = null;

        GetComponent<animalSight>().UpdatePrey();
    }

    // have I hunted an animal yet?
    private bool haveIHarrassedThem(GameObject claimer)
    {
        foreach (Harrass h in harrassList)
        {
            if (h.who == claimer)
                return true;

        }
        return false;
    }

    // find nearby places to roam
    private Vector3 findRoam()
    {

        Vector2 pos = new Vector2((int)transform.position.x, (int)transform.position.z);
        List<Vector2> roam = new List<Vector2>();

        // go thorugh all postitons in a 5m range
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


    // find far away places to roam
    private Vector3 findFar()
    {


        Vector2 pos = new Vector2((int)transform.position.x, (int)transform.position.z);
        List<Vector2> roam = new List<Vector2>();
        // go through all locations as long as their in a 5m to 15m range

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


    // find nearby food locations
    private findFoodRet findFood()
    {
        //get all colliders in 20m radius
        Collider[] colls = Physics.OverlapSphere(transform.position, 20, LayerMask.GetMask("Marker"), QueryTriggerInteraction.Collide);
        float minDist = 1000;
        Collider pointMin = null;
        floraController FCmin = null;

        // find closest one 
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

        //error checkign if foudn none etc

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



    // find water locartions
    findWaterRet findWater1()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, 20, LayerMask.GetMask("Marker"), QueryTriggerInteraction.Collide);
        // get all colliders in 20m radius

        float minDist = 1000;
        Collider pointMin = null;

        // find the closest one
        foreach (Collider coll in colls)
        {
            if (Vector3.Distance(transform.position, coll.transform.position) < minDist && coll.GetComponent<markerController>().type == "Water")
            {
                minDist = Vector3.Distance(transform.position, coll.transform.position);
                pointMin = coll;
            }
        }

        // error checking if cant find any etc
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
    

    private void Die() // gets called when the animal dies
    {
        alive = false;
        foreach (GameObject follower in followers)
        {
            // if someone is following the animal let it know to stop following
            if (follower != null)
                follower.GetComponent<animalController>().lostTarget();
        }
        GetComponent<Animator>().speed = aniBaseSpeed * TC.multiplier;
        
        PM.increaseDPS(speciesName);// increase death rate for species
        Destroy(gameObject);
    }

    private struct findWaterRet
    {
        public Vector3 pos;
        public Vector3 watPos;
    }
    private struct findFoodRet
    {
        public Vector3 pos;
        public Vector3 watPos;
        public floraController FC;
    }
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

}

