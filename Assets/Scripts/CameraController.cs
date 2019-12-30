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
        //Debug.Log("Detected camera with ratio " + cam.aspect);
        cam.fieldOfView = offset + (lerp * cam.aspect);
    }
}
