using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Perception.GroundTruth;


/*
    This is the main controller for the environment. It is where the UI sends instructions to modify the behavior of the actual game objects.

*/
public class button_logic : MonoBehaviour
{
    public GameObject textfield;
    public GameObject image_textfield;
    public GameObject image_name_display;
    public GameObject textfield_dataset;
    public GameObject sunlight;
    public GameObject randomize_sun_toggle;
    public GameObject settings_menu;
    public GameObject generate_settings;
    public GameObject volume_global;
    VolumeProfile profile;
    Exposure exposure;

    public bool save_rgb = true;

    public bool save_modalities = true;

    private bool datasetGeneration;
    private string first_image_name;
    private string image_name;
    private int new_sun_counter;

    private int gen_status = 0; //0 Unitialized, 1 generation, 2 pause

    private GameObject background_image;
    RenderTexture rt;
    RenderTexture oldRT;
    Camera cam;
    Texture2D tex;
    byte[] bytes;
    string path;
    float default_exposure_compensation;



    //This variable is used to prevent the loop from getting stuck on certain pictures with the auto exposure compensation to prevent heavy overexposure.
    private float savedelay = 0.05f;
    // Load the next pose in the list
    public void Next()
    {
        image_name = Camera.main.GetComponent<PoseLoader>().LoadNextPose();
        image_name_display.GetComponent<UnityEngine.UI.Text>().text = image_name;
        settings_menu.GetComponent<TabbedMenu>().UpdateImageName(image_name);

        Vector3 satellite_pose = Camera.main.GetComponent<PoseLoader>().getCurrentPosition();

        exposure.proceduralCenter = new NoInterpVector2Parameter(new Vector2(satellite_pose.x, satellite_pose.y));

    }
    // Load the previous pose in the list
    public void Back()
    {
        image_name = Camera.main.GetComponent<PoseLoader>().LoadPreviousPose();
        image_name_display.GetComponent<UnityEngine.UI.Text>().text = image_name;

        Vector3 satellite_pose = Camera.main.GetComponent<PoseLoader>().getCurrentPosition();
        Debug.Log(satellite_pose);

        exposure.proceduralCenter = new NoInterpVector2Parameter(new Vector2(satellite_pose.x, satellite_pose.y));
    }

