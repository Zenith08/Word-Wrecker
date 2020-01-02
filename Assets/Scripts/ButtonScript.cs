using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

	public int y;

	public int getX(){
		return GetComponentInParent<ButtonHolder> ().getX();
	}

	public int getY(){
		return y;
	}

    public void setSelected()
    {
        GameControler gc = GameObject.FindWithTag("GameController").GetComponent<GameControler>();
        gc.setLetterHighlighted(getX(), getY());
    }
}
