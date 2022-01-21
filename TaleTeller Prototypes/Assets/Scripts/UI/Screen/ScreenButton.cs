using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
{
    public Action onClick;
    public Image buttonImage;

    bool _selected;
    public bool selected 
    {
        get => _selected;
        set
        {
            _selected = value;

            //TODO add selectFeedback
        }
    }

    bool _interactable = true;
    public bool interactable
    {
        get => _interactable;
        set
        {
            _interactable = value;

            SetInteractable(_interactable);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(interactable)
        {
            LeanTween.cancel(buttonImage.gameObject);
            LeanTween.scale(buttonImage.gameObject, Vector3.one * 1.1f, 0.1f).setEaseInOutQuint();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(interactable)
        {
            LeanTween.cancel(buttonImage.gameObject);
            LeanTween.scale(buttonImage.gameObject, Vector3.one, 0.1f).setEaseInOutQuint();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(interactable)
        {
            if (selected)
                selected = false;
            else selected = true;

            onClick?.Invoke();
        }
    }

    public void SetInteractable(bool interactable)
    {
        if(interactable)
        {

        }
        else
        {
            buttonImage.gameObject.transform.localScale = Vector3.one;
        }
    }
}
