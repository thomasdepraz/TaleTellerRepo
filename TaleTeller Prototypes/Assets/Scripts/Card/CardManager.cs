using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : Singleton<CardManager>
{
    private void Awake()
    {
        CreateSingleton();
    }
    [Header("References")]
    public Deck cardDeck;
    public Hand cardHand;
    public Board board;
    public CardTweening cardTweening;
    public RectTransform discardPileTransform;
    public RectTransform deckTransform;
    public RectTransform plotAppearTransform;

    public ManaSystem manaSystem;

    public CardPicker cardPicker;

    [HideInInspector] public BoardSlot currentHoveredSlot;
    [HideInInspector] public bool holdingCard;
    [HideInInspector] public CardContainer currentCard;
    [HideInInspector] public CardContainer hoveredCard;

    public GameObject cardHandContainer;

    public Pointer pointerRef;
    public GameObject pointer;

    //Init Card
    public void CardInitialize(CardData data)
    {
        for (int i = 0; i < cardHand.hiddenHand.Count; i++)
        {
            if (!cardHand.hiddenHand[i].gameObject.activeSelf)
            {
                cardHand.hiddenHand[i].InitializeContainer(data);
                cardHand.hiddenHand[i].rectTransform.SetParent(cardHand.handTransform);

                break;
            }
        }
    }

    public void ClearCardLists()
    {
        cardDeck.cardDeck.Clear();
        cardDeck.discardPile.Clear();
        cardHand.ResetAllHand();
    }

    # region Card Appear
    public void CardAppear(EventQueue queue, CardData card, Vector3 position)
    {
        queue.events.Add(CardAppearRoutine(queue, card, position));
    }
    IEnumerator CardAppearRoutine(EventQueue queue, CardData card, Vector3 position)
    {
        #region Init Container
        EventQueue initQueue = new EventQueue();
        CardInitialize(card);
        initQueue.StartQueue();
        while (!initQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion

        card.currentContainer.rectTransform.localPosition = position;

        EventQueue appearFeedback = new EventQueue();
        cardTweening.MoveCard(card.currentContainer, position, true, true, appearFeedback,1.75f);
        while (!appearFeedback.resolved) { yield return new WaitForEndOfFrame(); }


        queue.UpdateQueue();
    }
    #endregion

    #region Send to hand
    public void CardToHand(EventQueue queue, CardData card)
    {
        queue.events.Add(CardToHandRoutine(queue, card));
    }
    IEnumerator CardToHandRoutine(EventQueue queue, CardData card)
    {
        yield return null;

        EventQueue toHandQueue = new EventQueue();

        if (cardHand.currentHand.Count + 1 > cardHand.maxHandSize)
        {
            //Overdraw
            cardDeck.OverDraw(toHandQueue, card);
        }
        else
        {
            //Deal
            cardDeck.Deal(toHandQueue, card);
        }

        toHandQueue.StartQueue();
        while (!toHandQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        queue.UpdateQueue();
    }
    #endregion

    #region Send to deck
    public void CardToDeck(EventQueue queue, CardData card, bool addToDeck = true)
    {
        queue.events.Add(CardToDeckRoutine(queue, card, addToDeck));
    }
    IEnumerator CardToDeckRoutine(EventQueue queue, CardData card, bool addToDeck = true)
    {
        EventQueue toDeckFeedback = new EventQueue();
        cardTweening.MoveCard(card.currentContainer, deckTransform.localPosition, true, false, toDeckFeedback);
        while (!toDeckFeedback.resolved) { yield return new WaitForEndOfFrame(); }

        if (addToDeck)
        {
            cardDeck.cardDeck.Add(card);

            Type cardType = card.GetType();
            if(cardType != typeof(PlotCard) && cardType != typeof(JunkCard))
            {
                cardDeck.cachedDeck.Add(card.dataReference);
            }
        }

        card.currentContainer.ResetContainer();
        queue.UpdateQueue();
    }
    #endregion

    #region Deck to Hand
    public void CardDeckToHand(EventQueue queue, CardData data, bool alreadyAppeared = false)
    {
        queue.events.Add(CardDeckToHandRoutine(queue, data, alreadyAppeared));
    }
    public IEnumerator CardDeckToHandRoutine(EventQueue queue, CardData data, bool alreadyAppeared)
    {
        yield return null;

        if(!alreadyAppeared)
            CardInitialize(data);

        cardHand.currentHand.Add(data.currentContainer);

        data.currentContainer.rectTransform.SetParent(cardHand.handTransform);
        cardTweening.MoveCard(data.currentContainer, cardHand.GetPositionInHand(data), !alreadyAppeared, !alreadyAppeared, queue);
        yield return new WaitForSeconds(0.2f);

        queue.UpdateQueue();
    }
    #endregion

    #region Board to Hand 
    public void CardBoardToHand(CardContainer card, bool canPushOverCard, EventQueue queue)
    {
        queue.events.Add(CardBoardToHandRoutine(card, canPushOverCard, queue));
    }
    IEnumerator CardBoardToHandRoutine(CardContainer card, bool canPushOverCard, EventQueue currentQueue)
    {
        yield return null;

        EventQueue returnQueue = new EventQueue();

        if (canPushOverCard)
        {
            if (cardHand.currentHand.Count == cardHand.maxHandSize)//if max cards in hand make the player select a card
            {
                //MAKE the player pick a card and discard it
                EventQueue pickQueue = new EventQueue();
                List<CardData> pickedCards = new List<CardData>();

                string instruction = LocalizationManager.Instance.GetString(LocalizationManager.Instance.instructionsDictionary, GameManager.Instance.instructionsData.chooseCardInstruction);
                cardPicker.Pick(pickQueue, cardHand.GetHandDataList(), pickedCards, 1, instruction);

                pickQueue.StartQueue();
                while (!pickQueue.resolved)
                {
                    yield return new WaitForEndOfFrame();
                }

                //discard all of the picked cards
                for (int i = 0; i < pickedCards.Count; i++)
                {
                    CardHandToDiscard(pickedCards[i].currentContainer, returnQueue);
                }

                //return card to hand
                ReturnCard(card, returnQueue);
            }
            else//just return card to hand
            {
                ReturnCard(card, returnQueue);
            }
        }
        else
        {
            if (cardHand.currentHand.Count == cardHand.maxHandSize)//if max cards in hand discard
            {
                CardBoardToDiscard(card, returnQueue);
            }
            else//just return card to hand
            {
                ReturnCard(card, returnQueue);
            }
        }

        returnQueue.StartQueue(); //Actual discard / return happens here

        while (!returnQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }

    private void ReturnCard(CardContainer card, EventQueue queue)
    {
        queue.events.Add(ReturnCardRoutine(card, queue));
    }
    IEnumerator ReturnCardRoutine(CardContainer card, EventQueue queue)
    {
        //remove from board list
        card.currentSlot.currentPlacedCard = null;
        card.currentSlot.canvasGroup.blocksRaycasts = true;
        card.currentSlot = null;


        //use method from deck to move cardBack to hand
        EventQueue feedback = new EventQueue();
        cardTweening.MoveCard(card, cardHand.GetPositionInHand(card.data), false, false, feedback);
        cardHand.currentHand.Add(card);
        while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }

        queue.UpdateQueue();
    }
    #endregion

    #region Board to Discard
    public void CardBoardToDiscard(CardContainer card, EventQueue queue)
    {
        //new discard method
        queue.events.Add(CardBoardToDiscardRoutine(card, queue));
    }
    IEnumerator CardBoardToDiscardRoutine(CardContainer card, EventQueue currentQueue)
    {
        //Add feedback
        EventQueue feedback = new EventQueue();
        cardTweening.MoveCard(card, discardPileTransform.localPosition, true, false, feedback);
        while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }


        card.data = card.data.ResetData(card.data);

        cardDeck.discardPile.Add(card.data);

        //remove from board list
        card.currentSlot.currentPlacedCard = null;
        card.currentSlot.canvasGroup.blocksRaycasts = true;

        card.ResetContainer();

        currentQueue.UpdateQueue();
    }
    #endregion

    #region Hand to Discard
    public void CardHandToDiscard(CardContainer card, EventQueue queue)
    {
        queue.events.Add(CardHandToDiscardRoutine(card, queue));
    }
    IEnumerator CardHandToDiscardRoutine(CardContainer card, EventQueue queue)
    {
        #region Event OnCardDiscard
        EventQueue onCardDiscardQueue = new EventQueue();

        if (card.data.onCardDiscard != null)
            card.data.onCardDiscard(onCardDiscardQueue, card.data);

        onCardDiscardQueue.StartQueue();
        while (!onCardDiscardQueue.resolved)
        { yield return new WaitForEndOfFrame(); }
        #endregion

        #region Event OnAnyCardDiscard (Overload)
        EventQueue overloadQueue = new EventQueue();
        board.CallBoardEvents("overload", overloadQueue);

        overloadQueue.StartQueue();
        while (!overloadQueue.resolved)
        { yield return new WaitForEndOfFrame(); }
        #endregion

        //Add feedback
        EventQueue feedback = new EventQueue();
        cardTweening.MoveCard(card, discardPileTransform.localPosition, true, false, feedback);
        while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }

        card.data = card.data.ResetData(card.data);
        cardHand.currentHand.Remove(card);
        cardDeck.discardPile.Add(card.data);
        card.ResetContainer();

        queue.UpdateQueue();
    }
    #endregion

    #region Composed Methods
    public void CardAppearToHand(CardData card, EventQueue queue, Vector3 appearPosition)
    {
        queue.events.Add(CardAppearToHandRoutine(card, queue, appearPosition));
    }
    IEnumerator CardAppearToHandRoutine(CardData card, EventQueue queue, Vector3 appearPosition)
    {
        EventQueue appearAndSendQueue = new EventQueue();
        CardAppear(appearAndSendQueue, card, appearPosition);
        CardToHand(appearAndSendQueue, card);
        appearAndSendQueue.StartQueue();
        while (!appearAndSendQueue.resolved) { yield return new WaitForEndOfFrame(); }
        queue.UpdateQueue();
    }

    public void CardAppearToDeck(CardData card, EventQueue queue, Vector3 appearPosition, bool addToDeck = true)
    {
        queue.events.Add(CardAppearToDeckRoutine(card, queue, appearPosition, addToDeck));
    }
    IEnumerator CardAppearToDeckRoutine(CardData card, EventQueue queue, Vector3 appearPosition, bool addToDeck)
    {
        EventQueue appearAndSendQueue = new EventQueue();
        CardAppear(appearAndSendQueue, card, appearPosition);
        CardToDeck(appearAndSendQueue, card, addToDeck);
        appearAndSendQueue.StartQueue();
        while (!appearAndSendQueue.resolved) { yield return new WaitForEndOfFrame(); }
        queue.UpdateQueue();
    }
    #endregion
}
