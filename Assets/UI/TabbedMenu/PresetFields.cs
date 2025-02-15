using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PresetFields 
{

    //Lightning "presetable" parameters
    public bool enableShadows;
    public bool enableSun;
    public float sunIntensity;
    public bool enableAmbientLight;
    public float ambientLightIntensity;
    public bool enableSpecular;
    public bool alternateMaterials;

    //Generation "presetable" parameters
    public bool background;
    public int background_rate;
    public string enable_background_at;
    public bool randomize_sun;
    public bool prevent_sun_occlussion;

    //Post Processing parameters
    public bool enableFilmGrain;
    public float filmGrainIntensity;
    public bool enableBloom;
    public float bloomIntensity;
    public float bloomScatter;
    public float bloomThreshold;

    public bool enableAmbientOcclusion;
    public bool enableColorAdjustments;

    public float focalDistance;
    public float focalLength;
    public float aperture;

    public float sensor_sx;
    public float sensor_sy;




}
