using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PoseLoader : MonoBehaviour
{
    public TextAsset jsonFile;
    public GameObject tango;
    public int file_no;
    int position_counter = 0;
    Poses posesInJson;
    bool autopose = false;
    float pps = 25.0f;
    float deltaPose;
    float poseTimer;

    public GameObject image_name_display;
    // Start is called before the first frame update
    void Start()
    {
        deltaPose = 1 / pps;
        poseTimer = deltaPose;
        // Set up camera matrix
        Matrix4x4 cameraMatrix = new Matrix4x4();

        cameraMatrix[0, 0] = 2988.5795163815555f;
        cameraMatrix[0, 1] = 0f;
        cameraMatrix[0, 2] = 960f;
        cameraMatrix[0, 3] = 0f;

        cameraMatrix[0, 0] = 0f;
        cameraMatrix[0, 1] = 2988.3401159176124f;
        cameraMatrix[0, 2] = 600f;
        cameraMatrix[0, 3] = 0f;

        cameraMatrix[0, 0] = 0f;
        cameraMatrix[0, 1] = 0f;
        cameraMatrix[0, 2] = 1f;
        cameraMatrix[0, 3] = 0f;

        cameraMatrix[0, 0] = 0f;
        cameraMatrix[0, 1] = 0f;
        cameraMatrix[0, 2] = 1f;
        cameraMatrix[0, 3] = 0f;

    }

    public string LoadPoses(string path)
    {
        position_counter = 0;
        if (path != "")
        {
            string json_text = File.ReadAllText(path);
            posesInJson = JsonUtility.FromJson<Poses>(json_text);
        }
        else
        {
            posesInJson = JsonUtility.FromJson<Poses>(jsonFile.text);
        }


        Vector3 start_pos = tango.transform.position;
        Quaternion start_rot = tango.transform.rotation;
        Pose p = posesInJson.poses[position_counter];


        tango.transform.position = (new Vector3((p.r_Vo2To_vbs_true[0]), -(p.r_Vo2To_vbs_true[1]), p.r_Vo2To_vbs_true[2]));
        tango.transform.rotation = Quaternion.Inverse(new Quaternion(p.q_vbs2tango_true[1], -p.q_vbs2tango_true[2], p.q_vbs2tango_true[3], p.q_vbs2tango_true[0]));
        image_name_display.GetComponent<UnityEngine.UI.Text>().text = p.filename;

        return p.filename;
    }
    public string LoadPoses(string jsonstring, bool isJsonString)
    {
        position_counter = 0;
        if (!isJsonString)
        {
            return LoadPoses(jsonstring);
        }
        if (jsonstring != "")
        {
            posesInJson = JsonUtility.FromJson<Poses>(jsonstring); ;
        }
        else
        {
            posesInJson = JsonUtility.FromJson<Poses>(jsonFile.text);
        }

        Vector3 start_pos = tango.transform.position;
        Quaternion start_rot = tango.transform.rotation;
        Pose p = posesInJson.poses[position_counter];


        tango.transform.position = (new Vector3((p.r_Vo2To_vbs_true[0]), -(p.r_Vo2To_vbs_true[1]), p.r_Vo2To_vbs_true[2]));
        tango.transform.rotation = Quaternion.Inverse(new Quaternion(p.q_vbs2tango_true[1], -p.q_vbs2tango_true[2], p.q_vbs2tango_true[3], p.q_vbs2tango_true[0]));
        image_name_display.GetComponent<UnityEngine.UI.Text>().text = p.filename;

        return p.filename;
    }

    public string LoadPose(int pose)
    {
        int pose_n = pose;

        Pose p = posesInJson.poses[pose_n];
        position_counter = pose_n;

        tango.transform.position = (new Vector3((p.r_Vo2To_vbs_true[0]), -(p.r_Vo2To_vbs_true[1]), p.r_Vo2To_vbs_true[2]));
        tango.transform.rotation = Quaternion.Inverse(new Quaternion(p.q_vbs2tango_true[1], -p.q_vbs2tango_true[2], p.q_vbs2tango_true[3], p.q_vbs2tango_true[0]));
        image_name_display.GetComponent<UnityEngine.UI.Text>().text = p.filename;

        return p.filename;
    }
    public string LoadNextPose()
    {
        if (position_counter == posesInJson.poses.Length - 1)
            position_counter = 0;
        else
            position_counter++;
        Pose p = posesInJson.poses[position_counter];

        tango.transform.position = (new Vector3((p.r_Vo2To_vbs_true[0]), -(p.r_Vo2To_vbs_true[1]), p.r_Vo2To_vbs_true[2]));
        tango.transform.rotation = Quaternion.Inverse(new Quaternion(p.q_vbs2tango_true[1], -p.q_vbs2tango_true[2], p.q_vbs2tango_true[3], p.q_vbs2tango_true[0]));
        image_name_display.GetComponent<UnityEngine.UI.Text>().text = p.filename;

        return p.filename;
    }
    public string LoadPreviousPose()
    {
        if (position_counter == 0)
            position_counter = posesInJson.poses.Length - 1;
        else
            position_counter--;
        Pose p = posesInJson.poses[position_counter];

        tango.transform.position = (new Vector3((p.r_Vo2To_vbs_true[0]), -(p.r_Vo2To_vbs_true[1]), p.r_Vo2To_vbs_true[2]));
        tango.transform.rotation = Quaternion.Inverse(new Quaternion(p.q_vbs2tango_true[1], -p.q_vbs2tango_true[2], p.q_vbs2tango_true[3], p.q_vbs2tango_true[0]));
        image_name_display.GetComponent<UnityEngine.UI.Text>().text = p.filename;

        return p.filename;


    }

    public bool LoadImageByName(string imagename)
    {

        int pointer = 0;
        Debug.Log(imagename);
        for (pointer = 0; pointer < posesInJson.poses.Length; pointer++)
        {
            if (posesInJson.poses[pointer].filename == imagename)
            {
                LoadPose(pointer);
                return true;
            }
        }

        return false;

    }
    // Update is called once per frame

    void Update()
    {
        if (autopose == true)
        {
            if (poseTimer <= 0)
            {
                LoadNextPose();
                poseTimer = deltaPose;
            }
            else
                poseTimer = poseTimer - Time.deltaTime;
        }


    }

    public Pose getCurrentPose()
    {
        if (posesInJson == null || posesInJson.poses == null)
        {
            return null; // Return null if posesInJson or poses is not initialized.
        }

        if (position_counter < 0 || position_counter >= posesInJson.poses.Length)
        {
            return null; // Return null if position_counter is out of bounds.
        }

        return posesInJson.poses[position_counter];
    }

    public Vector3 getCurrentPosition()
    {
        return tango.transform.position;
    }
    public (Vector2 max, Vector2 min, float z) GetBBMask()
    {

        //Project points
        //// Get points
        Vector3[] global_keypoints = GetCurrentKeypointsCoordinates();

        ////With camera calibration, project them (no need???)
        //Vector3[] projected_keypoints = new Vector3[global_keypoints.Length];
        Vector2 max = new Vector2(0, 0);
        Vector2 min = new Vector2(0, 0);


        for (int i = 0; i < global_keypoints.Length; i++)
        {
            if (global_keypoints[i].x > max.x)
                max.x = global_keypoints[i].x;
            else if (global_keypoints[i].x < min.x)
                min.x = global_keypoints[i].x;

            if (global_keypoints[i].y > max.y)
                max.y = global_keypoints[i].y;
            else if (global_keypoints[i].y < min.y)
                min.y = global_keypoints[i].y;

        }
        //Get bounding box indices
        return (max, min, tango.transform.position.z);


    }


    public void ToggleAutopose()
    {
        autopose = !autopose;
    }

    public Vector3[] GetCurrentKeypointsCoordinates()
    {

        Vector3[] keypoints = new Vector3[] {
        new Vector3(-0.37f, -0.265f, 0.02f),
        new Vector3(-0.37f, 0.305f, 0.02f),
        new Vector3(0.37f, 0.305f, 0.02f),
        new Vector3(0.37f, -0.265f, 0.02f),
        new Vector3(-0.37f, 0.385f, 0.32f),
        new Vector3(0.37f, 0.385f, 0.32f),
        new Vector3(0.37f, -0.385f, 0.32f),
        new Vector3(-0.37f, -0.385f, 0.3f),
        new Vector3(0.3f, -0.585f, 0.247f),
        new Vector3(-0.54f, 0.485f, 0.247f),
        new Vector3(0.55f, 0.495f, 0.247f)
    };

        Vector3[] vector3s = new Vector3[keypoints.Length];
        int i = 0;
        foreach (Vector3 point in keypoints)
        {
            Vector3 point_fix = point;
            point_fix.z = -point_fix.z;
            point_fix.y = -point_fix.y;


            //Point fix is local position, must change to global.
            point_fix = transform.TransformPoint(point_fix);
            vector3s[i] = point_fix;
            i++;

        }

        return vector3s;
    }



}
