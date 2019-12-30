using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine;

public static class WordDictionaryHandler {

	private static Dictionary<string, bool> words;
	public static bool dictionaryReady = false;
    
	private static char[] letters;

	//This needs to be initalized in the code.
	public static Dictionary<char, int> scores = new Dictionary<char, int> ();

    //Oxford API
    //CRITICAL: Move api keys to different file for gitignoring.
    private static string oxid = Keys.oxford_api_id;
    private static string oxkey = Keys.oxford_api_key;
    private static string PrimeUrl = "https://od-api.oxforddictionaries.com/api/v2/entries/en/";
    private static string lemmas = "https://od-api.oxforddictionaries.com/api/v2/lemmas/en/";

    public static void initWordsAsync(){
		Thread asyncLoader = new Thread (new ThreadStart(asyncInitalization));
		asyncLoader.Start ();
	}

	public static void asyncInitalization(){
		letters = "aaaaaaaaabbccddddeeeeeeeeeeeeffggghhiiiiiiiiijkllllmmnnnnnnooooooooppqrrrrrrssssttttttuuuuvvwwxyyz".ToCharArray ();
		initScores ();

		dictionaryReady = true; //Has to happen last
	}
    
    public static async void checkOxford(string word)
    {
        //Tests the oxford dictionary:
        //Online to oxford Entries
        HttpWebRequest req = null;
        string uri = PrimeUrl + word;

        req = (HttpWebRequest)WebRequest.Create(uri);

        //These are not network credentials, just custom headers
        req.Headers.Add("app_id", oxid);
        req.Headers.Add("app_key", oxkey);

        req.Method = WebRequestMethods.Http.Get;
        req.Accept = "application/json";

        //Tries with lemmas
        string luri = lemmas + word;

        HttpWebRequest lreq = (HttpWebRequest)WebRequest.Create(luri);

        //These are not network credentials, just custom headers
        lreq.Headers.Add("app_id", oxid);
        lreq.Headers.Add("app_key", oxkey);

        lreq.Method = WebRequestMethods.Http.Get;
        lreq.Accept = "application/json";

        try
        {
            await req.GetResponseAsync();
            //Word exists
            sendAsyncResult(true);
        }
        catch (WebException)
        {
            //Word does not exist in entries
            try
            {
                await lreq.GetResponseAsync();
                //Exists in lemmas
                sendAsyncResult(true);
            }
            catch (WebException)
            {
                //Does not exist
                sendAsyncResult(false);
            }
        }
    }

    private static void sendAsyncResult(bool result)
    {
        GameControler gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControler>();
        gc.validWordCallback(result);
    }

    private static bool DoRequestAsync(HttpWebRequest req, string word) { 
        try
        {
            using (HttpWebResponse HWR_Response = (HttpWebResponse)req.GetResponse())
            using (Stream respStream = HWR_Response.GetResponseStream())
            using (StreamReader sr = new StreamReader(respStream, Encoding.UTF8))
            {
               return true;
            }
        }
        catch (WebException)
        {
            //Tries with lemmas
            string luri = lemmas + word;

            req = (HttpWebRequest)WebRequest.Create(luri);

            //These are not network credentials, just custom headers
            req.Headers.Add("app_id", oxid);
            req.Headers.Add("app_key", oxkey);

            req.Method = WebRequestMethods.Http.Get;
            req.Accept = "application/json";

            try
            {
                using (HttpWebResponse HWR_Response = (HttpWebResponse)req.GetResponse())
                using (Stream respStream = HWR_Response.GetResponseStream())
                using (StreamReader sr = new StreamReader(respStream, Encoding.UTF8))
                {
                    return true;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }
        //End oxford stuff
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
	private static void initScores(){
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
}
