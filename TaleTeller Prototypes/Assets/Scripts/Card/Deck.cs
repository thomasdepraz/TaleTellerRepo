using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Deck : MonoBehaviour
{
    public List<CardData> cardDeck;
    public List<CardData> cachedDeck;
    public List<CardData> discardPile;

    [Header("Data")]
    public DeckData baseDeck;
    public int drawAmount;
    public int drawAmountFirstTurn;

    [Header("Debug")]
    public bool enableDebugDeck;
    [ShowIf("enableDebugDeck")]
    [Range(1,3)] public int actToTest = 1;

    [Header("Visuals")]
    public Image deckHighlight;
    public Image discardHighlight;

    public void Awake()
    {
        if(enableDebugDeck)
        {
            
        }
    }

    public void Start()
    {
        baseDeck = Instantiate(baseDeck);

        //Fill deck from base data
        for (int i = 0; i < baseDeck.deck.Count; i++)
        {
            cardDeck.Add(baseDeck.deck[i]);
        }

        #region DebugDeck
        if (enableDebugDeck)
        {
            StoryManager.Instance.actCount = actToTest - 1;

            if (actToTest >= 2)
            {
                int numCardsToRemove = Random.Range(0, 3);

                for (int i = 0; i < numCardsToRemove; i++)
                {
                    cardDeck.RemoveAt(Random.Range(0, cardDeck.Count));
                }

                if(actToTest >= 3)
                {
                    for (int i = 0; i < numCardsToRemove; i++)
                    {
                        cardDeck.RemoveAt(Random.Range(0, cardDeck.Count));
                    }
                }

                int numCardsToAdd = Random.Range(6, 8);

                int numCommonCards = Mathf.RoundToInt(numCardsToAdd * 0.55f);

                List<CardData> commonCards = RewardManager.Instance.GetMainPlotRewardsSecondBatch(RewardManager.Instance.rewardPoolAllIdeas, numCommonCards, CardRarity.Common);

                for(int i = 0; i < commonCards.Count; i++)
                {
                    cardDeck.Add(commonCards[i]);
                }

                int numUncommonCards = Mathf.RoundToInt(numCardsToAdd * 0.30f);

                List<CardData> uncommonCards = RewardManager.Instance.GetMainPlotRewardsSecondBatch(RewardManager.Instance.rewardPoolAllIdeas, numUncommonCards, CardRarity.Uncommon);

                for (int i = 0; i < uncommonCards.Count; i++)
                {
                    cardDeck.Add(uncommonCards[i]);
                }

                int numRareCards = Mathf.RoundToInt(numCardsToAdd * 0.15f);

                List<CardData> rareCards = RewardManager.Instance.GetMainPlotRewardsSecondBatch(RewardManager.Instance.rewardPoolAllIdeas, numRareCards, CardRarity.Rare);

                for (int i = 0; i < rareCards.Count; i++)
                {
                    cardDeck.Add(rareCards[i]);
                }

                if(actToTest >= 3)
                {
                    numCardsToAdd = Random.Range(6, 8);

                    numCommonCards = Mathf.RoundToInt(numCardsToAdd * 0.30f);

                    commonCards = RewardManager.Instance.GetMainPlotRewardsSecondBatch(RewardManager.Instance.rewardPoolAllIdeas, numCommonCards, CardRarity.Common);

                    for (int i = 0; i < commonCards.Count; i++)
                    {
                        cardDeck.Add(commonCards[i]);
                    }

                    numUncommonCards = Mathf.RoundToInt(numCardsToAdd * 0.40f);

                    uncommonCards = RewardManager.Instance.GetMainPlotRewardsSecondBatch(RewardManager.Instance.rewardPoolAllIdeas, numUncommonCards, CardRarity.Uncommon);

                    for (int i = 0; i < uncommonCards.Count; i++)
                    {
                        cardDeck.Add(uncommonCards[i]);
                    }

                    numRareCards = Mathf.RoundToInt(numCardsToAdd * 0.20f);

                    rareCards = RewardManager.Instance.GetMainPlotRewardsSecondBatch(RewardManager.Instance.rewardPoolAllIdeas, numRareCards, CardRarity.Rare);

                    for (int i = 0; i < rareCards.Count; i++)
                    {
                        cardDeck.Add(rareCards[i]);
                    }

                    int numEpicCards = Mathf.RoundToInt(numCardsToAdd * 0.10f);

                    List<CardData> epicCards = RewardManager.Instance.GetMainPlotRewardsSecondBatch(RewardManager.Instance.rewardPoolAllIdeas, numRareCards, CardRarity.Epic);

                    for (int i = 0; i < epicCards.Count; i++)
                    {
                        cardDeck.Add(epicCards[i]);
                    }
                }

            }
        }
        #endregion

        //Copy Cards to cachedDeck
        for (int i = 0; i < cardDeck.Count; i++)
        {
            cachedDeck.Add(cardDeck[i]);
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
        int numberOfCardsInHand = CardManager.Instance.cardHand.GetHandCount();

        EventQueue dealQueue = new EventQueue();

        for (int i = 0; i < count; i++)
        {
            if (cardsInDeck > 0)//Deal card while deck is not empty
            {
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
    public void Refill(EventQueue queue)
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

        if (dealtCard.currentContainer == null)
            CardManager.Instance.CardAppear(overdrawQueue, dealtCard, CardManager.Instance.deckTransform.position);

        //the card can be burn or push another card from the board
        //if (dealtCard.GetType() == typeof(PlotCard) || dealtCard.GetType() == typeof(JunkCard)) //if its a plot card it pushes cards from the board
        //{
            //EventQueue pickQueue = new EventQueue();
            //List<CardData> pickedCards = new List<CardData>();

            //string instruction = LocalizationManager.Instance.GetString(LocalizationManager.Instance.instructionsDictionary, GameManager.Instance.instructionsData.chooseCardInstruction);
            //CardManager.Instance.cardPicker.Pick(pickQueue,CardManager.Instance.cardHand.GetHandDataList(),pickedCards, 1, instruction);

            //pickQueue.StartQueue();
            //while(!pickQueue.resolved)
            //{
            //    yield return new WaitForEndOfFrame();
            //}

            CardPickerScreen screen = new CardPickerScreen(PickScreenMode.REPLACE, 1, CardManager.Instance.cardHand.GetHandDataList(), true);
            bool wait = true;
            screen.Open(() => wait = false);
            while (wait) { yield return new WaitForEndOfFrame(); }

            while (screen.open) { yield return new WaitForEndOfFrame(); }
            wait = true;
            screen.Close(() => wait = false);
            while (wait) { yield return new WaitForEndOfFrame(); }
            //Discard the pickedCards and deal the new one
            for (int i = 0; i < screen.pickedCards.Count; i++)
            {
                CardManager.Instance.CardHandToDiscard(screen.pickedCards[i].container.data.currentContainer, overdrawQueue);

            }
            Deal(overdrawQueue, dealtCard);
        /*}
        else
        {
            Burn(overdrawQueue, dealtCard);
        }*/

        overdrawQueue.StartQueue();
        while(!overdrawQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }
        
        queue.UpdateQueue();
    }

    public void Burn(EventQueue queue, CardData card)
    {
        queue.events.Add(BurnRoutine(queue, card));
    } 
    IEnumerator BurnRoutine(EventQueue queue, CardData card)
    {
        yield return null;

        #region Event OnCardDiscard
        EventQueue onCardDiscardQueue = new EventQueue();

        if (card.onCardDiscard != null)
            card.onCardDiscard(onCardDiscardQueue, card);

        onCardDiscardQueue.StartQueue();
        while (!onCardDiscardQueue.resolved)
        { yield return new WaitForEndOfFrame(); }
        #endregion

        #region Event OnAnyCardDiscard (Overload)
        EventQueue overloadQueue = new EventQueue();
        CardManager.Instance.board.CallBoardEvents("overload", overloadQueue);

        overloadQueue.StartQueue();
        while (!overloadQueue.resolved)
        { yield return new WaitForEndOfFrame(); }
        #endregion

        EventQueue burnQueue = new EventQueue();

        //Add feedback
        EventQueue feedback = new EventQueue();
        CardManager.Instance.cardTweening.MoveCard(card.currentContainer, CardManager.Instance.discardPileTransform.position, true, false, feedback);
        while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }


        if (CardManager.Instance.cardHand.currentHand.Contains(card.currentContainer))
        {
            CardManager.Instance.cardHand.currentHand.Remove(card.currentContainer);
            CardManager.Instance.UpdateHandCount();
        }
        if(cardDeck.Contains(card))cardDeck.Remove(card);

        card.currentContainer.ResetContainer();

        card = card.ResetData(card);
        discardPile.Add(card);


        burnQueue.StartQueue();
        while(!burnQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        queue.UpdateQueue();
    }
    #endregion

    public void InitCachedDeck()
    {
        for (int i = 0; i < cachedDeck.Count; i++)
        {
            cachedDeck[i] = cachedDeck[i].InitializeData(cachedDeck[i]);
        }
    }
    public void ResetCachedDeck()
    {
        for (int i = 0; i < cachedDeck.Count; i++)
        {
            cachedDeck[i] = cachedDeck[i].dataReference;
            cardDeck.Add(cachedDeck[i].dataReference);
        }
        for (int i = 0; i < cardDeck.Count; i++)
        {
            cardDeck[i] = cardDeck[i].InitializeData(cardDeck[i]);
        }
    }
}
