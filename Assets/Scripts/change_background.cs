using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_background : MonoBehaviour
{

    public Texture2D[] possible_backgrounds;

    public float maxDistance = 80;
    public float minDistance = 50;

    public float max_xangle = 90;
    public float min_xangle = -90;
    public int background_rate = 0;
    Quaternion base_rotation;

    public bool fix_background_image = false;
    public bool fix_pose = false;

    // Start is called before the first frame update
    void Start()
    {
        base_rotation= transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void NewPose() { 

        //Randomize a new distance
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, Random.Range(maxDistance,minDistance));
    }

    void NewRotation() {
        
        this.transform.localRotation = base_rotation*Quaternion.AngleAxis(Random.Range(min_xangle, max_xangle), Vector3.up);
    }

    void RandomMirror() { 
        bool mirror = ( Random.Range(min_xangle,max_xangle)>0.5) ? true : false; 
        
        if(mirror)
            this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z * -1);
    }

    void RandomImage() {
        int len = possible_backgrounds.Length;
        int next_texture = Random.Range(0, len);

        GetComponent<Renderer>().material.SetTexture("_BaseColorMap", possible_backgrounds[next_texture]);
    
    }

    void RandomHideBackground()
    {
        if (background_rate > 0)
        {
            int roll = Random.Range(0, 100);

            if (roll < background_rate)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);

        }
    }

    public void RandomBackground() {
        
        if(fix_pose==false)
        {
            NewPose();
            NewRotation();
            RandomMirror();
        }

        if (fix_background_image == false)
        {
            RandomImage();
            RandomHideBackground();
        }
    }
}
