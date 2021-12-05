using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<CardData> cardDeck;
    public List<CardData> discardPile;

    [Header("Data")]
    public int drawAmount;
    public int drawAmountFirstTurn;


    public void Start()
    {
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

    public void DrawCards(int count, EventQueue queue = null)
    {
        StartCoroutine(DrawCardsRoutine(count, queue));
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
                if (discardPile.Count != 0)
                {
                    Refill(dealQueue);
                    cardsInDeck += discardPile.Count;
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
        EventQueue dealQueue = new EventQueue();
        CardData dealtCard;

        if (card != null) dealtCard = card;
        else dealtCard = cardDeck[0];

        //TODO make the following logic in the queue so it can be animated-----------------
        CardManager.Instance.cardHand.InitCard(dealtCard);
        cardDeck.RemoveAt(0);
        yield return new WaitForSeconds(0.2f);//For now Wait 0.2 sec between each draw
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

        if (card != null) dealtCard = card;
        else dealtCard = cardDeck[0];

        //Appear(overDrawQueue)//TODO


        //the card can be burn or push another card from the board
        if (dealtCard.GetType() == typeof(PlotCard)) //if its a plot card it pushes cards from the board
        {

            EventQueue pickQueue = new EventQueue();
            List<CardData> pickedCards = new List<CardData>();

            CardManager.Instance.cardPicker.Pick(pickQueue,CardManager.Instance.cardHand.GetHandDataList(),pickedCards, 1, true);

            pickQueue.StartQueue();
            while(!pickQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }


            //Discard the pickedCards and deal the new one
            for (int i = 0; i < pickedCards.Count; i++)
            {
                CardManager.Instance.cardHand.DiscardCardFromHand(pickedCards[i].currentContainer, overdrawQueue);
            }
            Deal(overdrawQueue, dealtCard);
        }
        else
        {
            Burn(overdrawQueue);
        }

        overdrawQueue.StartQueue();
        while(!overdrawQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }
        
        queue.UpdateQueue();
    }

    void Burn(EventQueue queue)
    {
        queue.events.Add(BurnRoutine(queue));
    } 
    IEnumerator BurnRoutine(EventQueue queue)
    {
        yield return null;

        EventQueue burnQueue = new EventQueue();

        //TODO implement this to queue so it can be animated --- 

        discardPile.Add(cardDeck[0]);
        cardDeck.RemoveAt(0);

        //------------------------------------------------------

        burnQueue.StartQueue();
        while(!burnQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        queue.UpdateQueue();
    }

    //void DiscardFromDeck(EventQueue queue)
    //{
    //    queue.events.Add(DiscardFromDeckRoutine(queue));
    //}
    //IEnumerator DiscardFromDeckRoutine(EventQueue queue)
    //{
    //    yield return null;


    //}
    #endregion
}
