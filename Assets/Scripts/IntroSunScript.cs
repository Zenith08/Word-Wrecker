using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSunScript : MonoBehaviour {

	public int speed = 45;
    public string nextLevel = "MainMenue";

	// Use this for initialization
	void Start () {
		WordDictionaryHandler.initWordsAsync ();
	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log (transform.eulerAngles.x);
		if (transform.eulerAngles.x < 32 && transform.eulerAngles.x > 30) {
			if(WordDictionaryHandler.isDictionaryReady ()){
				SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
			}
		}else{
			transform.Rotate (Vector3.right * Time.deltaTime * speed);
		}
	}
}
