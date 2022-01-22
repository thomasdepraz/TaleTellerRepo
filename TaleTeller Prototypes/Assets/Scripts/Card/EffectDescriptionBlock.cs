using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EffectRangeType {All, RightAndLeft, Right, Left, AllRight, AllLeft, Hero, Self, Plot ,Other}

public class EffectDescriptionBlock : MonoBehaviour
{
    public TextMeshProUGUI textField;
    public Image rangeImage;
    public EffectRangeType rangeType;

    public void SetDescription(string description, Color color)
    {
        textField.text = description;
        textField.color = color;
    }

    public void SetRangeType(EffectRangeType _rangeType, Sprite icon)
    {
        Color visible = new Color(rangeImage.color.r, rangeImage.color.g, rangeImage.color.b, 1);
        Color transparent= new Color(rangeImage.color.r, rangeImage.color.g, rangeImage.color.b, 0);

        rangeType = _rangeType;

        if (rangeType == EffectRangeType.Other)
            rangeImage.color = transparent;
        else
            rangeImage.color = visible;

        rangeImage.sprite = icon;
    }
}
