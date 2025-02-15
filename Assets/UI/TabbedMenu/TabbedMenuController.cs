// This script defines the tab selection logic.
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.Settings;
using UnityEngine.Perception.GroundTruth.Consumers;


/*
    Controller for the tab menu with the different environment controls.

*/
public class TabbedMenuController
{
    private const string tabClassName = "tab";
    private const string currentlySelectedTabClassName = "currentlySelectedTab";
    private const string unselectedContentClassName = "unselectedContent";
    // Tab and tab content have the same prefix but different suffix
    // Define the suffix of the tab name
    private const string tabNameSuffix = "Tab";
    // Define the suffix of the tab content name
    private const string contentNameSuffix = "Content";

    private readonly VisualElement root;

    // Initialization of all the object components of the tabbed menu (for referencing the actual ui components)
    /* Lighting settings */
    Toggle enableShadows;
    Toggle enableSun;
    Slider sunIntensity;
    Toggle enableAmbientLight;
    Slider ambientLightIntensity;
    Toggle enableSpecular;
    Toggle alternateMaterials;

    GameObject satellite;
    GameObject[] ambientLights;
    GameObject otherUI;

    /* Generation Settings */
    public Toggle randomizeSun;
    public Toggle preventSunOcclusion;
    public Toggle background;
    SliderInt background_rate;
    Toggle fixBackgroundPose;
    Toggle fixBackgroundImage;

    TextField enableBackgroundAtImage;
    TextField posesPath;
    Button loadPoses;
    Button poses_sppsyn_t;
    Button poses_sppsyn_v;
    Button poses_spplb;
    Button poses_sppsl;

    public TextField datasetPath;

    TextField loadImageName;
    Button loadImagePose;

    Button new_sun;
    Button new_background;

    GameObject backgroundobject;


    /* Camera */
    TextField focal_distance;
    TextField focal_length;
    TextField aperture;
    TextField sensor_sx;
    TextField sensor_sy;
    Toggle linkToSatellite;




    /* Post Processing*/
    Toggle enableFilmGrain;
    Slider filmGrainIntensity;
    Toggle enableBloom;
    Slider bloomIntensity;
    Slider bloomScatter;
    Slider bloomThreshold;
    Toggle enableAmbientOcclusion;
    Toggle enableColorAdjustments;

    MinMaxSlider exposureMinMax;

    bool filmGrainEnabled;
    bool bloomEnabled;
    Bloom bloom;
    FilmGrain filmGrain;
    Exposure exposure;
    AmbientOcclusion ambientOcclusion;
    ColorAdjustments colorAdjustments;
    DepthOfField dof;

    /** CONFIG **/

    TextField preset_path;
    TextField preset_name;

    Button preset_load;
    Button preset_save;

    Button synthetic_preset;
    Button lightbox_preset;
    Button sunlamp_preset;

    TextField keypoints_file;
    Toggle display_keypoints;


    //Perception
    Toggle save_depth;
    TextField depth_save_location;
    //Other


    Volume volume;
    VolumeProfile profile;
    GameObject directionalLight;
    HDAdditionalLightData lightData;
    HDAdditionalCameraData cameraData;
    GameObject sun;


    //PerceptionEndpoint perception_settings = (PerceptionEndpoint)PerceptionSettings.GetOutputBasePath();

