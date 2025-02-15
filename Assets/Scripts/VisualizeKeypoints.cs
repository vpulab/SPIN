using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeKeypoints : MonoBehaviour
{

    public GameObject keyPointPrefab;
    public GameObject keyPointObject;
    
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

    List<GameObject> keypointsgobs;

    bool drawn;
    // Start is called before the first frame update
    void Start()
    {
        keypointsgobs = new List<GameObject>();
        drawn = false;
    }

    

    void DrawPoints() {
        foreach (Vector3 point in keypoints)
        {
            Vector3 point_fix = point;
            point_fix.z = -point_fix.z;
            point_fix.y = -point_fix.y;

            GameObject gob = Instantiate(keyPointPrefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform); // Quaternion.Inverse(transform.rotation)*
            gob.transform.localPosition = point_fix;
            keypointsgobs.Add(gob);
        }

        drawn = true;
    }

    void DeletePoints() { 
    
        foreach(GameObject gob in keypointsgobs)
            Destroy(gob);   

        drawn = false;
    }

    public void ToggleKeypoints(bool k) {
        if (k) {
            if (drawn)
                return;
            else
            {
                DrawPoints();
                drawn = true;
            }
        }
        else
        {
            if (drawn)
            {
                DeletePoints();
                drawn = false;
            }
            else
                return;
        }

    }

    public bool isDrawn()
    {
        return drawn;
    }

    public Vector3[] GetCurrentKeypointsCoordinates() {

        Vector3[] vector3s = new Vector3[keypoints.Length];
        int i = 0;
        foreach (Vector3 point in keypoints)
        {
            Vector3 point_fix = point;
            point_fix.z = -point_fix.z;
            point_fix.y = -point_fix.y;

            //Point fix is local position, must change to global.
            transform.TransformPoint(point_fix); 
            vector3s[i] = point_fix;
            i++;
            
        }

        return vector3s;
    }

    
}
