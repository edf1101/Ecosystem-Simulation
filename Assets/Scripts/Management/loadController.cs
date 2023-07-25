using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TerrainGenerator TG;
    [SerializeField] waterPlaces WP;
    [SerializeField] animalSpawner AS;
    [SerializeField] Grid grid;
    [SerializeField] bool EditorGen;
    [SerializeField] biomeGenerator BG;
    [SerializeField] weatherManager WM;
  //  bool inEditor; 
    void Start()
    {
#if !UNITY_EDITOR

        if (PlayerPrefs.GetString("genMode") == "Full")
            TG.generateTerrain();
        else if (PlayerPrefs.GetString("genMode") == "Quick")
            TG.partialGenerateTerrain();
        
#endif
        if(EditorGen)
            TG.partialGenerateTerrain();
        StartCoroutine(ExampleCoroutine());
       
        

    }
    IEnumerator ExampleCoroutine()
    {
      
        //yield on a new YieldInstruction that waits for 5 seconds.
        
        yield return new WaitForSeconds(1);
        BG.createBDict();
        AS.genSpawns();
        WP.findWater();
        grid.gridInit();
        AS.StartAnimals();
        WM.StartWeather();
    }

}
