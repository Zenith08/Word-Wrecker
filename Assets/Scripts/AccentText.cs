using UnityEngine;

public class AccentText : MonoBehaviour
{
    public float speed = 1.5f;
    public float opacitySpeed = 0.1f;
    public string text;

    private TextMesh tm;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Generated");
        tm = GetComponent<TextMesh>();
        rb = GetComponent<Rigidbody>();
        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("TM Text is " + tm.text + " Goal is " + text);
        if(tm.text != text)
        {
            tm.text = text;
            tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, 1);
            Debug.Log("Becoming Visible");
        }
        rb.MovePosition(rb.transform.position + Vector3.up * Time.deltaTime * speed);
        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, tm.color.a - 0.1f*opacitySpeed);
        if(tm.color.a <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
