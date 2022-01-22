using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
{
    public Action onClick;
    public Image buttonImage;
    public TextMeshProUGUI buttonText;
    public Color baseColor = new Color(0.3f, 0.3f, 0.3f, 1);
    public Color diabledColor = new Color(1,1,1,0.5f);
    bool _selected;
    public bool selected 
    {
        get => _selected;
        set
        {
            _selected = value;

            //TODO add selectFeedback
            SelectedFeedback(_selected);
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
            buttonImage.color = baseColor;
        }
        else
        {
            buttonImage.color = diabledColor;
            buttonImage.gameObject.transform.localScale = Vector3.one;
        }
    }

    public void SelectedFeedback(bool selected)
    {
        if(selected)
        {
            baseColor = buttonImage.color;//TEMP
            buttonImage.color = Color.red;
        }
        else
        {
            buttonImage.color = baseColor;
        }
    }

    public void SetText(string text)
    {
        buttonText.text = text;
    }
}
