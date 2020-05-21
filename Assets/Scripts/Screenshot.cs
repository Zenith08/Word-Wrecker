using UnityEngine;

public class Screenshot : MonoBehaviour
{
    private Camera targetCamera;
    private static Screenshot instance;
    private bool takeScreenshotNextFrame;

    public int width = 3840, height = 2160;

    private void Awake()
    {
        instance = this;
        targetCamera = gameObject.GetComponent<Camera>();
    }

    private void OnPostRender()
    {
        if(takeScreenshotNextFrame)
        {
            takeScreenshotNextFrame = false;
            RenderTexture renderTexture = targetCamera.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] bytearray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/camerascreenshot.png", bytearray);
            Debug.Log("Screenshot captured");
            RenderTexture.ReleaseTemporary(renderTexture);
            targetCamera.targetTexture = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void TakeScreenshot(int width, int height)
    {
        targetCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotNextFrame = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TakeScreenshot(width, height);
        }
    }
}
