using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swapMaterials : MonoBehaviour
{

    public Material[] materials;

    public Material alternate_material;

    Renderer[] renderers;

    public bool isSwapped = false;

    private void Start()
    {
        renderers = gameObject.GetComponentsInChildren<Renderer>();
        materials = new Material[renderers.Length]; //Store original materials

        int count = 0;
        foreach (Renderer renderer in renderers) { 
            materials[count] = renderer.material; //Save origin material
            count++;
        }
        isSwapped = false;


    }

    public void Swap() {
        if (isSwapped)
        {
            SwapToDefault();
            isSwapped = false;
        }
        else
        {
            SwapToAlternate();
            isSwapped = true;
        }
    }
    void SwapToAlternate() {

        foreach (Renderer renderer in renderers)
        {
            if (renderer.gameObject.GetComponent<alternateMaterial>() != null) {
                renderer.material = renderer.gameObject.GetComponent<alternateMaterial>().alternate_material;
            }
            else 
                renderer.material = alternate_material;
        }
    }

    void SwapToDefault() {

        int count = 0;
        foreach (Renderer renderer in renderers)
        {
            renderer.material = materials[count];
            count++;
        }
    }





}
