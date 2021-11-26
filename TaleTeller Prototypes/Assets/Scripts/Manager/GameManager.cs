using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    public Hero currentHero;
    public StoryManager storyManager;
    public CreativityManager creativityManager;
    public GameObject goButton;
    public Image fadePanel;
    public GameObject gameOverText;
    public Pointer pointer;

    public void Awake()
    {
        CreateSingleton(false);
    }

    public void Fade(bool toBlack)
    {
        Color transparent = new Color(0,0,0,0);
        if (toBlack)
        {
            LeanTween.color(gameObject, Color.black, 0.5f).setOnUpdate((Color col) => { fadePanel.color = col; });
        }
        else
        {
            LeanTween.value(gameObject, Color.black, transparent, 1f).setOnUpdate((Color col) => {fadePanel.color = col;});
        }
    }

    public void Update()
    {
        if(gameOverText.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);

        }

    }

    #region GameOver
    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        Fade(true);
        yield return new WaitForSeconds(0.8f);
        gameOverText.SetActive(true);
    }
    
    
    #endregion
}
