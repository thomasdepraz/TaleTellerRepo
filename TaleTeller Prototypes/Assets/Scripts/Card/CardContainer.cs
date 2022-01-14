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
    
    private void Update()
    {
        if(isDragging && Input.GetMouseButtonUp(0))
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
        }

        //if(Input.GetKeyDown(KeyCode.S))
        //{
        //    visuals.ShakeCard(this, new EventQueue());
        //}
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    CardManager.Instance.cardTweening.CardAttack(this, 0, new EventQueue());
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    visuals.EffectChangeFeedback(this, 1, new EventQueue());
        //}
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    visuals.EffectChangeFeedback(this, -1, new EventQueue());
        //}

    }

    #region Init/Reset
    public void InitializeContainer(CardData data, bool isPlaceHolder = false)
    {
        this.data = data;

        //load Data and activate gameobject
        visuals.InitializeVisuals(data);

        basePosition = transform.position;

        if(!isPlaceHolder)
            data.currentContainer = this;


        gameObject.SetActive(true);
        originPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);

        //add random rotation 
        rectTransform.rotation = new Quaternion(0,0,Random.Range(-0.1f,0.1f),1);

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

        if(!isPlaceholder)
        {
            data.currentContainer = null;
        }
        data = null;
        currentSlot = null;


        //Reset to hidden hand

        if(!isPlaceholder)
        {
            rectTransform.position = basePosition;
            CardManager.Instance.cardHand.currentHand.Remove(this);
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
        }
    }

    public void OnEndDrag()
    {
        #region Tweening + Graphics
        canTween = false;
        rectTransform.rotation = new Quaternion(0, 0, 0, 0);
        rectTransform.localScale = Vector3.one;
        shadowTransform.gameObject.SetActive(false);
        #endregion

        CardManager.Instance.board.HideTargetSlots();

        //If hovers, swap card, else drop in or out of slot
        if(CardManager.Instance.hoveredCard != null)
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
                }
                else //if other card in hand
                {
                    //If you have enough mana to swap cards
                    int manaDifference = CardManager.Instance.hoveredCard.data.manaCost - data.manaCost;
                    if (CardManager.Instance.manaSystem.CanUseCard(manaDifference))
                    {
                        BoardSlot tempSlot = currentSlot;
                        tempSlot.currentPlacedCard = CardManager.Instance.hoveredCard;
                        CardManager.Instance.hoveredCard.currentSlot = tempSlot;

                        currentSlot = null;


                        rectTransform.anchoredPosition = CardManager.Instance.hoveredCard.originPosition;

                        CardManager.Instance.hoveredCard.rectTransform.position = tempSlot.transform.position;
                        transform.SetParent(CardManager.Instance.cardHandContainer.transform);

                        //ApplyManaDiff
                        if (manaDifference > 0) CardManager.Instance.manaSystem.LoseMana(manaDifference);
                        else if (manaDifference < 0) CardManager.Instance.manaSystem.GainMana(Mathf.Abs(manaDifference));

                        //Manage hand list
                        CardManager.Instance.cardHand.currentHand.Remove(CardManager.Instance.hoveredCard);
                        CardManager.Instance.cardHand.currentHand.Add(this);

                    }
                    else
                    {
                        //reset card position
                        rectTransform.anchoredPosition = originPosition;
                        canTween = true; //temp ?
                        Debug.Log("Not enough mana");
                    }

                }

                CardManager.Instance.hoveredCard = null;
            }
            else //if card in hand
            {
                if (CardManager.Instance.hoveredCard.currentSlot != null)//If card other card in board
                {
                    //If you have enough mana to swap cards
                    int manaDifference = data.manaCost - CardManager.Instance.hoveredCard.data.manaCost;

                    if (CardManager.Instance.manaSystem.CanUseCard(manaDifference))
                    {
                        BoardSlot tempSlot = CardManager.Instance.hoveredCard.currentSlot;
                        tempSlot.currentPlacedCard = this;
                        currentSlot = tempSlot;

                        CardManager.Instance.hoveredCard.currentSlot = null;

                        CardManager.Instance.hoveredCard.rectTransform.anchoredPosition = originPosition;
                        CardManager.Instance.hoveredCard.transform.SetParent(CardManager.Instance.cardHandContainer.transform); //set back to hand

                        rectTransform.position = tempSlot.transform.position;

                        //ApplyManaDiff
                        if (manaDifference > 0) CardManager.Instance.manaSystem.LoseMana(manaDifference);
                        else if (manaDifference < 0) CardManager.Instance.manaSystem.GainMana(Mathf.Abs(manaDifference));

                        //Manage hand list
                        CardManager.Instance.cardHand.currentHand.Add(CardManager.Instance.hoveredCard);
                        CardManager.Instance.cardHand.currentHand.Remove(this);

                    }
                    else
                    {
                        //reset card position
                        rectTransform.anchoredPosition = originPosition;
                        canTween = true; //temp ?
                        Debug.Log("Not enough mana");
                    }
                }
                else //if other card in hand
                {
                    Vector3 position = CardManager.Instance.hoveredCard.rectTransform.anchoredPosition;

                    CardManager.Instance.hoveredCard.rectTransform.anchoredPosition = originPosition;
                    rectTransform.anchoredPosition = position;
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
                if (CardManager.Instance.manaSystem.CanUseCard(data.manaCost) && CardManager.Instance.currentHoveredSlot != currentSlot)
                {
                    if(currentSlot == null)
                    {
                        CardManager.Instance.manaSystem.LoseMana(data.manaCost);
                        CardManager.Instance.cardHand.currentHand.Remove(this);
                    }

                    if (currentSlot != null)
                    {
                        currentSlot.currentPlacedCard = null;
                        currentSlot = null;
                    }

                    CardManager.Instance.currentHoveredSlot.currentPlacedCard = this;
                    currentSlot = CardManager.Instance.currentHoveredSlot;
                    currentSlot.canvasGroup.blocksRaycasts = false;
                    rectTransform.position = CardManager.Instance.currentHoveredSlot.transform.position;

                    if(data.GetType()==typeof(PlotCard))
                    {
                        PlotCard card = data as PlotCard;
                        UpdateTimerTweening(card, false);
                    }
                }
                else
                {
                    //reset card position
                    rectTransform.anchoredPosition = originPosition;
                    canTween = true; //temp ?
                }
            }
            else //Drop Out Slot
            {
                targetTransform = null;
                CardManager.Instance.holdingCard = false;
                originPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
                rectTransform.anchoredPosition = originPosition;

                if (currentSlot != null)//Remove card from board
                {
                    currentSlot.currentPlacedCard = null;
                    currentSlot.canvasGroup.blocksRaycasts = true;
                    currentSlot = null;

                    //Refill Mana
                    CardManager.Instance.manaSystem.GainMana(data.manaCost);

                    //Add back to hand list 
                    CardManager.Instance.cardHand.currentHand.Add(this);
                }

                //if not in hand move to hand
                if(!CardManager.Instance.cardHand.IsInHand(this))
                {
                    CardManager.Instance.cardTweening.MoveCard(this, CardManager.Instance.cardHand.GetPositionInHand(data));
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

    public void OnPointerEnter()
    {
        if(CardManager.Instance.board.currentBoardState == BoardState.Idle && !LeanTween.isTweening(gameObject))
        {
            if (CardManager.Instance.holdingCard && CardManager.Instance.currentCard != this)
            {
                CardManager.Instance.hoveredCard = this;
            }

            originPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);

            if (transform.parent == CardManager.Instance.cardHandContainer.transform && !CardManager.Instance.holdingCard && canTween)//Check if in hand
            {
                canTween = false;

                //Reset card rotation
                rectTransform.rotation = new Quaternion(0, 0, 0, 0);

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
            if (transform.parent == CardManager.Instance.cardHandContainer.transform && !CardManager.Instance.holdingCard)//Check if in hand
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
    public void UpdateCharacterInfo(CharacterType character)
    {
        visuals.UpdateCharacterElements(character);
    }
    public void UpdatePlotInfo(PlotCard card)
    {
        visuals.UpdatePlotElements(card);
    }
    public void UpdateBaseInfo()
    {
        visuals.UpdateBaseElements(data);
    }

    public void ShowTooltip()
    {
        List<string> keywords = new List<string>(LocalizationManager.Instance.tooltipDictionary.Keys);
        int count = 0;

        //if effect contains keyword appear tooltip
        string effectDescription = visuals.cardDescriptionText.text;
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
    #endregion
}
