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
            CardManager.Instance.board.boardUpdate?.Invoke();

        }        
    }
    public CanvasGroup canvasGroup;

    [Header("Sprites")]
    public Image image;
    public Image highlightImage;
    public Sprite defaultSprite;
    public Sprite hoveredSprite;

    Color highlightColor;

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
        if (highlightColor == Color.clear) highlightColor = CardManager.Instance.board.baseColor;
        else if( highlightColor == CardManager.Instance.board.baseColor) highlightColor = CardManager.Instance.board.TwoEffectColor;
        else if (highlightColor == CardManager.Instance.board.TwoEffectColor) highlightColor = CardManager.Instance.board.ThreeEffectColor;
        CardManager.Instance.cardTweening.ShowHighlight(highlightImage, highlightColor);
    }

    public void HideHighlight()
    {
        CardManager.Instance.cardTweening.HideHighlight(highlightImage);
        highlightColor = Color.clear;
    }
}