    public TabbedMenuController(VisualElement root, Volume volume, GameObject dirLight)
    {
        this.root = root;
        cameraData = Camera.main.GetComponent<HDAdditionalCameraData>();

        otherUI = GameObject.FindGameObjectWithTag("UI");
        // Set up lighting gameobjects
        this.directionalLight = dirLight;
        //this.lightData = new HDAdditionalLightData();
        ambientLights = GameObject.FindGameObjectsWithTag("AmbientLight");
        sun = GameObject.FindGameObjectWithTag("Sun");

        satellite = GameObject.FindGameObjectWithTag("Satellite");
        // Set up scene gameobjects
        backgroundobject = GameObject.FindWithTag("Planet");

        // Set up postprocessing gameobjects
        this.volume = volume;
        profile = volume.sharedProfile;
        #region Lightning Settings
        /* Lighting settings */
        enableShadows = root.Q<Toggle>("enable-shadows");
        enableSun = root.Q<Toggle>("enable-sun");
        sunIntensity = root.Q<Slider>("sun-intensity");
        enableAmbientLight = root.Q<Toggle>("enable-ambient-light");
        ambientLightIntensity = root.Q<Slider>("ambient-light-intensity");
        enableSpecular = root.Q<Toggle>("enable-specular");
        alternateMaterials = root.Q<Toggle>("alternate-materials");


        enableSun.value = directionalLight.GetComponent<Light>().enabled;
        sunIntensity.value = directionalLight.GetComponent<Light>().intensity;
        if (enableSun.value)
            sunIntensity.SetEnabled(true);
        else
            sunIntensity.SetEnabled(false);

        enableAmbientLight.value = ambientLights[0].GetComponent<Light>().enabled;
        ambientLightIntensity.value = ambientLights[0].GetComponent<Light>().intensity;
        if (enableAmbientLight.value)
            ambientLightIntensity.SetEnabled(true);
        else
            ambientLightIntensity.SetEnabled(false);

        enableShadows.value = cameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.ShadowMaps);
        enableShadows.RegisterCallback((ChangeEvent<bool> e) =>
        {
            enableShadows.value = e.newValue;
            cameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ShadowMaps, e.newValue);

        });
        enableSun.RegisterCallback((ChangeEvent<bool> e) =>
        {
            enableSun.value = e.newValue;
            directionalLight.GetComponent<Light>().enabled = enableSun.value;

            if (enableSun.value)
                sunIntensity.SetEnabled(true);
            else
                sunIntensity.SetEnabled(false);

        });
        sunIntensity.RegisterValueChangedCallback(e =>
        {
            directionalLight.GetComponent<Light>().intensity = e.newValue;
            sunIntensity.value = e.newValue;
        });
        enableAmbientLight.RegisterCallback((ChangeEvent<bool> e) =>
        {
            enableAmbientLight.value = e.newValue;
            foreach (GameObject ambientLight in ambientLights)
                ambientLight.GetComponent<Light>().enabled = e.newValue;

            if (enableAmbientLight.value)
                ambientLightIntensity.SetEnabled(true);
            else
                ambientLightIntensity.SetEnabled(false);

        });
        ambientLightIntensity.RegisterValueChangedCallback(e =>
        {
            foreach (GameObject ambientLight in ambientLights)
                ambientLight.GetComponent<Light>().intensity = e.newValue;

        });
        enableSpecular.value = cameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.DirectSpecularLighting);
        enableSpecular.RegisterCallback((ChangeEvent<bool> e) =>
        {
            enableSpecular.value = e.newValue;

            cameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.DirectSpecularLighting, e.newValue);

        });

        alternateMaterials.value = satellite.GetComponent<swapMaterials>().isSwapped;
        alternateMaterials.RegisterCallback((ChangeEvent<bool> e) =>
        {
            alternateMaterials.value = e.newValue;
            satellite.GetComponent<swapMaterials>().Swap();
        });

        #endregion

        #region Generation Settings
        /* Generation Settings */

        background = root.Q<Toggle>("enable-background");
        background_rate = root.Q<SliderInt>("background-rate");
        fixBackgroundImage = root.Q<Toggle>("fix-background-image");
        fixBackgroundPose = root.Q<Toggle>("fix-background-pose");
        enableBackgroundAtImage = root.Q<TextField>("enable-background-at");
        randomizeSun = root.Q<Toggle>("randomize-sun");
        preventSunOcclusion = root.Q<Toggle>("prevent-sun-occlusion");
        datasetPath = root.Q<TextField>("dataset-path");

        posesPath = root.Q<TextField>("load-poses-path");
        loadPoses = root.Q<Button>("load-poses");

        poses_sppsyn_t = root.Q<Button>("load-spdpp-syn-t");
        poses_sppsyn_v = root.Q<Button>("load-spdpp-syn-v");
        poses_sppsl = root.Q<Button>("load-spdpp-sl");
        poses_spplb = root.Q<Button>("load-spdpp-lb");
        loadImageName = root.Q<TextField>("go-to-image-name");
        loadImagePose = root.Q<Button>("go-to-image");

        new_sun = root.Q<Button>("new-sun");
        new_background = root.Q<Button>("new-background");

        background_rate.value = backgroundobject.GetComponent<change_background>().background_rate;
        background.value = backgroundobject.activeSelf;

        fixBackgroundImage.value = backgroundobject.GetComponent<change_background>().fix_background_image;
        fixBackgroundPose.value = backgroundobject.GetComponent<change_background>().fix_pose;

        background.RegisterCallback((ChangeEvent<bool> e) =>
        {
            background.value = e.newValue;
            backgroundobject.SetActive(e.newValue);
        });

        fixBackgroundImage.RegisterCallback((ChangeEvent<bool> e) =>
        {
            fixBackgroundImage.value = e.newValue;
            backgroundobject.GetComponent<change_background>().fix_background_image = e.newValue;
        });
        fixBackgroundPose.RegisterCallback((ChangeEvent<bool> e) =>
        {
            fixBackgroundPose.value = e.newValue;
            backgroundobject.GetComponent<change_background>().fix_pose = e.newValue;
        });

        background_rate.RegisterValueChangedCallback(e =>
        {
            backgroundobject.GetComponent<change_background>().background_rate = e.newValue;
        });

        loadPoses.RegisterCallback<ClickEvent>(e =>
        {
            LoadPosesFromFile();
        });

        poses_sppsyn_t.RegisterCallback<ClickEvent>(e =>
        {
            LoadPosesFromPreset("speedpp_syn_t");
        });
        poses_sppsyn_v.RegisterCallback<ClickEvent>(e =>
        {
            LoadPosesFromPreset("speedpp_syn_v");
        });
        poses_spplb.RegisterCallback<ClickEvent>(e =>
        {
            LoadPosesFromPreset("speedpp_lightbox");
        });
        poses_sppsl.RegisterCallback<ClickEvent>(e =>
        {
            LoadPosesFromPreset("speedpp_sunlamp");
        });

        loadImagePose.RegisterCallback<ClickEvent>(e =>
        {
            GoToImagePose(loadImageName.value);
        });
        new_sun.RegisterCallback<ClickEvent>(e =>
        {
            sun.GetComponent<sun_placer>().NewSun();
        });
        new_background.RegisterCallback<ClickEvent>(e =>
        {
            backgroundobject.GetComponent<change_background>().RandomBackground();
        });

        #endregion

        #region Camera Settings Tab

        /* Camera Settings Fields retrieval*/
        focal_distance = root.Q<TextField>("camera-focal-distance");
        focal_length = root.Q<TextField>("camera-focal-length");
        aperture = root.Q<TextField>("camera-aperture");
        sensor_sx = root.Q<TextField>("camera-sensor-size-x");
        sensor_sy = root.Q<TextField>("camera-sensor-size-y");
        linkToSatellite = root.Q<Toggle>("link-focal-distance");

        /* Post Processing settings fields retrieval */
        enableFilmGrain = root.Q<Toggle>("enable-film-grain");
        filmGrainIntensity = root.Q<Slider>("film-grain-intensity");
        enableBloom = root.Q<Toggle>("enable-bloom");
        bloomIntensity = root.Q<Slider>("bloom-intensity");
        bloomScatter = root.Q<Slider>("bloom-scatter");
        bloomThreshold = root.Q<Slider>("bloom-threshold");
        enableAmbientOcclusion = root.Q<Toggle>("ambient-occlusion");
        enableColorAdjustments = root.Q<Toggle>("color-adjustments");

        /* Values and listeners for camera settings */

        focal_length.value = Camera.main.GetComponent<Camera>().focalLength.ToString();
        focal_length.RegisterCallback((ChangeEvent<string> e) =>
        {
            try
            {
                if (e.newValue.Length > 0)
                {
                    // Attempt to parse and set the focal length
                    float focalLength = float.Parse(e.newValue);
                    Camera.main.GetComponent<Camera>().focalLength = focalLength;
                }
            }

            catch (Exception ex)
            {
                // Catch any other unexpected exceptions
                Debug.LogError($"An unexpected error occurred: {ex.Message}");
            }


        });

        /* Focal Distance*/
        if (!profile.TryGet<DepthOfField>(out dof))
            Debug.LogError("DepthOfField not in volume");

        focal_distance.value = dof.focusDistance.value.ToString();
        focal_distance.RegisterCallback((ChangeEvent<string> e) =>
            {
                try
                {
                    if (e.newValue.Length > 0)
                    {
                        // Attempt to parse and set the focal length
                        float focal_distance = float.Parse(e.newValue);
                        dof.focusDistance.value = focal_distance;
                    }
                }

                catch (Exception ex)
                {
                    // Catch any other unexpected exceptions
                    Debug.LogError($"An unexpected error occurred: {ex.Message}");
                }


            });

        aperture.value = Camera.main.GetComponent<HDAdditionalCameraData>().physicalParameters.aperture.ToString();
        aperture.RegisterCallback((ChangeEvent<string> e) =>
        {
            try
            {
                if (e.newValue.Length > 0)
                {
                    // Attempt to parse and set the focal length
                    float aperture = float.Parse(e.newValue);
                    if (aperture > 0.7)
                        Camera.main.GetComponent<HDAdditionalCameraData>().physicalParameters.aperture = aperture;
                }
            }

            catch (Exception ex)
            {
                // Catch any other unexpected exceptions
                Debug.LogError($"An unexpected error occurred: {ex.Message}");
            }

        });

        sensor_sx.value = Camera.main.GetComponent<Camera>().sensorSize.x.ToString();
        sensor_sy.value = Camera.main.GetComponent<Camera>().sensorSize.y.ToString();

        sensor_sx.RegisterCallback((ChangeEvent<string> e) =>
        {
            try
            {
                if (e.newValue.Length > 0)
                {
                    // Attempt to parse and set the focal length
                    float sx = float.Parse(e.newValue);
                    float sy = float.Parse(sensor_sy.value);

                    if (sx > 0)
                        Camera.main.GetComponent<Camera>().sensorSize = new Vector2(sx, sy);

                }
            }

            catch (Exception ex)
            {
                // Catch any other unexpected exceptions
                Debug.LogError($"An unexpected error occurred: {ex.Message}");
            }

        });

        sensor_sy.RegisterCallback((ChangeEvent<string> e) =>
        {
            try
            {
                if (e.newValue.Length > 0)
                {
                    // Attempt to parse and set the focal length
                    float sx = float.Parse(sensor_sx.value);
                    float sy = float.Parse(e.newValue);

                    if (sy > 0)
                        Camera.main.GetComponent<Camera>().sensorSize = new Vector2(sx, sy);

                }
            }

            catch (Exception ex)
            {
                // Catch any other unexpected exceptions
                Debug.LogError($"An unexpected error occurred: {ex.Message}");
            }

        });



        /* Film Grain*/
        if (!profile.TryGet<FilmGrain>(out filmGrain))
            Debug.LogError("Film grain not in volume");

        filmGrainIntensity.value = filmGrain.intensity.value;
        if (filmGrain.intensity.value > 0)
        {
            filmGrainEnabled = true;
            enableFilmGrain.value = true;

        }
        else
        {
            filmGrainEnabled = false;
            filmGrainIntensity.SetEnabled(false);
        }
        enableFilmGrain.RegisterCallback((ChangeEvent<bool> e) =>
        {
            filmGrainEnabled = e.newValue;
            enableFilmGrain.value = e.newValue;
            filmGrainIntensity.SetEnabled(e.newValue);

            if (e.newValue == false)
                filmGrain.intensity.value = 0;
            else
                filmGrain.intensity.value = filmGrainIntensity.value;

        });
        filmGrainIntensity.RegisterValueChangedCallback(e =>
        {
            filmGrain.intensity.value = e.newValue;
        });

        /* Bloom*/
        if (!profile.TryGet<Bloom>(out bloom))
            Debug.LogError("Bloom not in volume");
        bloomIntensity.value = bloom.intensity.value;
        bloomScatter.value = bloom.scatter.value;
        bloomThreshold.value = bloom.threshold.value;
        if (bloom.intensity.value > 0)
        {
            bloomEnabled = true;
            enableBloom.value = true;

        }
        else
        {
            bloomEnabled = false;
            bloomIntensity.SetEnabled(false);
        }

        bloomIntensity.RegisterValueChangedCallback(e =>
        {
            bloom.intensity.value = e.newValue;
        });
        enableBloom.RegisterCallback((ChangeEvent<bool> e) =>
        {
            bloomEnabled = e.newValue;
            enableBloom.value = e.newValue;
            bloomIntensity.SetEnabled(e.newValue);
            bloomScatter.SetEnabled(e.newValue);
            bloomThreshold.SetEnabled(e.newValue);
            if (e.newValue == false)
                bloom.intensity.value = 0;
            else
                bloom.intensity.value = bloomIntensity.value;

        });
        bloomScatter.RegisterValueChangedCallback(e =>
        {
            bloom.scatter.value = e.newValue;
        });
        bloomThreshold.RegisterValueChangedCallback(e =>
        {
            bloom.threshold.value = e.newValue;
        });

        /* Exposure */
        if (!profile.TryGet<Exposure>(out exposure))
            Debug.LogError("Exposure not in volume");

        /* Ambient Occlusion*/
        if (!profile.TryGet<AmbientOcclusion>(out ambientOcclusion))
        {
            Debug.LogError("Ambient Occlusion not in volume.");
        }

        enableAmbientOcclusion.value = ambientOcclusion.active;
        enableAmbientOcclusion.RegisterCallback((ChangeEvent<bool> e) =>
        {
            enableAmbientOcclusion.value = e.newValue;
            ambientOcclusion.active = e.newValue;

        });

        /*Color Adjustments*/
        if (!profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.Log("Color Adjustments not in volume");
        }

        enableColorAdjustments.value = colorAdjustments.active;
        enableColorAdjustments.RegisterCallback((ChangeEvent<bool> e) =>
        {
            enableColorAdjustments.value = e.newValue;
            colorAdjustments.active = e.newValue;
        });

        #endregion

        #region Configuration
        display_keypoints = root.Q<Toggle>("display-keypoints");
        save_depth = root.Q<Toggle>("toggle-depth-capture");
        depth_save_location = root.Q<TextField>("depth-files-location");
        preset_path = root.Q<TextField>("preset-file-path");
        //preset_name = root.Q<TextField>("preset-name");
        preset_save = root.Q<Button>("save-preset");
        preset_load = root.Q<Button>("load-preset");
        synthetic_preset = root.Q<Button>("speedpp-syn");
        lightbox_preset = root.Q<Button>("speedpp-lb");
        sunlamp_preset = root.Q<Button>("speedpp-sl");
        display_keypoints.value = satellite.transform.GetComponentInParent<VisualizeKeypoints>().isDrawn();

        display_keypoints.RegisterCallback((ChangeEvent<bool> e) =>
        {
            satellite.transform.GetComponentInParent<VisualizeKeypoints>().ToggleKeypoints(e.newValue);


        });

        save_depth.value = otherUI.GetComponent<button_logic>().save_modalities;
        save_depth.RegisterCallback((ChangeEvent<bool> e) =>
        {
            save_depth.value = e.newValue;
            otherUI.GetComponent<button_logic>().save_modalities = save_depth.value;
        });
      
        depth_save_location.value = PerceptionSettings.defaultOutputPath;

        preset_save.RegisterCallback<ClickEvent>(e =>
        {
            SavePresetToFile();
        });
        preset_load.RegisterCallback<ClickEvent>(e =>
        {
            LoadPresetFromFile();
        });

        synthetic_preset.RegisterCallback<ClickEvent>(e =>
        {
            LoadPreset("speedpp-syn");
        });
        lightbox_preset.RegisterCallback<ClickEvent>(e =>
        {
            LoadPreset("speedpp-lb");
        });
        sunlamp_preset.RegisterCallback<ClickEvent>(e =>
        {
            LoadPreset("speedpp-sl");
        });

        #endregion
    }

    #region Dynamic Tabs

    // Used to register callbacks (function that processes clicks) for each tab.
    public void RegisterTabCallbacks()
    {
        UQueryBuilder<Label> tabs = GetAllTabs();
        tabs.ForEach((Label tab) =>
        {
            tab.RegisterCallback<ClickEvent>(TabOnClick);
        });
    }

    /* Method for the tab on-click event: 

       - If it is not selected, find other tabs that are selected, unselect them 
       - Then select the tab that was clicked on
    */
    private void TabOnClick(ClickEvent evt)
    {
        Label clickedTab = evt.currentTarget as Label;

        if (!TabIsCurrentlySelected(clickedTab))
        {
            GetAllTabs().Where(
                (tab) => tab != clickedTab && TabIsCurrentlySelected(tab)
            ).ForEach(UnselectTab);
            SelectTab(clickedTab);
        }
        else
        {
            UnselectTab(clickedTab);
        }
    }
    //Method that returns a Boolean indicating whether a tab is currently selected
    private static bool TabIsCurrentlySelected(Label tab)
    {
        return tab.ClassListContains(currentlySelectedTabClassName);
    }

    private UQueryBuilder<Label> GetAllTabs()
    {
        return root.Query<Label>(className: tabClassName);
    }

    /* Method for the selected tab: 
       -  Takes a tab as a parameter and adds the currentlySelectedTab class
       -  Then finds the tab content and removes the unselectedContent class */
    private void SelectTab(Label tab)
    {
        tab.AddToClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.style.display = DisplayStyle.Flex;
        content.RemoveFromClassList(unselectedContentClassName);
    }

    /* Method for the unselected tab: 
       -  Takes a tab as a parameter and removes the currentlySelectedTab class
       -  Then finds the tab content and adds the unselectedContent class */
    private void UnselectTab(Label tab)
    {
        tab.RemoveFromClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.style.display = DisplayStyle.None;
        content.AddToClassList(unselectedContentClassName);
    }

    // Method to generate the associated tab content name by for the given tab name
    private static string GenerateContentName(Label tab) =>
        tab.name.Replace(tabNameSuffix, contentNameSuffix);

    // Method that takes a tab as a parameter and returns the associated content element
    private VisualElement FindContent(Label tab)
    {
        return root.Q(GenerateContentName(tab));
    }

    #endregion

    #region Poses loading

    // This section will be updated in the future it is not very convenient right now
    public void UpdateImageName(string str)
    {

        if (enableBackgroundAtImage.value == str)
        {
            //Si  tiene un nombre de imagen, en el momento en que coincida, activar background. Random a 1, y enable a true
            background.value = true;
            background_rate.value = 100;
        }
    }

    public void LoadPosesFromFile()
    {
        string aux = posesPath.value;
        string image_name = Camera.main.GetComponent<PoseLoader>().LoadPoses(aux.ToString().Replace(@"\", "/"));
        UpdateImageName(image_name);
    }

    public void LoadPosesFromPreset(string str)
    {
        TextAsset file = Resources.Load<TextAsset>("default_poses/" + str);
        string image_name = Camera.main.GetComponent<PoseLoader>().LoadPoses(file.text, true);
        UpdateImageName(image_name);

    }

    public void GoToImagePose(string str)
    {
        bool ret = Camera.main.GetComponent<PoseLoader>().LoadImageByName(str);

        if (ret == false)
        {
            loadImageName.value = "Error image not in poses";
        }
    }

    #endregion

    #region PresetIO
    // Code to manage loading and saving presets. Currently only one custom preset is supported. 
    public void SavePresetToFile()
    {
        // Get path for profile.
        string preset_path_string = "custom_preset.json";
        //Create serializable object

        PresetFields preset = new PresetFields();

        preset.enableShadows = this.enableShadows.value;
        preset.enableSun = this.enableSun.value;
        preset.sunIntensity = this.sunIntensity.value;
        preset.enableAmbientLight = this.enableAmbientLight.value;
        preset.ambientLightIntensity = this.ambientLightIntensity.value;
        preset.enableSpecular = this.enableSpecular.value;
        preset.alternateMaterials = this.alternateMaterials.value;

        preset.background = this.background.value;
        preset.background_rate = this.background_rate.value;
        preset.enable_background_at = this.enableBackgroundAtImage.value;
        preset.randomize_sun = this.randomizeSun.value;
        preset.prevent_sun_occlussion = this.preventSunOcclusion.value;

        preset.enableFilmGrain = this.enableFilmGrain.value;
        preset.filmGrainIntensity = this.filmGrainIntensity.value;
        preset.enableBloom = this.enableBloom.value;
        preset.bloomIntensity = this.bloomIntensity.value;
        preset.bloomScatter = this.bloomScatter.value;
        preset.bloomThreshold = this.bloomThreshold.value;

        preset.enableAmbientOcclusion = this.enableAmbientOcclusion.value;
        preset.enableColorAdjustments = this.enableColorAdjustments.value;

        try
        {
            preset.aperture = float.Parse(this.aperture.value);
            preset.focalLength = float.Parse(this.focal_length.value);
            preset.focalDistance = float.Parse(this.focal_distance.value);
            preset.sensor_sx = float.Parse(this.sensor_sx.value);
            preset.sensor_sy = float.Parse(this.sensor_sy.value);
        }
        catch
        {
            Debug.LogError("Could not load camera parameters.");
        }

        string json_string = JsonUtility.ToJson(preset);

        File.WriteAllText(preset_path_string, json_string);

    }
    public void LoadPresetValues(PresetFields preset)
    {
        this.enableShadows.value = preset.enableShadows;
        this.enableSun.value = preset.enableSun;
        this.sunIntensity.value = preset.sunIntensity;
        this.enableAmbientLight.value = preset.enableAmbientLight;
        this.ambientLightIntensity.value = preset.ambientLightIntensity;
        this.enableSpecular.value = preset.enableSpecular;
        this.alternateMaterials.value = preset.alternateMaterials;

        this.background.value = preset.background;
        this.background_rate.value = preset.background_rate;
        this.enableBackgroundAtImage.value = preset.enable_background_at;
        this.randomizeSun.value = preset.randomize_sun;
        this.preventSunOcclusion.value = preset.prevent_sun_occlussion;

        this.enableFilmGrain.value = preset.enableFilmGrain;
        this.filmGrainIntensity.value = preset.filmGrainIntensity;
        this.enableBloom.value = preset.enableBloom;
        this.bloomIntensity.value = preset.bloomIntensity;
        this.bloomScatter.value = preset.bloomScatter;
        this.bloomThreshold.value = preset.bloomThreshold;

        this.enableAmbientOcclusion.value = preset.enableAmbientOcclusion;
        this.enableColorAdjustments.value = preset.enableColorAdjustments;

        this.aperture.value = preset.aperture.ToString();
        this.focal_length.value = preset.focalLength.ToString();
        this.focal_distance.value = preset.focalDistance.ToString();
        this.sensor_sx.value = preset.sensor_sx.ToString();
        this.sensor_sy.value = preset.sensor_sy.ToString();
    }
    public void LoadPresetFromFile()
    {
        // Get path for profile.
        string preset_path_string = "custom_preset.json";

        //Create serializable object

        PresetFields preset = JsonUtility.FromJson<PresetFields>(File.ReadAllText(preset_path_string));

        LoadPresetValues(preset);

    }
    public void LoadPreset(string name)
    {
        // Get path for profile.
        TextAsset file = Resources.Load<TextAsset>("default_presets/" + name);
        //Create serializable object

        PresetFields preset = JsonUtility.FromJson<PresetFields>(file.text);

        LoadPresetValues(preset);

    }

    #endregion


}