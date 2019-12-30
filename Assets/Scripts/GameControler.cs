using System.Collections.Generic;
using UnityEngine;

public class GameControler : MonoBehaviour {
	//The board of the game, a 2d array... sort of.
	private List<TextScript>[] board;

	//The places the letters start off.
	public GameObject[] startPoints = new GameObject[9];

	public GameObject letterRef;

	//Temp for testing
	public TextMesh debugText;

	//The Score Display
	public TextMesh scoreDisplay;

	private int score = 0;

	//State 0 = Starting, State 1 = Playable, State 2 = somthing else.
	public int GAME_STATE = 0;

	//Difficulty 0 = Easy, 1 = Normal, 2 = Hard, 3 = Lunatic, 4 = Somthing else.
	public int DIFFICULTY = 0;

	private SaveGameHandler saveGameHandler = new SaveGameHandler(); //Really it's just a convenience class.

	private bool loadSaveGame;

    //Allows async api calls.
    bool isPendingWord = false;
    string word;
    List<Vector2> lettersUsed;

    // Use this for initialization
    void Start () {
		debugText.text = "GS = 0";
		board = new List<TextScript>[9];
		for (int i = 0; i < board.Length; i++) {
			board [i] = new List<TextScript> (15);
		}

		loadSaveGame = saveGameHandler.hasSaveGame (DIFFICULTY);
		if (loadSaveGame) {
			score = saveGameHandler.getScoreFor (DIFFICULTY);
			scoreDisplay.text = "Score: " + score;
		}
    }

//	bool rowCreated = false;
	int waitTime = 1;

	// Update is called once per frame
	void Update () {
		if (GAME_STATE == 0) {
            //			Debug.Log ("GAME_STATE = 0");
            //debugText.text = "GS = 0";
			waitTime--;
	//			Debug.Log ("Wait Time = " + waitTime);
			if (waitTime == 0) {
	//				Debug.Log ("Wait Time = 0");
				waitTime = 30;
				bool rowCreated = false;
				//Instantiate letters then put them in the array
				for (int i = 0; i < board.Length; i++) {
					if (board [i].Count < 15) {
//					debugText.text = "Board Width = " + board.Length;
						GameObject nextLetter = Instantiate (letterRef);
						TextScript ts = nextLetter.GetComponent<TextScript> ();
						//Handles Save Game stuff
						if (loadSaveGame) {
							ts.setLetter (saveGameHandler.getLetterFor(DIFFICULTY, i, board[i].Count)); //The letter will be added after this is called so use Count with no modifiers.
						} else { //If Load Save Game is false it means we are in normal gameplay and can use normal letter distribution.
							ts.setLetter (WordDictionaryHandler.getLetters() [Random.Range (0, WordDictionaryHandler.getLetters().Length)]);
						}
						nextLetter.transform.SetPositionAndRotation (startPoints [i].transform.position, startPoints [i].transform.rotation); 
						//					nextLetter.GetComponent<Rigidbody>().position = startPoints [i].transform.position;
						board [i].Add (ts);
						rowCreated = true;
					}
				}
				if (rowCreated == false) {
					loadSaveGame = false;
					saveGameHandler.saveData (this);
					GAME_STATE = 1;
					//debugText.text = "GS=1";
				}
			}
		}else if (GAME_STATE == 1) {
            //debugText.text = "GS = 1";
		}
	}
		
	private TextScript getFromBoard(int x, int y){
		return board [x] [y];
	}

