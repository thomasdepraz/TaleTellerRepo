using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardSlot : MonoBehaviour
     , IPointerEnterHandler
     , IPointerExitHandler
{
    CardContainer _currentPlacedCard;
    public int slotIndex;
    public CardContainer currentPlacedCard
    { 
        get => _currentPlacedCard;
        set
        {
            _currentPlacedCard = value;
            if(value == null)
            {
                image.sprite = defaultSprite;
            }
            else
            {
                image.sprite = hoveredSprite;
            }
        }        
    }
    public CanvasGroup canvasGroup;

    [Header("Sprites")]
    public Image image;
    public Sprite defaultSprite;
    public Sprite hoveredSprite;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CardManager.Instance.holdingCard)
        {
            CardManager.Instance.currentHoveredSlot = this;
        }

        image.sprite = hoveredSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = defaultSprite;

        if (CardManager.Instance.holdingCard)
        {
            if (CardManager.Instance.currentHoveredSlot == this)
                CardManager.Instance.currentHoveredSlot = null;
        }
    }
}
