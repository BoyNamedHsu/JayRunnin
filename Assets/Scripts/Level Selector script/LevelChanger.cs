using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    private string levelToLoad;

    void Update()
    {
    }

    // Takes index of level and fades into that level
    public void FadeToLevel (string level)
    {
        levelToLoad = level;
        animator.SetTrigger("FadeOut");
    }

    public void ChooseLevel(int level)
    {
        LevelSelector.levelChosen = level;
        SceneManager.LoadScene("Level");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
