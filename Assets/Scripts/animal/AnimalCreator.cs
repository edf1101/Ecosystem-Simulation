using UnityEngine;

public class AnimalCreator : MonoBehaviour // this script is used to create animals once game running ie having a child 
{
    // public float maxModifier = 0.1f;
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
   

    bool inited = false;
    Vector3 firstSize;
    float firstOffset;
    float firstSpeed;
   // int Generation = 1;
    // public float currentAge;
    public GameObject me;
    public float oldAge=15;

    public void initiate()
    {
        animalController AC = GetComponent<animalController>();
        AC.baseSpeed = Mathf.Clamp(baseSpeed * Random.Range(0.5f, 2f), 0f, 10f);
        AC.age = age;
        AC.foodChain = foodChain;
        AC.minTemp = Mathf.Clamp01(minTemp + Random.Range(-0.1f, 0.1f));
        AC.maxTemp = Mathf.Clamp01(maxTemp  +Random.Range(-0.05f, 0.05f));
        AC.aggression = Mathf.Clamp01((aggression + 0.005f) + Random.Range(-0.2f, 0.2f));
        AC.chanceAtt2D = Mathf.Clamp01(chance2D * Random.Range(0.7f, 1.3f)); ;
        AC.chanceAtt1D = Mathf.Clamp01(chance1D * Random.Range(0.7f, 1.3f)); ;
        AC.faunaMul = Mathf.Clamp01(faunaMult + Random.Range(-0.2f, 0.2f)); ;
        Color newCol = new Color(Mathf.Clamp01(baseCol.r * Random.Range(0.9f, 1.1f)), Mathf.Clamp01(baseCol.g * Random.Range(0.9f, 1.1f)), Mathf.Clamp01(baseCol.b * Random.Range(0.9f, 1.1f)));
        AC.myColor = newCol;
        Material[] mats = GetComponentInChildren<Renderer>().materials;
        AC.male = Random.Range(0, 2) % 2 == 0;
        AC.speciesName = speciesName;
        AC.childWait = childWait;
        mats[editMat].color = newCol;
        me = gameObject;
        gameObject.name = speciesName + " Gen: " + generation.ToString();
        //  GetComponent<animalPath>().myStart();
        GetComponent<animalController>().myStart();
        GetComponent<glowController>().myStart();
        GetComponent<animalEating>().myStart();
        firstSize = transform.localScale;
        firstOffset = GetComponent<animalPath>().heightOffset;
        firstSpeed = AC.baseSpeed;
        inited = true;
    }

    private void Update()
    {
        if (inited)
        {
            transform.localScale = firstSize * relativeSize;
            GetComponent<animalPath>().heightOffset = firstOffset * relativeSize;
            float cAge = GetComponent<animalController>().age;
            relativeSize = Mathf.Min((cAge - 0) * (1 - 0.2f) / (2 - 0) + 0.2f, 1);
            if(cAge < 3)
                GetComponent<animalController>().baseSpeed = Mathf.Clamp((cAge - 0) * (firstSpeed - (firstSpeed * 0.3f)) / (3 - 0) + (firstSpeed * 0.3f), 0, firstSpeed);
            if (cAge > oldAge)
            {
                GetComponent<animalController>().baseSpeed =Mathf.Clamp( (cAge - oldAge) * (0.2f-firstSpeed ) / (25 - oldAge) + firstSpeed,0.2f,firstSpeed);
            }
        }
    }
    public void haveChild(GameObject otherParent)
    {
        GameObject child = Instantiate(me, transform.parent);
        child.transform.position = transform.position;
        AnimalCreator ACr = child.GetComponent<AnimalCreator>();
        AnimalCreator MCr = GetComponent<AnimalCreator>();
        AnimalCreator OCr = otherParent.GetComponent<AnimalCreator>();
        GetComponent<animalController>().PM.increaseBPS(speciesName);
        ACr.age = 0;
        ACr.baseSpeed = (MCr.baseSpeed + OCr.baseSpeed) / 2f;
        ACr.aggression = (MCr.aggression + OCr.aggression) / 2f;
        ACr.aggression = (MCr.maxTemp + OCr.maxTemp) / 2f;
        ACr.minTemp = (MCr.minTemp + OCr.minTemp) / 2f;
        ACr.baseCol = new Color((MCr.baseCol.r+OCr.baseCol.r)/2, (MCr.baseCol.g + OCr.baseCol.g) / 2, (MCr.baseCol.b + OCr.baseCol.b) / 2);
        child.GetComponent<animalController>().mateSpot = Vector3.zero;
        child.GetComponent<animalController>().myMate = null;
        child.GetComponent<animalController>().canChild = 0;
        child.GetComponent<animalController>().health = 100;
        child.GetComponent<animalController>().hunger = 100;
        child.GetComponent<animalController>().thirst = 100;
        child.GetComponent<animalPath>().moveCheap = false;
        child.GetComponent<animalPath>().moving = false;
        ACr.generation = (int)((MCr.generation + OCr.generation) / 2)+1;

        

        ACr.initiate();
    }
}
