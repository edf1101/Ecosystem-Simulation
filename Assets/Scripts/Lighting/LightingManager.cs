using UnityEngine;


public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light DirectionLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField] private float speed;
    [SerializeField] private bool AlwaysDay;
    [SerializeField] AnimationCurve nightRatio;
    [SerializeField] timeController TC;


    [SerializeField] float accTime; // time after curve applied
    private float TimeOfDay; // raw time

    //called each frame
    private  void Update()
    {
        speed = TC.speed * TC.multiplier;

        if (Preset == null) // if no preset then ignore
            return;

        if (Application.isPlaying)
        {
            if (!AlwaysDay) // update raw time if not always day mode
                TimeOfDay += (Time.deltaTime) * speed;
            else
                TimeOfDay = 12f;

            TimeOfDay %= 24; // make it stay between 24h

            UpdateLighting(nightRatio.Evaluate(  TimeOfDay/24f));

            // apply curve to time
            accTime = nightRatio.Evaluate(TimeOfDay / 24f) * 24f;
        }
        
    }
    void UpdateLighting(float timePer)
    {
        // set the lighting data according to current time and the gradients
        // in our preset


        RenderSettings.ambientLight = Preset.AmbientColour.Evaluate(timePer);
        RenderSettings.fogColor = Preset.FogColour.Evaluate(timePer);
        RenderSettings.skybox.SetColor("_GroundColor",Preset.SkyboxColour.Evaluate(timePer));

        //set the sun position in the sky
        if (DirectionLight != null)
        {
            DirectionLight.color=Preset.DirectionalColour.Evaluate(timePer);
            
                DirectionLight.transform.localRotation = Quaternion.Euler( new Vector3(((timePer) * 360f) - 90, -170, 0)   );
        }
    }

    // make sure all the correct light refernces are set up
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
