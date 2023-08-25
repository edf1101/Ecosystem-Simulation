using UnityEngine;

// This class enables animals to predate and eat other animals
public class animalEating : MonoBehaviour
{
    
    private animalController myAnimalController; // reference to my animalController

    private float lastEnter; // used to have a cooldown when animals can kill each other
    [SerializeField] private GameObject bloodPrefab; // prefab for blood effect

    private Transform effectsLocation; // where the blood should come from in animal

    [SerializeField] private float bloodOffset; // how high above animal should it be

    // set to true after run start there so we dont execute update too early
    private bool started; 


     // called before first frame
    public void myStart()
    {
        myAnimalController = GetComponent<animalController>(); // set animal controller
        started = true;
    }

    // When an animals collider stays inside another colldier
    private void OnTriggerStay(Collider other)
    {
        // if its started and its the animal im following and its been 1s since last run this if statement
        if (started && other.gameObject == myAnimalController.followingWho && Time.time - lastEnter > 1f)
        {
            lastEnter = Time.time;

            // create blood effects
            GameObject temp = Instantiate(bloodPrefab, effectsLocation);
            temp.transform.position = transform.position + (transform.forward * bloodOffset);

            // decrease health by 20
            myAnimalController.followingWho.GetComponent<animalController>().health -= 20;

            // if their health is below 1 they die
            if (myAnimalController.followingWho.GetComponent<animalController>().health < 1)
            {
                //kill
                // update my stats for kills
                myAnimalController.PM.increaseKPS(myAnimalController.speciesName);

            }
            // update my hunger values but clamp it at 100
            myAnimalController.hunger += 10;
            if (myAnimalController.hunger > 100)
                myAnimalController.hunger = 100;

        }
    }

    // setter for privat variable
    public void setEffectsLoc(Transform _loc)
    {
        effectsLocation = _loc;
    }
}