    // Load the poses from a file
    public void LoadPoses()
    {
        string aux = textfield.GetComponent<UnityEngine.UI.InputField>().text;
        Debug.Log(aux);
        image_name = Camera.main.GetComponent<PoseLoader>().LoadPoses(textfield.GetComponent<UnityEngine.UI.InputField>().text.ToString().Replace(@"\", "/"));
        image_name_display.GetComponent<UnityEngine.UI.Text>().text = image_name;
        gen_status = 0;
        generate_settings.GetComponentInChildren<UnityEngine.UI.Text>().text = "Generate Dataset";
        Vector3 satellite_pose = Camera.main.GetComponent<PoseLoader>().getCurrentPosition();
        Debug.Log(satellite_pose);
        exposure.proceduralCenter = new NoInterpVector2Parameter(new Vector2(satellite_pose.x, satellite_pose.y));

    }


    // Not currently in use, the idea is to load an image using the textfield
    public void GoToImage()
    {
        //string aux = image_textfield.GetComponent<UnityEngine.UI.InputField>().text;

        //image_name = Camera.main.GetComponent<PoseLoader>().LoadPose(aux);
        //image_name_display.GetComponent<UnityEngine.UI.Text>().text = image_name;

    }

    public void ToggleAutoPose()
    {
        Camera.main.GetComponent<PoseLoader>().ToggleAutopose();
    }


    // This controls the dataset generation status.
    public void GenerateDataset()
    {
        if (gen_status == 0)
        {
            Pose current_pose = Camera.main.GetComponent<PoseLoader>().getCurrentPose();
            string datapath = settings_menu.GetComponent<TabbedMenu>().GetTextField(TabbedMenu.TextFields.Dataset_Path).ToString().Replace(@"\", "/");
            if (current_pose != null && datapath.Length > 0){
                first_image_name = image_name_display.GetComponent<UnityEngine.UI.Text>().text;
                generate_settings.GetComponentInChildren<UnityEngine.UI.Text>().text = "Pause";
                datasetGeneration = true;
                new_sun_counter = 0;
                gen_status = 1;
                image_name = Camera.main.GetComponent<PoseLoader>().getCurrentPose().filename;
            }else{
                string message = "";
                if (current_pose == null)
                    message = "You need to load Poses from Poses tab. \n";
                if (datapath.Length == 0)
                    message += " " + "Be sure to set up the output path in generation tab.";
                    image_name_display.GetComponent<UnityEngine.UI.Text>().text = message;
                
            }

        }
        else if (gen_status == 1)
        {
            datasetGeneration = false;
            generate_settings.GetComponentInChildren<UnityEngine.UI.Text>().text = "Resume";
            gen_status = 2;

        }
        else if (gen_status == 2)
        {
            datasetGeneration = true;
            generate_settings.GetComponentInChildren<UnityEngine.UI.Text>().text = "Pause";
            gen_status = 1;


        }

    }

    private void Start()
    {
        cam = Camera.main;
        background_image = GameObject.FindGameObjectWithTag("Planet");

        profile = volume_global.GetComponent<Volume>().sharedProfile;

        if (!profile.TryGet<Exposure>(out exposure))
            Debug.LogError("Exposure not in volume");

        default_exposure_compensation = exposure.compensation.value;
    }

    // Update is called once per frame
    void Update()
    {   
        // If we are generating a dataset
        if (datasetGeneration)
        {
            // If not on delay (to allow time for the scene to render after pose change)
            if (savedelay <= 0)
            {

                if (settings_menu.GetComponent<TabbedMenu>().GetBooleanField(TabbedMenu.BooleanFields.RandomizedSun))
                {
                    bool prevent_sun_occ = settings_menu.GetComponent<TabbedMenu>().GetBooleanField(TabbedMenu.BooleanFields.PreventSunOcclusion);

                    sunlight.GetComponent<sun_placer>().NewSun();
                }


                background_image.GetComponent<change_background>().RandomBackground();

                //save modalities if desired
                if(save_modalities){
                    cam.GetComponent<PerceptionCamera>().RequestCapture();

                }
                //save image
                if (save_rgb)
                {
                    rt = new RenderTexture(1920, 1200, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                    oldRT = cam.targetTexture;
                    cam.targetTexture = rt;
                    cam.Render();
                    cam.targetTexture = oldRT;

                    RenderTexture.active = rt;
                    tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

                    tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

                    float averageBrightness = GetAverageValuesInMaskedFrame(tex, rt);

                    //THIS NEEDS A TOGGLE

                    if (averageBrightness > 0.7)
                    {
                        //adjust exposure_compensation
                        exposure.compensation.value = exposure.compensation.value - 2;

                        Destroy(tex);
                        Destroy(rt);

                        savedelay = 0.35f; ///This is to prevent a bug where it gets stuck on the same pose trying to compensate exposure


                        return;
                    }


                    exposure.compensation.value = default_exposure_compensation;

                    RenderTexture.active = null;


                    //store the image
                    bytes = tex.EncodeToPNG();


                    string dataset_path = settings_menu.GetComponent<TabbedMenu>().GetTextField(TabbedMenu.TextFields.Dataset_Path).ToString().Replace(@"\", "/");

                    path = "" + dataset_path + "/" + image_name.Split('.')[0] + ".png";
                    System.IO.File.WriteAllBytes(path, bytes);

                    Destroy(tex);
                    Destroy(rt);
                }

                if(save_modalities || save_rgb)
                    savedelay = 0.10f;

                Next();

                if (image_name == first_image_name)
                {

                    datasetGeneration = false;

                }

            }
            else
            {
                savedelay -= Time.deltaTime;
            }
        }

    }

    // This function is used to get the average brightness of the image in the masked area around the satellite, to minimize the amount of fully overexposed images.
    private float GetAverageValuesInMaskedFrame(Texture2D texture, RenderTexture rt)
    {
        var (max, min, z) = Camera.main.GetComponent<PoseLoader>().GetBBMask();
        Vector3 maxz = Camera.main.WorldToScreenPoint(new Vector3(max.x, max.y, z));

        Vector3 minz = Camera.main.WorldToScreenPoint(new Vector3(min.x, min.y, z));

        //Debug.Log("X: " + minz.x + " Y: " + minz.y);
        //Debug.Log("Max x: " + max.x + " Min x:" + min.x + " Diff: " + (max.x - min.x));
        //Debug.Log("Max y: " + max.y + " Min x:" + min.y + " Diff: " + (max.y - min.y));

        int width = Mathf.RoundToInt(maxz.x - minz.x);
        int height = Mathf.RoundToInt(maxz.y - minz.y);
        int x = Mathf.RoundToInt(minz.x);
        int y = Mathf.RoundToInt(minz.y);
        //Debug.Log("Trying to catch pixels from x:" + x + " y:" + y + " with width:" + width + " and height: " + height);

        if ((x + width) > rt.width)
            width = width - (x + width - rt.width);
        if ((y + height) > rt.height)
            height = height - (y + height - rt.height);

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(x, y, width, height), 0, 0);

        RenderTexture.active = null;

        Color[] pixels = tex.GetPixels();
        Color sumColor = new Color(0, 0, 0);
        foreach (Color pixel in pixels)
        {
            sumColor += pixel;
        }
        Color avgColor = sumColor / pixels.Length;

        //string dataset_path = settings_menu.GetComponent<TabbedMenu>().GetTextField(TabbedMenu.TextFields.Dataset_Path).ToString().Replace(@"\", "/");
        //Debug.Log("Image" + image_name.Split('.')[0] + " crop: " + avgColor );

        Destroy(tex);


        return avgColor[0];


    }

}
