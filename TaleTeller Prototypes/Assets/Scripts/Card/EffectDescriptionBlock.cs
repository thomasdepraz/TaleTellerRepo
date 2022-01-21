using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EffectRangeType { Self, Right, Left, AllRight, AllLeft, Hero}

public class EffectDescriptionBlock : MonoBehaviour
{
    public TextMeshProUGUI textField;
    public Image rangeImage;
    public EffectRangeType rangeType;

    public void SetDescription(string description)
    {
        textField.text = description;
    }

    public void SetRangeType(EffectRangeType _rangeType, Sprite icon)
    {
        if (rangeType == _rangeType)
            return;

        var imageAlpha = rangeImage.color.a;
        rangeType = _rangeType;

        if (rangeType == EffectRangeType.Self)
            imageAlpha = 0;
        else
            imageAlpha = 1;

        rangeImage.sprite = icon;
    }
}
