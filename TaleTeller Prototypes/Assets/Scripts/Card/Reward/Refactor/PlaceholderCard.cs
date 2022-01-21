using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderCard : MonoBehaviour
{
    public Action onClick;
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

    public void OnPointerEnter()
    {

    }

    public void OnPointerExit()
    {

    }

    public void OnPointerClick()
    {
        if (selected)
            selected = false;
        else selected = true;

        onClick?.Invoke();
    }


}
