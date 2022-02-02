using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public enum GameState {GAME, TUTORIAL}
public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    public Hero currentHero;
    public InstructionsData instructionsData;
    public TutorialManager tutorialManager;
    public GameObject goButton;
    public Image fadePanel;
    public Pointer pointer;
    public Button returnToMenuButton;

    [HideInInspector]public bool pause;
    public GameState currentState = GameState.TUTORIAL;

    public void Awake()
    {
        CreateSingleton(false);
        instructionsData = Instantiate(instructionsData);
    }

    public void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
        {
            if (Time.timeScale > 1)
                Time.timeScale = 1;
            else
                Time.timeScale = 3;
        }
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

    #region GameOver
    public void GameOver(EventQueue queue)
    {
        queue.events.Add(GameOverCoroutine(queue));
    }

    IEnumerator GameOverCoroutine(EventQueue queue)
    {
        //Pause so other routines stop;
        pause = true;
        //Fade To Black
        //NOTE USE SOMETHING ELSE LATER
        Fade(true);
        yield return new WaitForSeconds(1);

        List<EventQueue> events  = new List<EventQueue>();
        events = StoryManager.Instance.queueList;

        //Destroy all event queue and stop all of there coroutines
        while(events.Count > 1)
        {
            for (int i = 0; i < events[0].events.Count; i++)
            {
                StopCoroutine(events[0].events[i]);
            }

            events.RemoveAt(0);
        }


        //Return to Menu Button
        returnToMenuButton.gameObject.SetActive(true);
    }
    #endregion

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }   
}
