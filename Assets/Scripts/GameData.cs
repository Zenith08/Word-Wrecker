using UnityEngine;

public class GameData : MonoBehaviour
{
    public int DIFFICULTY = 0;
    public bool LOAD_SAVE = false;

    // Start is called before the first frame update
    void Awake()
    {
        if(GameObject.FindGameObjectWithTag("GameData") != this.gameObject)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
