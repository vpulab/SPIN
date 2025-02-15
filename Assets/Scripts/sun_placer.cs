using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sun_placer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void NewSun()
    {
        this.transform.rotation = newRotation();
        
    }
    //New rotation
    Quaternion newRotation() {

        float x = Random.value*2-1;
        float y = Random.value * 2 - 1;

        float z = Mathf.Pow(x, 2) + Mathf.Pow(y, 2); 

        while (z >= 1)
        {
            x = Random.value * 2 - 1;
            y = Random.value * 2 - 1;
            z = Mathf.Pow(x, 2) + Mathf.Pow(y, 2);
        }

        float u = Random.value * 2 - 1;
        float v = Random.value * 2 - 1;

        float w = Mathf.Pow(u, 2) + Mathf.Pow(v, 2);
        while (w >= 1)
        {
            u = Random.value * 2 - 1;
            v = Random.value * 2 - 1;
            w = Mathf.Pow(u, 2) + Mathf.Pow(v, 2);
        }

        float s = Mathf.Sqrt((1 - z) / w);

        return new Quaternion(x,y,s*u,s*v);
    }

}
