using UnityEngine;

public class TouchHandler : MonoBehaviour {
	
	private Vector2 startTouch;
    private Vector3 startButton;
	private int startX, startY, endX, endY, lastX = -1, lastY = -1;
	private Vector2 endTouch;

    private GameControler gc;

    private void Start()
    {
        gc = GetComponent<GameControler>();
    }

    // Update is called once per frame
    void LateUpdate () {
		if (gc.GAME_STATE == 1) {
			if (Input.touchCount == 1) {
				Touch touch = Input.GetTouch (0);
				RaycastHit touchedObject;
				if (Physics.Raycast (Camera.main.ScreenPointToRay (new Vector3 (touch.position.x, touch.position.y, 0)), out touchedObject, 200F)) {
					if (touchedObject.collider != null) {
						if (touchedObject.collider.gameObject.tag == "Button") {
                            ButtonScript button = touchedObject.collider.gameObject.GetComponent<ButtonScript>();
                            int buttonX = button.getX();
                            int buttonY = button.getY();

                            if (!gc.isPendingWord)
                            {    
                                button.setSelected();
                            }

							if (touch.phase == TouchPhase.Began) {
                                startButton = button.transform.position;
								startTouch = touch.position;
                                startX = buttonX;
								startY = buttonY;
							} else {
                                //Draws a debug line of the current selection by calculating positions
                                Debug.DrawLine(startButton, button.transform.position, Color.red);
                                //Have to calculate which grid position it is before this is called!
                                int deltaX = Mathf.Abs(buttonX - startX);
                                int deltaY = Mathf.Abs(buttonY - startY);

                                int yFinal, xFinal;

                                if (deltaX == deltaY)
                                { //Diagonal Line (45 Degrees)
                                    xFinal = buttonX;
                                    yFinal = buttonY;
                                }
                                else
                                { //Not Diagonal
                                    if (deltaY < deltaX)
                                    { //The smaller of the two is considered an annomoly and is set to an original value to create a perfectly horizontal or vertical line.
                                        yFinal = startY; //Sets the y (smaller) value to be that of the start
                                        xFinal = buttonX; //Sets the x value to be the final position
                                    }
                                    else
                                    {
                                        xFinal = startX; //Sets the x (smaller) value to be that of the start
                                        yFinal = buttonY; //Sets the y value to be the final position
                                    }
                                }
                                
                                Debug.DrawLine(gc.GetFromBoard(startX, startY).gameObject.transform.position,
                                    gc.GetFromBoard(xFinal, yFinal).gameObject.transform.position, Color.green);

                                lastX = xFinal;
                                lastY = yFinal;

                                //This makes sure only the current selection is highlighted
                                int x = startX;
                                int y = startY;
                                int nADeltaX = xFinal - startX;
                                int nADeltaY = yFinal - startY;

                                gc.Dehighlight();
                                //Now collect letters

                                //Get all the letters of the selected word
                                do
                                {
                                    gc.setLetterHighlighted(x, y);

                                    if (nADeltaX > 0)
                                    {
                                        x++;
                                        nADeltaX--;
                                    }
                                    else if (nADeltaX < 0)
                                    {
                                        x--;
                                        nADeltaX++;
                                    }

                                    if (nADeltaY > 0)
                                    {
                                        y++;
                                        nADeltaY--;
                                    }
                                    else if (nADeltaY < 0)
                                    {
                                        y--;
                                        nADeltaY++;
                                    }
                                } while (nADeltaX != 0 || nADeltaY != 0);
                                //Final letter bugfix
                                gc.setLetterHighlighted(xFinal, yFinal);
                            }
						}
					}
				}
			}
            else if(Input.touchCount == 0)
            {
                //Debug.Log("Touch = 0");
                //Something about last touch
                if(lastX >= 0 && lastY >= 0)
                {
                    Debug.Log("LastX and Y bigger");
                    gc.collectLetters(startX, startY, lastX, lastY);
                    //Then make sure nothing happens
                    lastX = -1;
                    lastY = -1;
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
