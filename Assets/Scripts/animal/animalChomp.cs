using UnityEngine;

public class animalChomp : MonoBehaviour
{
    // Start is called before the first frame update
    animalController aC;
    float lastEnter;
    [SerializeField] GameObject bloodPrefab;
    public Transform effectsLocation;
    public float bloodOffset;
    bool inside;
    bool started;
    public void myStart()
    {
        aC = GetComponent<animalController>();
        started = true;
    }

    private void OnTriggerStay(Collider other)
    {
        // print("x");
        if (started && other.gameObject == aC.followingWho && Time.time - lastEnter > 1f)
        {
            lastEnter = Time.time;
            GameObject temp = Instantiate(bloodPrefab, effectsLocation);
            temp.transform.position = transform.position + (transform.forward * bloodOffset);
            aC.followingWho.GetComponent<animalController>().health -= 20;
            if (aC.followingWho.GetComponent<animalController>().health < 1)
            {
                //kill
                animalController AC = GetComponent<animalController>();
                aC.PM.increaseKPS(AC.speciesName);

            }
            aC.hunger += 10;
            if (aC.hunger > 100)
                aC.hunger = 100;

        }
    }
}
