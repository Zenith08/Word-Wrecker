using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveCamController : MonoBehaviour
{
    public float offset = 95;
    public float lerp = -80;

    // Start is called before the first frame update
    void Start()
    {
        Camera cam = GetComponent<Camera>();
        //Debug.Log("Detected camera with ratio " + cam.aspect);
        cam.fieldOfView = offset + (lerp * cam.aspect);
    }

}
