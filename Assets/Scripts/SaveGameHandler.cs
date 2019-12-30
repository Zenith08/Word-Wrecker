using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameHandler {

	//For all keys ! indicates the value of difficulty.
	public string SAVE_GAME_USED_KEY = "save-game-exists-!";
	//X and Y are to be replaced with the actual X and Y Values.
	public string GAME_BOARD_KEYS = "game-board-!-x-y";
	public string GAME_SCORE_KEY = "score-!";

	public void saveData(GameControler controler){
		PlayerPrefs.SetInt (SAVE_GAME_USED_KEY.Replace("!", controler.DIFFICULTY.ToString ()), 1);
		PlayerPrefs.SetInt(GAME_SCORE_KEY.Replace("!", controler.DIFFICULTY.ToString ()), controler.getPlayerScore());
		saveBoard (controler.getBoard (), controler.DIFFICULTY);
	}

	private void saveBoard(List<TextScript>[] board, int difficulty){
		for (int x = 0; x < board.Length; x++) {
			for (int y = 0; y < board [0].Count; y++) {
				PlayerPrefs.SetString (GAME_BOARD_KEYS.Replace ("!", difficulty.ToString()).Replace ("x", x.ToString ()).Replace ("y", y.ToString ()), board [x] [y].getLetter ().ToString ());
			}
		}
	}

	public void invalidateSaveGame(int difficulty){
		PlayerPrefs.SetInt(SAVE_GAME_USED_KEY.Replace("!", difficulty.ToString()), 0);
	}

	public bool hasSaveGame(int difficulty){
		int exists = PlayerPrefs.GetInt (SAVE_GAME_USED_KEY.Replace ("!", difficulty.ToString ()));
		if (exists == 1) {
			return true;
		} else {
			return false;
		}
	}

	public char getLetterFor(int difficulty, int x, int y){
		if(hasSaveGame(difficulty)) {
			string savedValue = PlayerPrefs.GetString(GAME_BOARD_KEYS.Replace ("!", difficulty.ToString()).Replace ("x", x.ToString ()).Replace ("y", y.ToString ()));
			return savedValue.ToCharArray()[0];
		}

		return '0';
	}

	public int getScoreFor(int difficulty){
		if (hasSaveGame (difficulty)) {
			return PlayerPrefs.GetInt (GAME_SCORE_KEY.Replace ("!", difficulty.ToString ()));
		} else {
			return 0;
		}
	}
}
