using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenue : MonoBehaviour
{
    public void ReturnToMenue()
    {
        SceneManager.LoadScene("MainMenue", LoadSceneMode.Single);
    }
}
