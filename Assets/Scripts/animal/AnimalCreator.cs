using UnityEngine;


// This class creates each animals characteristics ie is it fast, slow...
public class AnimalCreator : MonoBehaviour
{

    // all the charactersitics that can be changed for each animal
    public string speciesName;
    public float baseSpeed;
    public float age;
    public int foodChain;
    public float minTemp;
    public float maxTemp;
    public float aggression;
    public int editMat;
    public Color baseCol;
    public int generation = 1;
    public int childWait;
    public float startSize;
    public float relativeSize = 1;
    public float faunaMult = 1;
    public float chance1D = 1;
    public float chance2D = 0;
   

    // variables for spawning in animals
    private bool inited = false;
    private Vector3 firstSize;
    private float firstOffset;
    private float firstSpeed;
   
    public GameObject me; // reference to the animal I have created
    public float oldAge=15; // how old should the animal start dying

    // initiates an animal from scratch
    public void initiate()
    {
        // gets reference to my animal controller
        animalController AC = GetComponent<animalController>();

        // assign all the characteristic variables +- a random offset
        // also clamp the offset so values are all sensible

        AC.baseSpeed = Mathf.Clamp(baseSpeed * Random.Range(0.5f, 2f), 0f, 10f);
        AC.age = age;
        AC.foodChain = foodChain;
        AC.minTemp = Mathf.Clamp01(minTemp + Random.Range(-0.1f, 0.1f));
        AC.maxTemp = Mathf.Clamp01(maxTemp  +Random.Range(-0.05f, 0.05f));
        AC.aggression = Mathf.Clamp01((aggression + 0.005f) + Random.Range(-0.2f, 0.2f));
        AC.chanceAtt2D = Mathf.Clamp01(chance2D * Random.Range(0.7f, 1.3f)); 
        AC.chanceAtt1D = Mathf.Clamp01(chance1D * Random.Range(0.7f, 1.3f)); 
        AC.faunaMul = Mathf.Clamp01(faunaMult + Random.Range(-0.2f, 0.2f));
        AC.speciesName = speciesName;
        AC.childWait = childWait;
        AC.male = Random.Range(0, 2) % 2 == 0; // random chance if its male or female

        // Assign colour to the animal
        Color newCol = new Color(Mathf.Clamp01(baseCol.r * Random.Range(0.9f, 1.1f)), Mathf.Clamp01(baseCol.g * Random.Range(0.9f, 1.1f)), Mathf.Clamp01(baseCol.b * Random.Range(0.9f, 1.1f)));
        AC.myColor = newCol;
        Material[] mats = GetComponentInChildren<Renderer>().materials;
        mats[editMat].color = newCol;

        // assisgn myself as agameObject and create my name as species + generation
        me = gameObject;
        gameObject.name = speciesName + " Gen: " + generation.ToString();

        // start the relavent scripts for the animal to get going
        GetComponent<animalController>().myStart();
        GetComponent<glowController>().myStart();
        GetComponent<animalEating>().myStart();

        // make it small so it moves like a baby
        firstSize = transform.localScale;
        firstOffset = GetComponent<animalPath>().heightOffset;
        firstSpeed = AC.baseSpeed;

        // set this true so we know it has begun
        inited = true;
    }


    //called each frame
    private void Update()
    {
        if (inited) // if begun
        {
            // Make it grow larger until it gets to 3 so it ages 
            transform.localScale = firstSize * relativeSize;
            GetComponent<animalPath>().heightOffset = firstOffset * relativeSize;
            float cAge = GetComponent<animalController>().age;
            relativeSize = Mathf.Min((cAge - 0) * (1 - 0.2f) / (2 - 0) + 0.2f, 1);

             // if its smaller than 3 make it a bit slower as well
            if(cAge < 3)
                GetComponent<animalController>().baseSpeed = Mathf.Clamp((cAge - 0) * (firstSpeed - (firstSpeed * 0.3f)) / (3 - 0) + (firstSpeed * 0.3f), 0, firstSpeed);

            // if its in old age make it weak and move slower
            if (cAge > oldAge)
            {
                GetComponent<animalController>().baseSpeed =Mathf.Clamp( (cAge - oldAge) * (0.2f-firstSpeed ) / (25 - oldAge) + firstSpeed,0.2f,firstSpeed);
            }
        }
    }


    //gets called when it wants to have a child
    public void haveChild(GameObject otherParent)
    {
        // create new gameobejct for child thats like my own game object
        GameObject child = Instantiate(me, transform.parent);
        child.transform.position = transform.position;

        // get references to childs and both parents animal creators
        AnimalCreator ACr = child.GetComponent<AnimalCreator>();
        AnimalCreator MCr = GetComponent<AnimalCreator>();
        AnimalCreator OCr = otherParent.GetComponent<AnimalCreator>();

        // increase birth rate for that species
        GetComponent<animalController>().PM.increaseBPS(speciesName);

        
        ACr.age = 0; // reset its age

        // make these characteristcs a average of its parents
        ACr.baseSpeed = (MCr.baseSpeed + OCr.baseSpeed) / 2f;
        ACr.aggression = (MCr.aggression + OCr.aggression) / 2f;
        ACr.aggression = (MCr.maxTemp + OCr.maxTemp) / 2f;
        ACr.minTemp = (MCr.minTemp + OCr.minTemp) / 2f;
        ACr.baseCol = new Color((MCr.baseCol.r+OCr.baseCol.r)/2, (MCr.baseCol.g + OCr.baseCol.g) / 2, (MCr.baseCol.b + OCr.baseCol.b) / 2);

         // reset data from its copied animal controller
        child.GetComponent<animalController>().mateSpot = Vector3.zero;
        child.GetComponent<animalController>().myMate = null;
        child.GetComponent<animalController>().canChild = 0;
        child.GetComponent<animalController>().health = 100;
        child.GetComponent<animalController>().hunger = 100;
        child.GetComponent<animalController>().thirst = 100;
        child.GetComponent<animalPath>().moveCheap = false;
        child.GetComponent<animalPath>().moving = false;

        // update its generation
        ACr.generation = (int)((MCr.generation + OCr.generation) / 2)+1;


        ACr.initiate(); // initialise its animal creator so it has some random offsets
    }
}
