using UnityEngine;

public class TextScript : MonoBehaviour {

    public static Color normal = Color.white;
    public static Color highlighted = Color.blue;
    public static Color fail = Color.red;
    public static Color error = Color.yellow;

	public int speed = 5;
    public int ErrorTime = 2 * 30;

	private TextMesh displayText;
	private char characterValue;
	private Rigidbody rb;
	private bool falling = false;
	public byte fallTimer = 2;
    private int errTime;
    
    // Use this for initialization
    void Start () {
		displayText = GetComponent<TextMesh> ();
        GetComponent<MeshRenderer>().enabled = false;
		rb = GetComponent<Rigidbody> ();
		falling = true;
	}

	// Update is called once per frame
	void Update () {
		if (displayText.text != characterValue.ToString ()) {
			displayText.text = characterValue.ToString ();
            GetComponent<MeshRenderer>().enabled = true;
		}
        if(errTime > 0)
        {
            errTime--;
            if(errTime == 0)
            {
                setHighlighted(false);
            }
        }
	}

	void FixedUpdate(){
		if (falling) {
			rb.MovePosition (rb.transform.position + Vector3.down * Time.deltaTime * speed);
		} else {
			fallTimer--;
			if (fallTimer == 0) {
				falling = true;
				fallTimer = 2;
			}
		}
	}

	void OnTriggerStay(Collider collider){
		if (collider.transform.position.y + 0.5F < gameObject.transform.position.y || collider.gameObject.tag == "Ground") {
			fallTimer = 2;
		} else {
			//Don't do that
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Letter" || other.tag == "Ground") {
			falling = false;
		}
	}

	void OnTriggerExit(){
		falling = true;
	}

	public void setLetter(char letter){
		//Shouldn't need ToUpper(char) later on but it is a good catcher
		characterValue = char.ToUpper(letter);
	}

	public char getLetter(){
		return characterValue;
	}

    public void setHighlighted(bool highlight)
    {
        TextMesh tm = GetComponent<TextMesh>();
        if (highlight)
        {
            tm.color = highlighted;
        }
        else
        {
            tm.color = normal;
        }
    }

    public void setFailiureState(bool internalError)
    {
        errTime = ErrorTime;
        TextMesh tm = GetComponent<TextMesh>();
        if (internalError)
        {
            tm.color = error;
        }
        else
        {
            tm.color = fail;
        }
    }
}
