using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Collections.Generic.Dictionary<string, string>;

public class CardContainer : MonoBehaviour
{
    [Header("References")]
    public RectTransform rectTransform;
    public Image selfImage;
    private Transform targetTransform;
    private Vector3 origin;
    private Vector3 basePosition;

    public BoardSlot currentSlot;
    public HandSlot currentHandSlot;
    public CardData data;
    public CardVisuals visuals;
    public List<CardTooltip> tooltips = new List<CardTooltip>();

    public RectTransform shadowTransform;

    delegate void UIEvent();

    private Vector2 originPosition;
    private int siblingIndex;

    //Variables for dragging management
    public CanvasGroup canvasGroup;
    private bool isDragging;
    private bool canTween = true;

    [HideInInspector]public AudioSource audioSource;
    
    private void Update()
    {
        if(isDragging && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
        {
            isDragging = false;
            canvasGroup.blocksRaycasts = true;
        }
        if(isDragging)
        {
            //follow 
            shadowTransform.rotation = new Quaternion(0,0,0,1);
            rectTransform.position = targetTransform.position;
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, new Quaternion(CardManager.Instance.pointerRef.pointerDirection.y*0.3f, -CardManager.Instance.pointerRef.pointerDirection.x*0.3f, 0, 1), Time.deltaTime);
            shadowTransform.localScale = new Vector3(1f - Mathf.Abs(rectTransform.rotation.y*0.5f), 1f -  Mathf.Abs(rectTransform.rotation.x*0.5f), 1);
        }
    }

    #region Init/Reset
    public void InitializeContainer(CardData data, bool isPlaceHolder = false)
    {
        this.data = data;

        if (data == null)
        {
            return;
        }
        //load Data and activate gameobject
        visuals.InitializeVisuals(data);

        basePosition = transform.position;

        if(!isPlaceHolder)
            data.currentContainer = this;


        gameObject.SetActive(true);
        originPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);

        //add random rotation 
        rectTransform.rotation = new Quaternion(0,0,UnityEngine.Random.Range(-0.1f,0.1f),1);

