using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public Inspire inspire;
    public CardTweening cardTweening;
    public RectTransform discardPileTransform;
    public RectTransform deckTransform;
    public RectTransform plotAppearTransform;
    public RectTransform deckAppearTransform;
    public RectTransform oblivionTransfrom;

    public ManaSystem manaSystem;

    public CardPicker cardPicker;

    [HideInInspector] public BoardSlot currentHoveredSlot;
    [HideInInspector] public HandSlot currentHoveredHandSlot;
    [HideInInspector] public bool holdingCard;
    [HideInInspector] public CardContainer currentCard;
    [HideInInspector] public CardContainer hoveredCard;

    public GameObject cardHandContainer;

    public Pointer pointerRef;
    public GameObject pointer;

    [Header("Player Exclamation")]
    public RectTransform exclamationPanelTransform;
    public TextMeshProUGUI exclamationText;
    public RectTransform messagePanelTransform;
    public TextMeshProUGUI messageText;
    public HeroMessage currentPlayerMessage;
    public ScreenButton playerMessageButton;
    public Message currentMessage;
    public ScreenButton messageButton;

    public CardData activatedCard;

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

        card.currentContainer.rectTransform.position = position;

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

        if (cardHand.GetHandCount() + 1 > cardHand.maxHandSize)
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
        cardTweening.MoveCard(card.currentContainer, deckTransform.position, true, false, toDeckFeedback);
        while (!toDeckFeedback.resolved) { yield return new WaitForEndOfFrame(); }

        if (addToDeck)
        {
            int r = UnityEngine.Random.Range(0, CardManager.Instance.cardDeck.cardDeck.Count);

            cardDeck.cardDeck.Insert(r, card);

            Type cardType = card.GetType();
            if(cardType != typeof(PlotCard) && cardType != typeof(JunkCard))
            {
                if (r < cardDeck.cachedDeck.Count)
                    cardDeck.cachedDeck.Insert(r, card.dataReference);
                else
                    cardDeck.cachedDeck.Insert(cardDeck.cachedDeck.Count, card.dataReference);
            }
        }

        card.currentContainer.ResetContainer();
        queue.UpdateQueue();
    }
    #endregion

    #region SendToOblivion
    public void CardToOblivion(EventQueue queue, CardData card)
    {
        queue.events.Add(CardToOblivionRoutine(queue, card));
    }
    IEnumerator CardToOblivionRoutine(EventQueue queue, CardData card)
    {
        //Move feedback
        card.UnsubscribeEvents(card);
        StoryManager.Instance.cardsToDestroy.Add(card);

        if(card.currentContainer != null)
        {
            if(card.GetType()== typeof(PlotCard))
            {
                card.currentContainer.rectTransform.localScale = Vector3.one * 1.01f;
                EventQueue centerFeedback = new EventQueue();
                cardTweening.MoveCard(card.currentContainer, plotAppearTransform.position, false, false, centerFeedback, 2f);
                while (!centerFeedback.resolved) { yield return new WaitForEndOfFrame(); }
            }

            EventQueue feedback = new EventQueue();
            cardTweening.MoveCard(card.currentContainer, oblivionTransfrom.position, false, false, feedback);
            while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }

            card.currentContainer.ResetContainer();
        }

        if(cardDeck.cardDeck.Contains(card))
        {
            CardInitialize(card);
            card.currentContainer.rectTransform.position = deckTransform.position;

            EventQueue feedback = new EventQueue();
            cardTweening.MoveCard(card.currentContainer, oblivionTransfrom.position, true, true, feedback);
            while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }
            cardDeck.cardDeck.Remove(card);
        }
        else if (cardDeck.discardPile.Contains(card))
        {
            CardInitialize(card);
            card.currentContainer.rectTransform.position = discardPileTransform.position;

            EventQueue feedback = new EventQueue();
            cardTweening.MoveCard(card.currentContainer, oblivionTransfrom.position, true, true, feedback);
            while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }
            cardDeck.discardPile.Remove(card);
        }

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
        UpdateHandCount();

        if (data.currentContainer.audioSource == null) data.currentContainer.audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
        Sound intervert = new Sound(data.currentContainer.audioSource, "SFX_DRAW", SoundType.SFX, false, false);
        intervert.Play();

        data.currentContainer.rectTransform.SetParent(cardHand.handTransform);
        cardTweening.MoveCard(data.currentContainer, cardHand.GetPosInHand(data.currentContainer), !alreadyAppeared, !alreadyAppeared, queue);
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
            if (cardHand.GetHandCount() == cardHand.maxHandSize)//if max cards in hand make the player select a card
            {

                //MAKE the player pick a card and discard it
                CardPickerScreen screen = new CardPickerScreen(PickScreenMode.REPLACE, 1, cardHand.GetHandDataList(), true);
                bool wait = true;
                screen.Open(() => wait = false);
                while (wait) { yield return new WaitForEndOfFrame(); }

                while (screen.open) { yield return new WaitForEndOfFrame(); }
                wait = true;
                screen.Close(() => wait = false);
                while (wait) { yield return new WaitForEndOfFrame(); }


                //discard all of the picked cards
                for (int i = 0; i < screen.pickedCards.Count; i++)
                {
                    CardHandToDiscard(screen.pickedCards[i].container.data.currentContainer, returnQueue);
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
            if (cardHand.GetHandCount() == cardHand.maxHandSize)//if max cards in hand discard
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
        cardTweening.MoveCard(card, cardHand.GetPosInHand(card), false, false, feedback);
        cardHand.currentHand.Add(card);
        UpdateHandCount();
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
        if (card.audioSource == null) card.audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
        Sound intervert = new Sound(card.audioSource, "SFX_DISCARD", SoundType.SFX, false, false);
        intervert.Play();

        //Add feedback
        EventQueue feedback = new EventQueue();
        cardTweening.MoveCard(card, discardPileTransform.position, true, false, feedback);
        while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }

        print("Reset Number 2");
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

        if (card.audioSource == null) card.audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
        Sound intervert = new Sound(card.audioSource, "SFX_DISCARD", SoundType.SFX, false, false);
        intervert.Play();

        //Add feedback
        EventQueue feedback = new EventQueue();
        cardTweening.MoveCard(card, discardPileTransform.position, true, false, feedback);
        while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }

        CardData data = card.data.ResetData(card.data);
        cardDeck.discardPile.Add(data);

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

    #region DiscardFromDeck
    public void DiscardFromDeck(int countToDiscard, EventQueue queue)
    {
        queue.events.Add(DiscardFromDeckRoutine(countToDiscard, queue));
    }
    IEnumerator DiscardFromDeckRoutine(int countToDiscard, EventQueue queue)
    {
        List<CardData> cards = GetCurrentCards();

        if(cards.Count>0)
        {
            CardPickerScreen screen = new CardPickerScreen(PickScreenMode.WITHDRAW, countToDiscard, cards, true);
            bool wait = true;
            screen.Open(()=> wait = false);
            while (wait) { yield return new WaitForEndOfFrame(); }

            while(screen.open) { yield return new WaitForEndOfFrame(); }
            wait = true;
            screen.Close(()=> wait = false);
            while (wait) { yield return new WaitForEndOfFrame(); }

            for (int i = 0; i < screen.pickedCards.Count; i++)
            {
                if(cardDeck.cardDeck.Contains(screen.pickedCards[i].container.data))
                    cardDeck.cardDeck.Remove(screen.pickedCards[i].container.data); //TODO Animate this with a card management method such as SendToOblivion

                else if (cardDeck.discardPile.Contains(screen.pickedCards[i].container.data))
                    cardDeck.discardPile.Remove(screen.pickedCards[i].container.data); //TODO Animate this with a card management method such as SendToOblivion

                else if (cardHand.IsInHand(screen.pickedCards[i].container.data.currentContainer))
                    screen.pickedCards[i].container.data.currentContainer.ResetContainer();
            
                else if(board.IsCardOnBoard(screen.pickedCards[i].container.data))
                    screen.pickedCards[i].container.data.currentContainer.ResetContainer();


                if (screen.pickedCards[i].container.data.currentContainer != null)
                {
                    screen.pickedCards[i].container.data.UnsubscribeEvents(screen.pickedCards[i].container.data) ;
                    StoryManager.Instance.cardsToDestroy.Add(screen.pickedCards[i].container.data);
                }
            }
        }

        

        ////refill deck + reinit cards
        //cardDeck.ResetCachedDeck();
        queue.UpdateQueue();
    }
    #endregion

    public void UpdateHandCount()
    {
        cardHand.handCountText.text = $"{cardHand.GetHandCount()}/{cardHand.maxHandSize}";
    }

    public List<CardData> GetCurrentCards()
    {
        List<CardData> result = new List<CardData>();

        //DECK
        for (int i = 0; i < cardDeck.cardDeck.Count; i++)
        {
            Type cardType = cardDeck.cardDeck[i].GetType();
            if (cardType != typeof(PlotCard) && cardType != typeof(JunkCard))
                result.Add(cardDeck.cardDeck[i]);
        }

        //DISCARD
        for (int i = 0; i < cardDeck.discardPile.Count; i++)
        {
            Type cardType = cardDeck.discardPile[i].GetType();
            if (cardType != typeof(PlotCard) && cardType != typeof(JunkCard))
                result.Add(cardDeck.discardPile[i]);
        }

        //BOARD
        for (int i = 0; i < board.slots.Count; i++)
        {
            if(board.slots[i].currentPlacedCard != null)
            {
                Type cardType = board.slots[i].currentPlacedCard.data.GetType();
                if (cardType != typeof(PlotCard) && cardType != typeof(JunkCard))
                    result.Add(board.slots[i].currentPlacedCard.data);
            }
        }

        //HAND
        for (int i = 0; i < cardHand.currentHand.Count; i++)
        {
            Type cardType = cardHand.currentHand[i].data.GetType();
            if (cardType != typeof(PlotCard) && cardType != typeof(JunkCard))
                result.Add(cardHand.currentHand[i].data);
        }

        return result;
    }
}
