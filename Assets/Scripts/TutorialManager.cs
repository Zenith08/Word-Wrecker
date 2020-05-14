using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public List<Sprite> pages;

    private int currentPage = 0;
    private Image display;
    // Start is called before the first frame update
    void Start()
    {
        display = GetComponent<Image>();
    }

    public void MoveForward()
    {
        currentPage++;
        ProcessPageChange();
    }

    public void MoveBackwards()
    {
        currentPage--;
        ProcessPageChange();
    }

    private void ProcessPageChange()
    {
        if (ShouldLevelChange())
        {
            ReturnToMenu();
        }
        else
        {
            display.sprite = pages[currentPage];
        }
    }

    private bool ShouldLevelChange()
    {
        return currentPage < 0 || currentPage >= pages.Count;
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenue", LoadSceneMode.Single);
    }
}
