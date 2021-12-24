using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<CardData> cardDeck;
    public List<CardData> discardPile;

    [Header("Data")]
    public DeckData baseDeck;
    public int drawAmount;
    public int drawAmountFirstTurn;


    public void Start()
    {
        baseDeck = Instantiate(baseDeck);

        //Fill deck from base data
        for (int i = 0; i < baseDeck.deck.Count; i++)
        {
            cardDeck.Add(baseDeck.deck[i]);
        }

        //Init card data
        for (int i = 0; i < cardDeck.Count; i++)
        {
            cardDeck[i] = cardDeck[i].InitializeData(cardDeck[i]);
        }

        ShuffleCards(cardDeck);
    }

    public List<CardData> ShuffleCards(List<CardData> deckToShuffle) //FisherYates Shuffle
    {
        int n = deckToShuffle.Count;
        for (int i = 0; i < (n - 1); i++)
        {
            int r = i + Random.Range(0, n-i);
            CardData t = deckToShuffle[r];
            deckToShuffle[r] = deckToShuffle[i];
            deckToShuffle[i] = t;
        }
        return deckToShuffle;
    }

    public void DrawCards(int count, EventQueue queue)
    {
        queue.events.Add(DrawCardsRoutine(count, queue));
    }
    IEnumerator DrawCardsRoutine(int count, EventQueue queue)
    {
        int cardsInDeck = cardDeck.Count;
        int numberOfCardsInHand = CardManager.Instance.cardHand.currentHand.Count;

        EventQueue dealQueue = new EventQueue();

        for (int i = 0; i < count; i++)
        {
            if (cardsInDeck > 0)//Deal card while deck is not empty
            {
                //TODO deal or burn
                #region DEAL/BURN
                if(numberOfCardsInHand + i + 1 > CardManager.Instance.cardHand.maxHandSize)
                {
                    OverDraw(dealQueue);
                }
                else
                {
                    Deal(dealQueue);
                }


                cardsInDeck--;
                #endregion
            }
            else //Get card from discrad back to deck
            {
                #region Refill
                if (discardPile.Count - i > 0)//NOTE : DOES THIS EVEN WORK?
                {
                    Refill(dealQueue);
                    cardsInDeck += discardPile.Count;
                    i--;
                }
                else break;
                #endregion
            }
        }

        dealQueue.StartQueue(); //actual deal / burn /shuffle

        while(!dealQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        //Add queue.Update on optional queue
        if (queue != null) queue.UpdateQueue();
    }

    #region Utility
    void Refill(EventQueue queue)
    {
        queue.events.Add(RefillDeck(queue));
    }
    IEnumerator RefillDeck(EventQueue queue)
    {
        EventQueue refillQueue = new EventQueue();

        //TODO make the following logic in the queue so it can be animated-----------------
        for (int j = 0; j < discardPile.Count; j++)
        {
            cardDeck.Add(discardPile[j]);
        }
        discardPile.Clear();
        //--------------------------------------

        refillQueue.StartQueue();

        while (!refillQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        //All the discarded cards ar back in the deck now u can shuffle
        ShuffleCards(cardDeck);
        Debug.LogError("Shuffling discard pile in deck");

        queue.UpdateQueue();
    }

    public void Deal(EventQueue queue, CardData card = null)
    {
        queue.events.Add(DealRoutine(queue, card));
    }
    IEnumerator DealRoutine(EventQueue queue, CardData card)
    {
        CardData dealtCard;

        if (card != null)
        {
            dealtCard = card;
        }
        else
        {
            dealtCard = cardDeck[0];
        }

        #region Event OnCardDraw
        EventQueue onCardDrawQueue = new EventQueue();
        if (dealtCard.onCardDraw != null)
            dealtCard.onCardDraw(onCardDrawQueue, dealtCard);
        onCardDrawQueue.StartQueue();
        while(!onCardDrawQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }
        #endregion

        #region Event OnAnyCardDrawn (Illumination)
        EventQueue illuminationQueue = new EventQueue();

        CardManager.Instance.board.CallBoardEvents("illumination", illuminationQueue);

        illuminationQueue.StartQueue();
        while(!illuminationQueue.resolved)
        { yield return new WaitForEndOfFrame(); }
        #endregion

        EventQueue dealQueue = new EventQueue();

        //TODO make the following logic in the queue so it can be animated-----------------
        
        EventQueue initQueue = new EventQueue();

        CardManager.Instance.CardDeckToHand(initQueue, dealtCard, dealtCard.currentContainer != null);

        initQueue.StartQueue();
        while (!initQueue.resolved) { yield return new WaitForEndOfFrame(); }

        if(card == null)//Only remove if card = null
        {
            cardDeck.RemoveAt(0);
        }
        else
        {
            if (cardDeck.Contains(card)) cardDeck.Remove(card);
        }
        //----------------------------------------------------

        dealQueue.StartQueue();

        while(!dealQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        queue.UpdateQueue();
    }

    public void OverDraw(EventQueue queue, CardData card = null)
    {
        queue.events.Add(OverDrawRoutine(queue, card));   
    }
    IEnumerator OverDrawRoutine(EventQueue queue, CardData card)
    {
        yield return null;
        EventQueue overdrawQueue = new EventQueue();

        CardData dealtCard;

        if (card != null)
        {
            dealtCard = card;
        }
        else
        {
            dealtCard = cardDeck[0];
        }
        //Appear(overDrawQueue)//TODO
        if (dealtCard.currentContainer == null)
            CardManager.Instance.CardAppear(overdrawQueue, dealtCard, CardManager.Instance.deckTransform.localPosition);

        //the card can be burn or push another card from the board
        if (dealtCard.GetType() == typeof(PlotCard)) //if its a plot card it pushes cards from the board
        {
            EventQueue pickQueue = new EventQueue();
            List<CardData> pickedCards = new List<CardData>();

            CardManager.Instance.cardPicker.Pick(pickQueue,CardManager.Instance.cardHand.GetHandDataList(),pickedCards, 1, true, "Choose a card to discard");

            pickQueue.StartQueue();
            while(!pickQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }


            //Discard the pickedCards and deal the new one
            for (int i = 0; i < pickedCards.Count; i++)
            {
                CardManager.Instance.CardHandToDiscard(pickedCards[i].currentContainer, overdrawQueue);

            }
            Deal(overdrawQueue, dealtCard);
        }
        else
        {
            Burn(overdrawQueue, dealtCard);
        }

        overdrawQueue.StartQueue();
        while(!overdrawQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }
        
        queue.UpdateQueue();
    }

    void Burn(EventQueue queue, CardData card)
    {
        queue.events.Add(BurnRoutine(queue, card));
    } 
    IEnumerator BurnRoutine(EventQueue queue, CardData card)
    {
        yield return null;

        EventQueue burnQueue = new EventQueue();

        //TODO implement this to queue so it can be animated --- 

        //Add feedback
        EventQueue feedback = new EventQueue();
        CardManager.Instance.cardTweening.MoveCard(card.currentContainer, CardManager.Instance.discardPileTransform.localPosition, true, false, feedback);
        while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }


        discardPile.Add(card);
        if (CardManager.Instance.cardHand.currentHand.Contains(card.currentContainer)) CardManager.Instance.cardHand.currentHand.Remove(card.currentContainer);
        if(cardDeck.Contains(card))cardDeck.Remove(card);

        //------------------------------------------------------

        burnQueue.StartQueue();
        while(!burnQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        queue.UpdateQueue();
    }
    #endregion
}
