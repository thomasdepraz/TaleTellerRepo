using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{
    public Action onClick;
    public Image buttonImage;
    public Image scaledImage;
    public TextMeshProUGUI buttonText;
    public Color baseColor = new Color(0.3f, 0.3f, 0.3f, 1);
    public Color selectedColor;
    public Color pressedColor;
    public Color diabledColor = new Color(1,1,1,0.5f);
    bool _selected;

    public bool noScreenMode;
    public bool highlight;
    [ShowIf("highlight")]public Image highlightImage;
    [ShowIf("highlight")] public Color highlightColor;

    Color buttonTextColor;

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
            LeanTween.cancel(scaledImage.gameObject);
            LeanTween.scale(scaledImage.gameObject, Vector3.one * 1.1f, 0.1f).setEaseInOutQuint();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(interactable)
        {
            if (buttonImage.color == pressedColor) buttonImage.color = selected ? selectedColor : baseColor;

            LeanTween.cancel(scaledImage.gameObject);
            LeanTween.scale(scaledImage.gameObject, Vector3.one, 0.1f).setEaseInOutQuint();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!noScreenMode)
        {
            if(ScreenManager.Instance.currentScreen.open)
            {
                if(interactable)
                {
                    if (selected)
                        selected = false;
                    else selected = true;

                    onClick?.Invoke();
                }
            }
        }
        else
        {
            if (interactable)
            {
                if (selected)
                    selected = false;
                else selected = true;

                onClick?.Invoke();
            }
        }
    }

    public void SetInteractable(bool interactable)
    {
        if (buttonText != null) buttonTextColor = Color.black;
        if(interactable)
        {
            buttonImage.color = baseColor;
            if (buttonText != null) buttonText.color = buttonTextColor;
        }
        else
        {
            buttonImage.color = diabledColor;
            buttonImage.gameObject.transform.localScale = Vector3.one;
            if (buttonText != null) buttonText.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 0.3f);
        }
    }

    public void SelectedFeedback(bool selected)
    {
        if(selected)
        {
            if(highlight)
            {
                if (highlightImage != null)
                {
                    buttonImage.color = baseColor;
                    //CardManager.Instance.cardTweening.ShowHighlight(highlightImage, highlightColor);
                    highlightImage.color = highlightColor;
                }
                else
                {
                    buttonImage.color = selectedColor;
                }
            }
            else
            {
                buttonImage.color = baseColor;
                //buttonImage.gameObject.transform.localScale = Vector3.one;
                
            }
        }
        else
        {
            if(highlight)
            {
                buttonImage.color = baseColor;
                if (highlightImage != null)
                {
                    buttonImage.color = baseColor;
                    //CardManager.Instance.cardTweening.HideHighlight(highlightImage);
                    highlightImage.color = Color.clear;
                }
            }
        }
    }

    public void SetText(string text)
    {
        buttonText.text = text;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(interactable)
        {
            buttonImage.color = pressedColor;
        }
    }
}
