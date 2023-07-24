using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light DirectionLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField] private float speed;
    [SerializeField] bool AlwaysDay;
     private float TimeOfDay;
    [SerializeField] AnimationCurve nightRatio;
    [SerializeField] float accTime;
  //  [SerializeField] private Light moon;
   private  void Update()
    {
        if (Preset == null)
            return;
        if (Application.isPlaying)
        {
            if (!AlwaysDay)
                TimeOfDay += (Time.deltaTime) * speed;
            else
                TimeOfDay = 12f;
            TimeOfDay %= 24;
            UpdateLighting(nightRatio.Evaluate(  TimeOfDay/24f));
            accTime = nightRatio.Evaluate(TimeOfDay / 24f) * 24f;
        }
        
    }
    void UpdateLighting(float timePer)
    {
        RenderSettings.ambientLight = Preset.AmbientColour.Evaluate(timePer);
        RenderSettings.fogColor = Preset.FogColour.Evaluate(timePer);
        RenderSettings.skybox.SetColor("_GroundColor",Preset.SkyboxColour.Evaluate(timePer));
        if (DirectionLight != null)
        {
            DirectionLight.color=Preset.DirectionalColour.Evaluate(timePer);
            
                DirectionLight.transform.localRotation = Quaternion.Euler( new Vector3(((timePer) * 360f) - 90, -170, 0)   );
               // moon.transform.localRotation = Quaternion.Euler(new Vector3(90-((timePer) * 360f) - 0, 10, 0));

        }
    }

    void OnValidate()
    {
        if (DirectionLight != null)
            return;
        if (RenderSettings.sun!=null)
        {
            DirectionLight = RenderSettings.sun;
            
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                if (light.type==LightType.Directional)
                    DirectionLight = light;
            }
        }
    }

}
