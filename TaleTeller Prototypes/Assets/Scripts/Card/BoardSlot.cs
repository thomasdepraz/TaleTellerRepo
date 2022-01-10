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
            CardManager.Instance.board.ShowTargetSlots(CardManager.Instance.currentCard.data);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardManager.Instance.holdingCard)
        {
            if (CardManager.Instance.currentHoveredSlot == this)
                CardManager.Instance.currentHoveredSlot = null;

            CardManager.Instance.board.HideTargetSlots();
        }
    }

    public void ShowHighlight()
    {
        image.color = Color.red;
    }

    public void HideHighlight()
    {
        image.color = Color.white;
    }
}
