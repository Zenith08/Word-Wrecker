using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Text;
using UnityEngine;
using System.Net.Sockets;

public static class WordDictionaryHandler {
    
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

    /*
    public static void CheckLocalDB(string word)
    {
        string con = "Driver={Words};";
        //string con = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = D:/UnityEngine/HelloSql.accdb; Persist Security Info = False;";
        Debug.Log(con);
        OdbcConnection oCon = new OdbcConnection(con);
        try
        {
            oCon.Open();
            string query = "SELECT * FROM scrabble";
            OdbcCommand command = new OdbcCommand(query);
            command.Connection = oCon;
            command.ExecuteNonQuery();
            oCon.Close();
        }
        catch(Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
            if(oCon.State != System.Data.ConnectionState.Closed)
            {
                oCon.Close();
            }
            oCon.Dispose();
        }
        SendAsyncResult(false);
    }
    */

    public static async void CheckJavaDB(string word)
    {
        Debug.Log("Checking word " + word);
        string check = word.ToLower();

        var client = new UdpClient();
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.1.9"), 4445); // endpoint where server is listening
        client.Connect(ep);

        // send data
        byte[] bts = Encoding.ASCII.GetBytes(check);
        client.Send(bts, bts.Length);
        Debug.Log("Sent request");
        // then receive data
        UdpReceiveResult receivedData = await client.ReceiveAsync();

        string res = Encoding.ASCII.GetString(receivedData.Buffer);
        if(res == "true")
        {
            SendAsyncResult(true);
        }
        else
        {
            SendAsyncResult(false);
        }

        Debug.Log("receive data from " + ep.ToString() + " saying " + receivedData.ToString() + " or " + Encoding.ASCII.GetString(receivedData.Buffer));
    }

    public static async void CheckOxford(string word)
    {
        string check = word.ToLower();
        //Tests the oxford dictionary:
        //Online to oxford Entries
        HttpWebRequest req = null;
        string uri = PrimeUrl + check;

        req = (HttpWebRequest)WebRequest.Create(uri);

        //These are not network credentials, just custom headers
        req.Headers.Add("app_id", oxid);
        req.Headers.Add("app_key", oxkey);

        req.Method = WebRequestMethods.Http.Get;
        req.Accept = "application/json";

        //Tries with lemmas
        string luri = lemmas + check;

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
            SendAsyncResult(true);
        }
        catch (WebException)
        {
            //Word does not exist in entries
            try
            {
                await lreq.GetResponseAsync();
                //Exists in lemmas
                SendAsyncResult(true);
            }
            catch (WebException)
            {
                //Does not exist
                SendAsyncResult(false);
            }
        }
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
