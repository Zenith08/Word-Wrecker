using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Camera cam;
    public float offset = 95;
    public float lerp = -80;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        Debug.Log("Detected camera with ratio " + cam.aspect);
        //cam.fieldOfView = offset + (lerp * cam.aspect);
        cam.orthographicSize = offset + (lerp * cam.aspect);
        lastAspect = cam.aspect;
    }

    //TODO Remove
    bool doonce = false;
    float lastAspect;
    private void Update()
    {
        if (!doonce)
        {
            Debug.LogError("Debug code still present");
            doonce = true;
        }
        if(lastAspect != cam.aspect)
        {
            Debug.LogError("Aspect Ratio Changed to " + cam.aspect);
            //cam.fieldOfView = offset + (lerp * cam.aspect);
            cam.orthographicSize = offset + (lerp * cam.aspect);
            lastAspect = cam.aspect;
        }
    }
}
