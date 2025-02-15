using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{

    public float rotation_speed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       this.transform.eulerAngles = this.transform.eulerAngles + new Vector3(rotation_speed*Time.deltaTime,rotation_speed*Time.deltaTime,rotation_speed*Time.deltaTime);
    }
}
