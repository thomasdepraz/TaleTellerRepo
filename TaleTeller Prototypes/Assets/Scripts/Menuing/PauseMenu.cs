using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PauseMenu : MonoBehaviour
{

    [Header("References")] 
    public RectTransform panelTransform;
    public List<RectTransform> buttonTransforms = new List<RectTransform>();

    private bool menuOpened;

    public void Click(RectTransform buttonSelf)
    {
        //openMen
        if(!menuOpened)
            OpenMenu();
    }

    public void PointerEnter(RectTransform buttonSelf)
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, 1f, 1.3f, 0.2f).setOnUpdate((float value) => 
        {
            buttonSelf.localScale =new Vector3(value, value, 1);

        }).setEaseInOutQuint();
    }

    public void PointerExit(RectTransform buttonSelf)
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, buttonSelf.localScale.x, 1, 0.2f).setOnUpdate((float value) =>
        {
            buttonSelf.localScale = new Vector3(value, value, 1);

        }).setEaseInOutQuint();
    }

    void OpenMenu()
    {
        menuOpened = true;
        GameManager.Instance.pause = true;
        LeanTween.value(panelTransform.gameObject, 1, 0, 0.5f).setOnUpdate((float value) =>
        {
            panelTransform.anchorMin = new Vector2(0, value);

        }).setEaseInOutQuint().setOnStart(() =>
        {
            panelTransform.gameObject.SetActive(true);
        }).setOnComplete(()=> 
        {
            for (int i = 0; i < buttonTransforms.Count; i++)
            {
                ShowButton(i);
            }
        });
    }

    void ShowButton(int index)
    {
        LeanTween.value(buttonTransforms[index].gameObject, 0f, 1f, 0.2f).setOnUpdate((float value) =>
        {
            buttonTransforms[index].localScale = new Vector3(value, value, 0);
        }).setDelay(index * 0.2f).setEaseInOutQuint();
    }

    void CloseMenu()
    {
        GameManager.Instance.pause = false;
        LeanTween.value(panelTransform.gameObject, 0, 1, 0.5f).setOnUpdate((float value) =>
        {
            panelTransform.anchorMin = new Vector2(0, value);

        }).setEaseInOutQuint().setOnComplete(()=> 
        {
            panelTransform.gameObject.SetActive(false);
            for (int i = 0; i < buttonTransforms.Count; i++)
            {
                buttonTransforms[i].localScale = Vector3.zero;
            }
            menuOpened = false;
        });
    }

    public void Resume()
    {
        CloseMenu();
    }

    public void ReturnToMenu()
    {
        GameManager.Instance.LoadScene("TitleScreenScene");
    }

    
}
