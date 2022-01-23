using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandSlot : MonoBehaviour
{
    public RectTransform self;
    public CardContainer currentPlacedCard;
    public CanvasGroup canvasGroup;

    public void OnPointerEnter()
    {
        if(CardManager.Instance.holdingCard)
        {
            CardManager.Instance.currentHoveredHandSlot = this;
        }
    }

    public void OnPointerExit()
    {
        if (CardManager.Instance.holdingCard)
        {
            if (CardManager.Instance.currentHoveredHandSlot == this)
                CardManager.Instance.currentHoveredHandSlot = null;
        }
    }


}
