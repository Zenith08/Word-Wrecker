using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MenueManager : MonoBehaviour
{
    private GameData gd;

    public RectTransform mainMenue;
    public RectTransform difficultySelect;
    public int speed = 10;

    public bool inMenue = true; //True means Main Menue false means difficulty select
    
    private Vector3 right;
    private Vector3 center;
    private Vector3 left;
    

    // Start is called before the first frame update
    void Start()
    {
        gd = GameObject.FindWithTag("GameData").GetComponent<GameData>();

        right = difficultySelect.position;
        center = mainMenue.position;
        left = new Vector3(right.x * -1, right.y);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (inMenue)
        {
            mainMenue.Translate(Vector3.MoveTowards(mainMenue.position, center, speed) - mainMenue.position, Space.World);
            difficultySelect.Translate(Vector3.MoveTowards(difficultySelect.position, right, speed) - difficultySelect.position, Space.World);
        }
        else
        {
            mainMenue.Translate(Vector3.MoveTowards(mainMenue.position, left, speed) - mainMenue.position, Space.World);
            difficultySelect.Translate(Vector3.MoveTowards(difficultySelect.position, center, speed) - difficultySelect.position, Space.World);
        }
    }

    public void GoToDifficulty()
    {
        //Debug.Log("Difficulty with " + gd.LOAD_SAVE);
        inMenue = false;
    }

    public void GoToMenue()
    {
        inMenue = true;
    }

    public void StartGameplay()
    {
        SceneManager.LoadScene("MWSMainGameplay", LoadSceneMode.Single);
    }

    public void SetNewGame()
    {
        gd.LOAD_SAVE = false;
        GoToDifficulty();
    }

    public void SetContinueGame()
    {
        gd.LOAD_SAVE = true;
        GoToDifficulty();
    }

    public void TutorialMode()
    {
        SceneManager.LoadScene("TutorialLevel", LoadSceneMode.Single);
    }

    public void EasyMode()
    {
        gd.DIFFICULTY = 0;
        StartGameplay();
    }

    public void NormalMode()
    {
        gd.DIFFICULTY = 1;
        StartGameplay();
    }

    public void HardMode()
    {
        gd.DIFFICULTY = 2;
        StartGameplay();
    }

    public void GoBack()
    {
        GoToMenue();
    }
}
