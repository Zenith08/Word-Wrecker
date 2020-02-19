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
    public static readonly int EASY = 0;
    public static readonly int NORMAL = 1;
    public static readonly int HARD = 2;

	public int DIFFICULTY = NORMAL;

	private SaveGameHandler saveGameHandler = new SaveGameHandler(); //Really it's just a convenience class.

	private bool loadSaveGame;

    //Allows letters to light up when tapped (in theory)
    private List<TextScript> highlighted = new List<TextScript>();

    //Allows async api calls.
    public int PendingTimeout = 3 * 30;
    public bool isPendingWord = false;
    private string word;
    private int pendingTime = 0;
    private List<Vector2> lettersUsed;

    private GameData data;

    // Use this for initialization
    void Start () {
		debugText.text = "GS = 0";
		board = new List<TextScript>[9];
		for (int i = 0; i < board.Length; i++) {
			board [i] = new List<TextScript> (15);
		}
        GameObject controller = GameObject.FindWithTag("GameData");

        if(controller != null)
        {
            data = controller.GetComponent<GameData>();
            DIFFICULTY = data.DIFFICULTY;

            if (saveGameHandler.hasSaveGame(DIFFICULTY) && data.LOAD_SAVE)
            {
                loadSaveGame = true;
                score = saveGameHandler.getScoreFor(DIFFICULTY);
                scoreDisplay.text = "Score: " + score;
            }
            else
            {
                loadSaveGame = false;
            }
        }
        else
        {
            loadSaveGame = true;
        }
        
    }
    
	int waitTime = 1;

	// Update is called once per frame
	void Update () {
		if (GAME_STATE == 0) {
			waitTime--;
			if (waitTime == 0) {
				waitTime = 30;
				bool rowCreated = false;
				//Instantiate letters then put them in the array
				for (int i = 0; i < board.Length; i++) {
					if (board [i].Count < 15) {
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
				}
			}
		}else if (GAME_STATE == 1) {
            if (isPendingWord)
            {
                pendingTime--;
                if(pendingTime <= 0)
                {
                    Debug.Log("Failed API Call for " + word);
                    foreach(Vector2 v in lettersUsed)
                    {
                        board[(int)v.x][(int)v.y].setFailiureState(true);
                    }
                    isPendingWord = false;
                }
            }
		}
	}

    public void setLetterHighlighted(int x, int y)
    {
        GetFromBoard(x, y).setHighlighted(true);
        highlighted.Add(GetFromBoard(x, y));
    }

    public void Dehighlight()
    {
        //Unhighlight all letters first
        foreach (TextScript letter in highlighted)
        {
            if(letter != null)
                letter.setHighlighted(false);
        }
        highlighted.Clear();
    }
		
	public TextScript GetFromBoard(int x, int y){
		return board [x] [y];
	}

    //A is the starting position, B is the final position.
    public void collectLetters(int ax, int ay, int bx, int by){
        //Then process
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

            Dehighlight();
            //Now collect letters

            //Get all the letters of the selected word
            do
            {
                word += board[x][y].getLetter();
                lettersUsed.Add(new Vector2(x, y));
                setLetterHighlighted(x, y);

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
            setLetterHighlighted(xFinal, yFinal);
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
            if (!DifficultyCheck()) //If its a more than one letter word we can let the dictionary deal with it.
            {
                //Activate async call
                debugText.text = "PW: " + word;
                isPendingWord = true;
                pendingTime = PendingTimeout;
                //WordDictionaryHandler.CheckJavaDB(word);
                //WordDictionaryHandler.CheckOxford(word);
                WordDictionaryHandler.CheckLocalDB(word);
            }
        }
	}

    //True if the word was handled by difficulty whether or not it was allowed
    private bool DifficultyCheck()
    {
        if (lettersUsed.Count == 1) //1 letter words are only allowed on easy difficulty and even then only "a" and "I"
        {
            if (DIFFICULTY > EASY) //Easy = 0 so > EASY makes sense.
            {
                //Don't allow word because 1 letter words are not allowed on difficulty Normal or higher
                RejectWord();
                return true;
            }
            else
            {
                if (word.Equals("A") || word.Equals("I")) //These are the only actual 1 letter words so we can just allow them.
                {
                    debugText.text = "PW: " + word; //Also for the callback to make sense.
                    isPendingWord = true;
                    pendingTime = PendingTimeout;
                    validWordCallback(true);
                    return true;
                }
            }
        }
        else if (lettersUsed.Count == 2)
        {
            if(DIFFICULTY <= NORMAL)
            {
                return false;
            }
            else
            {
                RejectWord(); //So on hard 2 letter words are rejected.
                return true;
            }
        }
        return false;
    }

    private void RejectWord()
    {
        debugText.text = "PW: " + word; //This is needed so that the callback understands what it's doing
        isPendingWord = true;
        pendingTime = PendingTimeout;
        validWordCallback(false);
    }

    public void validWordCallback(bool validWord)
    {
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
                    //Does not remove from highlighted list, definatly going to be an issue.
                    Destroy(destroyGO);
                }
            }
            else
            { //The normal loop
                for (int i = 0; i < lettersUsed.Count; i++)
                {
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
            foreach(Vector2 v in lettersUsed)
            {
                board[(int)v.x][(int)v.y].setFailiureState(false);
            }
        }
    }

	public List<TextScript>[] getBoard(){
		return board;
	}

	public int getPlayerScore(){
		return score;
	}
}
