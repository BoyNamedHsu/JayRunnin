using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    public int levelToLoad;

    void Update()
    {
    }

    // Takes index of level and fades into that level
    public void FadeToLevel (int levelIndex)
    {
        levelToLoad = levelIndex;
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
