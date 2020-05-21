using UnityEngine;

public class AccentText : MonoBehaviour
{
    public float speed = 0.5f;
    public float opacitySpeed = 0.25f;

    private TextMesh tm;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        tm = GetComponent<TextMesh>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.MovePosition(rb.transform.position + Vector3.up * Time.deltaTime * speed);
        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, tm.color.a - 0.1f*opacitySpeed);
    }
}
