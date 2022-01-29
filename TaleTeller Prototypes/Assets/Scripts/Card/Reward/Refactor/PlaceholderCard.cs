using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderCard : MonoBehaviour
{
    public CardContainer container;
    public Action onClick;
    bool _selected;
    public bool selected
    {
        get => _selected;
        set
        {
            _selected = value;

            SelectFeedback(_selected);
        }
    }

    public void OnPointerEnter()
    {
        LeanTween.cancel(gameObject);
        gameObject.transform.localScale = Vector3.one;
        LeanTween.rotate(gameObject, Vector3.zero, 0.1f).setEaseOutQuint();
        LeanTween.scale(gameObject, Vector3.one * 1.5f, 0.2f).setEaseOutQuint();

        gameObject.transform.SetAsLastSibling();
    }

    public void OnPointerExit()
    {
        LeanTween.cancel(gameObject);
        gameObject.transform.rotation = Quaternion.identity;
        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseOutQuint();
    }

    public void OnPointerClick()
    {
        if(ScreenManager.Instance.currentScreen.open)
        {
            if (selected)
                selected = false;
            else selected = true;

            onClick?.Invoke();
        }
    }

    public void SelectFeedback(bool selected)
    {
        if(selected)
        {
            CardManager.Instance.cardTweening.ShowHighlight(container.visuals.cardHighlight, container.visuals.profile.highlightColor);
        }
        else
        {
            CardManager.Instance.cardTweening.HideHighlight(container.visuals.cardHighlight);
        }
    }


}
