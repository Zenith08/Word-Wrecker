using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScript : MonoBehaviour {

	public int speed = 5;

	private TextMesh displayText;
	private char characterValue;
	private Rigidbody rb;
	private bool falling = false;
	public byte fallTimer = 2;

	// Use this for initialization
	void Start () {
		displayText = GetComponent<TextMesh> ();
		rb = GetComponent<Rigidbody> ();
		falling = true;
		//TEMP
//		setLetter('a');
	}
		
	// Update is called once per frame
	void Update () {
		if (displayText.text != characterValue.ToString ()) {
			displayText.text = characterValue.ToString ();
		}
	}

	void FixedUpdate(){
	//	Debug.Log ("Falling = " + falling);
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
	//	Debug.Log ("On Trigger Enter");
		if (other.tag == "Letter" || other.tag == "Ground") {
			falling = false;
		}
	}

	void OnTriggerExit(){
	//	Debug.Log ("On Trigger Exit");
		falling = true;
	}

	public void setLetter(char letter){
		//Shouldn't need ToUpper(char) later on but it is a good catcher
		characterValue = char.ToUpper(letter);
	}

	public char getLetter(){
		return characterValue;
	}
}