    //A is the starting position, B is the final position.
    public void collectLetters(int ax, int ay, int bx, int by){
        if (!isPendingWord)
        {
            //Have to calculate which grid position it is before this is called!
            int deltaX = Mathf.Abs(bx - ax);
            int deltaY = Mathf.Abs(by - ay);

            int yFinal, xFinal;

            if (deltaX == deltaY)
            { //Diagonal Line (45 Degrees)
                xFinal = bx;
                yFinal = by;
            }
            else
            { //Not Diagonal
                if (deltaY < deltaX)
                { //The smaller of the two is considered an annomoly and is set to an original value to create a perfectly horizontal or vertical line.
                    yFinal = ay; //Sets the y (smaller) value to be that of the start
                    xFinal = bx; //Sets the x value to be the final position
                }
                else
                {
                    xFinal = ax; //Sets the x (smaller) value to be that of the start
                    yFinal = by; //Sets the y value to be the final position
                }
            }

            word = "";
            lettersUsed = new List<Vector2>();
            int x = ax;
            int y = ay;
            int nADeltaX = xFinal - ax;
            int nADeltaY = yFinal - ay;

            //Get all the letters of the selected word
            do
            {
                word += board[x][y].getLetter();
                lettersUsed.Add(new Vector2(x, y));

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
            //Adds the final letter into the word because otherwise it wouldn't be added in properly
            word += board[xFinal][yFinal].getLetter();
            lettersUsed.Add(new Vector2(xFinal, yFinal));
            //End last letter bug fix

            //If the person only selected one letter this will fix the existing bug
            if (lettersUsed.Count == 2)
            {
                if (lettersUsed[0].Equals(lettersUsed[1]))
                {
                    word = board[(int)lettersUsed[0].x][(int)lettersUsed[0].y].getLetter() + "";
                    lettersUsed.RemoveAt(1);
                }
            }
            //End one letter bug fix

            //Activate async call
            WordDictionaryHandler.checkOxford(word);
            debugText.text = "PW: " + word;
            isPendingWord = true;
        }
	}

    public void validWordCallback(bool validWord)
    {
        //TODO remove check for original dictionary
        if (isPendingWord && validWord)
        { //If the word is valid then score the points and destroy the letters
            debugText.text = "V " + word;
            //Score the word
            char[] letters = word.ToLower().ToCharArray();
            foreach (char c in letters)
            {
                score += WordDictionaryHandler.getScores()[c];
            }
            scoreDisplay.text = "Score: " + score;
            //Then destroy the letters
            //If the word goes up then go backwards
            //TODO Still Broken, maybe not
            Debug.Log("LettersUsed.Count = " + lettersUsed.Count);

            if (lettersUsed[lettersUsed.Count - 1].y < lettersUsed[0].y)
            { //The backwards loop
                for (int i = 0; i < lettersUsed.Count; i++)
                {
                    Debug.Log("D L Loop " + i + " address " + i);
                    Debug.Log("Value: " + lettersUsed[i].x + ", " + lettersUsed[i].y);

                    int destroyX = (int)lettersUsed[i].x;
                    int destroyY = (int)lettersUsed[i].y;

                    GameObject destroyGO = board[destroyX][destroyY].gameObject;
                    board[destroyX].RemoveAt(destroyY);
                    Destroy(destroyGO);
                }
            }
            else
            { //The normal loop
                for (int i = 0; i < lettersUsed.Count; i++)
                {
                    Debug.Log("D L Loop " + i + " address " + (lettersUsed.Count - 1 - i));
                    Debug.Log("Value: " + lettersUsed[lettersUsed.Count - 1 - i].x + ", " + lettersUsed[lettersUsed.Count - 1 - i].y);

                    int destroyX = (int)lettersUsed[lettersUsed.Count - 1 - i].x;
                    int destroyY = (int)lettersUsed[lettersUsed.Count - 1 - i].y;

                    GameObject destroyGO = board[destroyX][destroyY].gameObject;
                    board[destroyX].RemoveAt(destroyY);
                    Destroy(destroyGO);
                }
            }
            isPendingWord = false;
            GAME_STATE = 0;
        }
        else
        {
            isPendingWord = false;
            debugText.text = "F " + word;
        }
    }

	public List<TextScript>[] getBoard(){
		return board;
	}

	public int getPlayerScore(){
		return score;
	}
}
