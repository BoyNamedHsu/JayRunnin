using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    public Animator menuAnimator;
    public GameObject image;
    private static string levelToLoad;

    // Takes index of level and fades into that level
    public void FadeToLevel (string level)
    {
        levelToLoad = level;
        animator.SetTrigger("FadeOut");
    }

    public void ChooseLevel(int level)
    {

        print(level);
        if (level <= Unlocker.GetHighestUnlockedLevel())
        {
            LevelSelector.levelChosen = level;
            print(LevelSelector.levelChosen);
            SceneManager.LoadScene("Level");
        }
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void Ending()
    {
        SceneManager.LoadScene("Ending");
    }

    public void MenuWoopWoop()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PanUp()
    {
        image.SetActive(false);
        menuAnimator.Play("PanUpMenu");
    }
}
