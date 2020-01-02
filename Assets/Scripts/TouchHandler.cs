using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHandler : MonoBehaviour {
	
	private Vector2 startTouch;
	private int startX, startY, endX, endY;
	private Vector2 endTouch;
	
	// Update is called once per frame
	void LateUpdate () {
		if (GetComponent<GameControler> ().GAME_STATE == 1) {
			if (Input.touchCount == 1) {
				Touch touch = Input.GetTouch (0);
				RaycastHit touchedObject;
				if (Physics.Raycast (Camera.main.ScreenPointToRay (new Vector3 (touch.position.x, touch.position.y, 0)), out touchedObject, 200F)) {
					if (touchedObject.collider != null) {
						if (touchedObject.collider.gameObject.tag == "Button") {
                            ButtonScript button = touchedObject.collider.gameObject.GetComponent<ButtonScript>();

                            if (!GetComponent<GameControler>().isPendingWord)
                            {    
                                button.setSelected();
                            }

							if (touch.phase == TouchPhase.Began) {
								startTouch = touch.position;
								startX = button.getX ();
								startY = button.getY ();
							} else if (touch.phase == TouchPhase.Ended) {
								endTouch = touch.position;
								endX = button.getX ();
								endY = button.getY ();

								GetComponent<GameControler> ().collectLetters (startX, startY, endX, endY);
							} else {
					
							}
						}
					}
				}
			}
		}
	}

	void validateBounds(){
		if (startX == 9) {
			startX -= 1;
		}
		if (startY == 16) {
			startY -= 1;
		}
		if (endX == 9) {
			endX -= 1;
		}
		if (endY == 16) {
			endY -= 1;
		}
	}

	int chopDecimals(float start){
		int ret = 0;
		//		Debug.Log ("Chop Decimals starting with " + start);
		for (double d = start; d > 1; d--) {
			ret++;
		}
		//		Debug.Log ("Chop Decimals ending with " + ret);
		return ret;
	}
		
}
