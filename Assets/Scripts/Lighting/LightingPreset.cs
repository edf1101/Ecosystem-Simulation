
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName="Lighting Preset",menuName="Scriptables/Lighting Preset",order=1)]
public class LightingPreset : ScriptableObject
{
    public Gradient AmbientColour;
    public Gradient DirectionalColour;
    public Gradient FogColour;
    public Gradient SkyboxColour;
    
}
