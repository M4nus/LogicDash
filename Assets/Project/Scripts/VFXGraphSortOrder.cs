using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXGraphSortOrder : MonoBehaviour
{
    public int sortPriority = -10; // Or whatever HDRP sortPriority you want to set it to.
    // Start is called before the first frame update
    void Start()
    {
        var renderer = gameObject.GetComponent<Renderer>(); //VFXRenderer inherits from Renderer
        if (renderer != null)
        {
            renderer.sharedMaterial.renderQueue = 3000 + sortPriority;
            renderer.sharedMaterial.SetFloat("_TransparentSortPriority", sortPriority);
        }
    }
}
