using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class SplashScreen : MonoBehaviour
{

    public Image logo;
    [Scene] public int sceneToLoad;
    AsyncOperation sceneLoad;

    public void Start()
    {
        sceneLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        sceneLoad.allowSceneActivation = false;
        FadeLogo();
    }

    public void FadeLogo()
    {
        logo.color = Color.clear;
        LeanTween.value(logo.gameObject, Color.clear, Color.white, 2).setOnUpdate((Color col) =>
        {
            logo.color = col;
        }).setOnComplete(() =>
        {
            LeanTween.value(logo.gameObject, Color.white, Color.clear, 1).setOnUpdate((Color col) =>
            {
                logo.color = col;
            }).setOnComplete(() => sceneLoad.allowSceneActivation = true).setEaseInQuint().setDelay(1);
        });
    }
}
