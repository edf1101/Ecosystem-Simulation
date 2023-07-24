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
        AS.genSpawns();
        WP.findWater();
        grid.gridInit();
        AS.StartAnimals();
        

    }

}