        //set scale to zero
        //rectTransform.localScale = Vector3.zero; NOTE NOT SUPPOSED TO BE COMMENTED
    }
    public void ResetContainer(bool isPlaceholder = false)
    {
        //unload Data and deactivate gameobject
        if(currentSlot != null)
        {
            currentSlot.currentPlacedCard = null;
            currentSlot.canvasGroup.blocksRaycasts = true;
            currentSlot = null;
        }

        if(currentHandSlot != null)
        {
            if(currentHandSlot.currentPlacedCard == this)
            {
                currentHandSlot.currentPlacedCard = null;
                currentHandSlot.canvasGroup.blocksRaycasts = true;
            }

            currentHandSlot = null;
        }

        if(!isPlaceholder)
        {
            data.currentContainer = null;
        }
        data = null;


        //Reset to hidden hand

        if(!isPlaceholder)
        {
            rectTransform.position = basePosition;
            CardManager.Instance.cardHand.currentHand.Remove(this);
            CardManager.Instance.UpdateHandCount();
            transform.SetParent(CardManager.Instance.cardHandContainer.transform);
        }
        
        
        gameObject.SetActive(false);
    }
    #endregion

    #region Events
    public void OnDrag()
    {
        //Allows OnBeginDrag and OnEndDrag.
    }

    public void OnBeginDrag()
    {
        if (CardManager.Instance.board.currentBoardState == BoardState.Idle && !LeanTween.isTweening(gameObject))
        {
            CardManager.Instance.currentHoveredHandSlot = null;
            #region Tweening + Graphics
            LeanTween.cancel(gameObject);

            origin = rectTransform.anchoredPosition;
            targetTransform = CardManager.Instance.pointer.transform;
            transform.SetAsLastSibling();
            shadowTransform.gameObject.SetActive(true);
            rectTransform.localScale = Vector3.one * visuals.profile.draggedScale;
            #endregion

            HideTooltip();
            HideLinkedCards();

            CardManager.Instance.holdingCard = true;
            CardManager.Instance.currentCard = this;

            //Trigger movement detection
            isDragging = true;
            canvasGroup.blocksRaycasts = false;

            if (currentSlot != null)
                currentSlot.canvasGroup.blocksRaycasts = true;

            if (currentHandSlot != null)
                currentHandSlot.canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnEndDrag()
    {
        if (CardManager.Instance.board.currentBoardState == BoardState.Idle && !LeanTween.isTweening(gameObject))
        {
            #region Tweening + Graphics
            canTween = false;
            rectTransform.rotation = new Quaternion(0, 0, 0, 0);
            rectTransform.localScale = Vector3.one;
            shadowTransform.gameObject.SetActive(false);
            #endregion

            CardManager.Instance.board.HideTargetSlots();

            //If hovers, swap card, else drop in or out of slot
            if (CardManager.Instance.hoveredCard != null)
            {
                #region Swap Card
                //Swap cards from the board
                if (currentSlot != null) //If card in board
                {
                    if (CardManager.Instance.hoveredCard.currentSlot != null) //if other card in board
                    {
                        BoardSlot tempSlot = CardManager.Instance.hoveredCard.currentSlot;

                        CardManager.Instance.hoveredCard.currentSlot = currentSlot;
                        CardManager.Instance.hoveredCard.currentSlot.currentPlacedCard = CardManager.Instance.hoveredCard;
                        CardManager.Instance.hoveredCard.rectTransform.position = CardManager.Instance.hoveredCard.currentSlot.transform.position;
                        CardManager.Instance.hoveredCard.currentSlot.canvasGroup.blocksRaycasts = false;

                        tempSlot.currentPlacedCard = this;
                        currentSlot = tempSlot;
                        rectTransform.position = currentSlot.transform.position;

                        if (audioSource == null) audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
                        Sound intervert = new Sound(audioSource, "SFX_INTERVERTCARD", SoundType.SFX, false, false);
                        intervert.Play();
                    }
                    else //if other card in hand
                    {
                        if (IsCorrectPlacement(this, CardManager.Instance.hoveredCard))
                        {
                            //If you have enough mana to swap cards
                            int manaDifference = CardManager.Instance.hoveredCard.data.manaCost - data.manaCost;
                            if (CardManager.Instance.manaSystem.CanUseCard(manaDifference))
                            {
                                CardContainer hoveredCard = CardManager.Instance.hoveredCard;
                                HandSlot hoveredCardHandSlot = hoveredCard.currentHandSlot;
                                BoardSlot tempSlot = currentSlot;

                                PlaceOnBoardSlot(hoveredCard, tempSlot);
                                RemoveFromBoard(this);
                                PlaceOnHandSlot(this, hoveredCardHandSlot);

                                if (audioSource == null) audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
                                Sound intervert = new Sound(audioSource, "SFX_INTERVERTCARD", SoundType.SFX, false, false);
                                intervert.Play();
                            }
                            else
                            {
                                ReturnToOriginHandSlot(this);
                                canTween = true; //temp ?
                            }
                        }
                        else
                        {
                            rectTransform.position = currentSlot.transform.position;
                        }
                    }

                    CardManager.Instance.hoveredCard = null;
                }
                else //if card in hand
                {
                    if (CardManager.Instance.hoveredCard.currentSlot != null)//If card other card in board
                    {
                        if (IsCorrectPlacement(this, CardManager.Instance.hoveredCard))
                        {
                            //If you have enough mana to swap cards
                            int manaDifference = data.manaCost - CardManager.Instance.hoveredCard.data.manaCost;

                            if (CardManager.Instance.manaSystem.CanUseCard(manaDifference))
                            {
                                CardContainer hoveredCard = CardManager.Instance.hoveredCard;
                                BoardSlot hoveredCardSlot = hoveredCard.currentSlot;

                                RemoveFromBoard(hoveredCard);
                                PlaceOnHandSlot(hoveredCard, currentHandSlot);
                                PlaceOnBoardSlot(this, hoveredCardSlot);

                                if (audioSource == null) audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
                                Sound intervert = new Sound(audioSource, "SFX_INTERVERTCARD", SoundType.SFX, false, false);
                                intervert.Play();
                            }
                            else
                            {
                                ReturnToOriginHandSlot(this);
                                canTween = true; //temp ?
                                Debug.Log("Not enough mana");
                            }
                        }
                        else
                        {
                            ReturnToOriginHandSlot(this);
                        }
                    }
                    else
                    {
                        if (IsCorrectPlacement(this, CardManager.Instance.hoveredCard))
                        {
                            HandSlot originSlot = currentHandSlot;
                            CardContainer hoveredCard = CardManager.Instance.hoveredCard;

                            PlaceOnHandSlot(this, hoveredCard.currentHandSlot);

                            PlaceOnHandSlot(hoveredCard, originSlot);

                            if (audioSource == null) audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
                            Sound intervert = new Sound(audioSource, "SFX_INTERVERTCARD", SoundType.SFX, false, false);
                            intervert.Play();
                        }
                        else
                        {
                            ReturnToOriginHandSlot(this);
                        }
                    }
                    CardManager.Instance.hoveredCard = null;
                }
                #endregion
            }
            else
            {
                #region Drop in/out slot
                //Drop In Slot
                if (CardManager.Instance.currentHoveredSlot != null)
                {
                    //if you have enough man else abort and reset card
                    int manaCost = currentSlot != null ? 0 : data.manaCost;

                    if (CardManager.Instance.manaSystem.CanUseCard(manaCost) && CardManager.Instance.currentHoveredSlot != currentSlot)
                    {

                        PlaceOnBoardSlot(this, CardManager.Instance.currentHoveredSlot);

                        currentHandSlot.currentPlacedCard = null;

                        if (data.GetType() == typeof(PlotCard))
                        {
                            PlotCard card = data as PlotCard;
                            UpdateTimerTweening(card, false);
                        }
                    }
                    else
                    {
                        //reset card position
                        ReturnToOriginHandSlot(this);
                        canTween = true; //temp ?
                    }
                }
                else //Drop Out Slot
                {
                    targetTransform = null;
                    CardManager.Instance.holdingCard = false;

                    if (CardManager.Instance.currentHoveredHandSlot != null)
                    {
                        if (IsCorrectPlacement(this, CardManager.Instance.currentHoveredHandSlot))
                        {
                            PlaceOnHandSlot(this, CardManager.Instance.currentHoveredHandSlot);
                        }
                        else
                        {
                            ReturnToOriginHandSlot(this);
                        }
                    }
                    else
                    {
                        ReturnToOriginHandSlot(this);
                    }

                    if (currentSlot != null)//Remove card from board
                    {
                        RemoveFromBoard(this);
                    }

                    if (data.GetType() == typeof(PlotCard))
                    {
                        PlotCard card = data as PlotCard;
                        UpdateTimerTweening(card, true);
                    }
                }
                #endregion
            }

            CardManager.Instance.currentHoveredSlot = null;
            CardManager.Instance.holdingCard = false;
            CardManager.Instance.currentCard = null;
        }
    }

    public void OnPointerEnter()
    {
        //print(GetInfos());
        if(CardManager.Instance.board.currentBoardState == BoardState.Idle && !LeanTween.isTweening(gameObject))
        {
            if (CardManager.Instance.holdingCard && CardManager.Instance.currentCard != this)
            {
                CardManager.Instance.hoveredCard = this;
            }

            originPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);

            if (!CardManager.Instance.holdingCard && canTween)//Check if in hand
            {
                canTween = false;

                //Reset card rotation
                rectTransform.rotation = new Quaternion(0, 0, 0, 0);
                shadowTransform.localScale = Vector3.zero;

                //Scale up and bring to front;
                LeanTween.cancel(gameObject);
                siblingIndex = transform.GetSiblingIndex();
                transform.SetAsLastSibling();

                rectTransform.anchoredPosition = originPosition;
                originPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);

                rectTransform.localScale = Vector3.one * visuals.profile.hoveredScale;

                shadowTransform.gameObject.SetActive(true);

                ShowTooltip();

                if (data.GetType() == typeof(PlotCard))
                {
                    PlotCard plotCard = data as PlotCard;
                    if(plotCard.isMainPlot)
                    {
                        ShowLinkedCards(plotCard);
                    }
                }
            }
        }
    }

    public void OnPointerExit()
    {
        if (CardManager.Instance.board.currentBoardState == BoardState.Idle && !LeanTween.isTweening(gameObject))
        {
            if (CardManager.Instance.holdingCard && CardManager.Instance.currentCard != this)
            {
                CardManager.Instance.hoveredCard = null;
            }

            #region Tweening
            canTween = true;
            if (!CardManager.Instance.holdingCard)//Check if in hand
            {
                //Scale down 
                shadowTransform.gameObject.SetActive(false);
                rectTransform.localScale = Vector3.one;

                HideTooltip();
                HideLinkedCards();

                //if (!isDragging)
                //    transform.SetSiblingIndex(siblingIndex);
            }
            #endregion
        }
    }
    #endregion

    #region Utility
    void PlaceOnBoardSlot(CardContainer container, BoardSlot targetSlot)
    {
        if (container.currentSlot == null)
        {
            CardManager.Instance.manaSystem.LoseMana(container.data.manaCost);
            CardManager.Instance.cardHand.currentHand.Remove(container);
            CardManager.Instance.UpdateHandCount();
        }

        if (container.currentSlot != null)
        {
            container.currentSlot.currentPlacedCard = null;
            container.currentSlot = null;
        }

        targetSlot.currentPlacedCard = container;
        container.currentSlot = targetSlot;
        container.currentSlot.canvasGroup.blocksRaycasts = false;
        container.rectTransform.position = targetSlot.transform.position;
    }

    void PlaceOnHandSlot(CardContainer container, HandSlot targetSlot)
    {
        if (container.currentHandSlot != null && container.currentHandSlot.currentPlacedCard == container)
        {
            container.currentHandSlot.currentPlacedCard = null;
            container.currentHandSlot.canvasGroup.blocksRaycasts = true;
        }

        container.rectTransform.position = targetSlot.self.position;
        container.currentHandSlot = targetSlot;
        container.currentHandSlot.currentPlacedCard = container;
        container.currentHandSlot.canvasGroup.blocksRaycasts = false;
    }

    void ReturnToOriginHandSlot(CardContainer container)
    {
        if (currentSlot != null)
        {
            currentSlot.currentPlacedCard = null;
            currentSlot = null;
            CardManager.Instance.manaSystem.GainMana(container.data.manaCost);
            CardManager.Instance.cardHand.currentHand.Add(this);
            CardManager.Instance.UpdateHandCount();
        }
        container.rectTransform.position = CardManager.Instance.cardHand.GetPosInHand(this);
    }

    void RemoveFromBoard(CardContainer container)
    {
        container.currentSlot.currentPlacedCard = null;
        container.currentSlot.canvasGroup.blocksRaycasts = true;
        container.currentSlot = null;

        //Refill Mana
        CardManager.Instance.manaSystem.GainMana(container.data.manaCost);

        //Add back to hand list 
        CardManager.Instance.cardHand.currentHand.Add(container);
        CardManager.Instance.UpdateHandCount();
    }

    void WrongPlacementSound()
    {
        if (audioSource == null) audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
        Sound intervert = new Sound(audioSource, "SFX_WRONGPLACEMENT", SoundType.SFX, false, false);
        intervert.Play();
    }
    bool IsCorrectPlacement(CardContainer container, HandSlot hoveredHandSlot)
    {
        Type cardType = container.data.GetType();
        if (cardType != typeof(PlotCard) && hoveredHandSlot == CardManager.Instance.cardHand.plotHandSlot)
        {
            WrongPlacementSound();
            return false;
        }
        else if(cardType == typeof(PlotCard) && hoveredHandSlot != CardManager.Instance.cardHand.plotHandSlot)
        {
            WrongPlacementSound();
            return false;
        }
        return true;

    }
    bool IsCorrectPlacement(CardContainer currentContainer, CardContainer hoveredContainer)
    {
        Type currentCardType = currentContainer.data.GetType();
        Type hoveredCardType = hoveredContainer.data.GetType();

        if (currentCardType == typeof(PlotCard) && hoveredCardType != typeof(PlotCard))
        {
            WrongPlacementSound();
            return false;
        }
        if (currentCardType != typeof(PlotCard) && hoveredCardType == typeof(PlotCard))
        {
            WrongPlacementSound();
            return false;
        }

        return true;
    }

    public void UpdateCharacterInfo(CharacterType character)
    {
        visuals.UpdateCharacterElements(character, true);
    }
    public void UpdatePlotInfo(PlotCard card)
    {
        visuals.UpdatePlotElements(card, true);
    }
    public void UpdateBaseInfo()
    {
        visuals.UpdateBaseElements(data, true);
    }
    public void ShowTooltip()
    {
        List<string> keywords = new List<string>(LocalizationManager.Instance.tooltipDictionary.Keys);
        int count = 0;

        //if effect contains keyword appear tooltip
        string effectDescription = visuals.cardDescriptionText.text.ToLower();
        for (int j = 0; j < keywords.Count; j++)
        {
            if(effectDescription.Contains(keywords[j]))
            {
                tooltips[count].AppearTooltip(keywords[j], 1, 0.3f + (count + 1) * 0.1f);
                count++;
            }
        }
    }
    public void HideTooltip()
    {
        for (int i = 0; i < tooltips.Count; i++)
        {
            tooltips[i].hovered = false;
        }
        int count = 0;
        //hide all active tooltip
        for (int i = 0; i < tooltips.Count; i++)
        {
            if(tooltips[i].gameObject.activeSelf)
            {
                tooltips[i].HideTooltip(count * 0.1f);
                count++;
            }
        }
    }

    public void ShowLinkedCards(PlotCard card)
    {
        for (int i = 0; i < card.objective.linkedJunkedCards.Count; i++)
        {
            //Show Feedback for each card
            if(card.objective.linkedJunkedCards[i].currentContainer != null)
            {
                CardVisuals visuals = card.objective.linkedJunkedCards[i].currentContainer.visuals;
                CardManager.Instance.cardTweening.ShowHighlight(visuals.cardHighlight, visuals.profile.highlightColor);
            }
            else if(CardManager.Instance.cardDeck.cardDeck.Contains(card.objective.linkedJunkedCards[i]))
            {
                CardManager.Instance.cardTweening.ShowHighlight(CardManager.Instance.cardDeck.deckHighlight, visuals.profile.highlightColor);
            }
            else if(CardManager.Instance.cardDeck.discardPile.Contains(card.objective.linkedJunkedCards[i]))
            {
                CardManager.Instance.cardTweening.ShowHighlight(CardManager.Instance.cardDeck.discardHighlight, visuals.profile.highlightColor);
            }
        }
    }
    public void HideLinkedCards()
    {
        //Hide for each container
        for (int i = 0; i < CardManager.Instance.cardHand.hiddenHand.Count; i++)
        {
            CardVisuals visuals = CardManager.Instance.cardHand.hiddenHand[i].visuals;
            CardManager.Instance.cardTweening.HideHighlight(visuals.cardHighlight);
        }

        //Hide for deck
        CardManager.Instance.cardTweening.HideHighlight(CardManager.Instance.cardDeck.deckHighlight);

        //Hide for discard
        CardManager.Instance.cardTweening.HideHighlight(CardManager.Instance.cardDeck.discardHighlight);
    }

    public void UpdateTimerTweening(PlotCard card,bool enable)
    {
        if(enable)
        {
            if(card.completionTimer == 1)
            {
                if (audioSource == null) audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
                Sound intervert = new Sound(audioSource, "SFX_CLOCKTICKEND01", SoundType.SFX, false, false);
                intervert.Play();

                visuals.timerText.color = Color.red;
                LeanTween.cancel(card.currentContainer.visuals.cardTimerFrame.gameObject);
                card.currentContainer.visuals.cardTimerFrame.gameObject.transform.localScale = Vector3.one;
                CardManager.Instance.cardTweening.ScaleBounceLoop(card.currentContainer.visuals.cardTimerFrame.gameObject, 1.5f);
            }
        }
        else
        {
            LeanTween.cancel(card.currentContainer.visuals.cardTimerFrame.gameObject);
            card.currentContainer.visuals.cardTimerFrame.gameObject.transform.localScale = Vector3.one;
        }
    }

    public string GetInfos()
    {
        string cardInfo = $" slot : {currentSlot}\n cardName : {data.cardName}\n cardType : {data.cardType}\n cardEffects :\n";
        for (int i = 0; i < data.effects.Count; i++)
        {
            cardInfo += $"{data.effects[i].description}";
        }
        cardInfo += $"\ncardEnter Event :{data.onCardEnter}\nstoryEndEvent :{data.onStoryEnd}";
        return cardInfo;
    }
    #endregion
}
