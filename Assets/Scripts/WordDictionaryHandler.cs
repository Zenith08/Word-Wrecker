using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class WordDictionaryHandler {
    
	public static bool dictionaryReady = false;

	//This needs to be initalized in the code.
	public static Dictionary<char, int> scores = new Dictionary<char, int> ();

    public static readonly string path = "dict_short";

    private static char[] letters;
    private static HashSet<string> dictionary;

    //Oxford API
    //CRITICAL: Move api keys to different file for gitignoring.
    //private static string oxid = Keys.oxford_api_id;
    //private static string oxkey = Keys.oxford_api_key;
    //private static readonly string oxid = "", oxkey = "";
    //private static readonly string PrimeUrl = "https://od-api.oxforddictionaries.com/api/v2/entries/en/";
    //private static readonly string lemmas = "https://od-api.oxforddictionaries.com/api/v2/lemmas/en/";

    public static void initWordsAsync(){
		Thread asyncLoader = new Thread (new ThreadStart(asyncInitalization));
		asyncLoader.Start ();
	}

	public static void asyncInitalization(){
		letters = "aaaaaaaaaabbbccddddeeeeeeeeeeeeeffggggghhhiiiiiiiiiijkklllllmmmmnnnnnnooooooooopppqrrrrrrssssstttttttuuvwwxyyz".ToCharArray ();
		InitScores ();

        //Initalize the text file
        TextAsset pathTxt = (TextAsset)Resources.Load(path, typeof(TextAsset));
        Debug.Log("Read file");
        
        dictionary = new HashSet<string>(pathTxt.text.Split('!'));

        dictionaryReady = true; //Has to happen last
	}

    public static void CheckLocalDB(string word)
    {
        if(!dictionaryReady)
        {
            asyncInitalization();
        }
        //Debug.Log("Checking word " + word.ToLower() + " and getting " + dictionary.Contains(word.ToLower()));
        SendAsyncResult(dictionary.Contains(word.ToLower()));
    }

    private static void SendAsyncResult(bool result)
    {
        GameControler gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControler>();
        gc.validWordCallback(result);
    }
    
    public static char[] getLetters (){
		if (!dictionaryReady) {
			asyncInitalization ();
		}
		return letters;
	}

	public static Dictionary<char, int> getScores(){
		if (!dictionaryReady) {
			asyncInitalization ();
		}
		return scores;
	}

	public static bool isDictionaryReady(){
		return dictionaryReady;
	}

	//Initalizes the scores for all the letters based on scrabble scoring
	private static void InitScores(){
		//A - J
		scores.Add ('a', 1);
		scores.Add ('b', 3);
		scores.Add ('c', 3);
		scores.Add ('d', 2);
		scores.Add ('e', 1);
		scores.Add ('f', 4);
		scores.Add ('g', 2);
		scores.Add ('h', 4);
		scores.Add ('i', 1);
		scores.Add ('j', 8);
		//K - T
		scores.Add ('k', 5);
		scores.Add ('l', 1);
		scores.Add ('m', 3);
		scores.Add ('n', 1);
		scores.Add ('o', 1);
		scores.Add ('p', 3);
		scores.Add ('q', 10);
		scores.Add ('r', 1);
		scores.Add ('s', 1);
		scores.Add ('t', 1);
		//W - Z
		scores.Add ('u', 1);
		scores.Add ('v', 4);
		scores.Add ('w', 4);
		scores.Add ('x', 8);
		scores.Add ('y', 4);
		scores.Add ('z', 10);
	}

    public static void AddWebLoadedWords(string[] words)
    {
        if (!dictionaryReady)
        {
            asyncInitalization();
        }
        for (int i = 0; i < words.Length; i++)
        {
            if(!dictionary.Contains(words[i]) && !words[i].StartsWith("#"))
            {
                //Debug.Log("Adding new word " + words[i]);
                dictionary.Add(words[i]);
            }
        }
    }
}
