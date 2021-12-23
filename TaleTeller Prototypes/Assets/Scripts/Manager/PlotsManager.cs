using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotsManager : Singleton<PlotsManager>
{
    public MainPlotScheme currentMainPlotScheme;
    public List<MainPlotScheme> schemes = new List<MainPlotScheme>();
    public List<CardData> secondaryPlots = new List<CardData>();
    public List<CardData> darkIdeas = new List<CardData>();

    [HideInInspector] public CardData currentPickedCard;

    private void Awake()
    {
        CreateSingleton();
    }

    public void Start()
    {
        //InitData
        for (int i = 0; i < secondaryPlots.Count; i++)
        {
            secondaryPlots[i] = secondaryPlots[i].InitializeData(secondaryPlots[i]); 
        }
        for (int i = 0; i < darkIdeas.Count; i++)
        {
            darkIdeas[i] = darkIdeas[i].InitializeData(darkIdeas[i]);

        }

    }

    //Link this method to the act beggining TODO
    public void ChooseMainPlot(EventQueue queue, List<MainPlotScheme> schemesToChooseFrom)
    {
        queue.events.Add(ChooseMainPlotRoutine(queue, schemesToChooseFrom));
    }
    IEnumerator ChooseMainPlotRoutine(EventQueue queue, List<MainPlotScheme> schemes)
    {
        List<MainPlotScheme> chosenScheme = new List<MainPlotScheme>();

        EventQueue pickQueue = new EventQueue();
        CardManager.Instance.cardPicker.PickScheme(pickQueue, schemes, chosenScheme);

        pickQueue.StartQueue();
        while (!pickQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        //Load MainScheme
        EventQueue loadQueue = new EventQueue();

        currentMainPlotScheme = chosenScheme[0];
        currentMainPlotScheme = currentMainPlotScheme.InitScheme(currentMainPlotScheme); //Note maybe leave the shemes list untouched and only instantiate a copy

        currentMainPlotScheme.LoadStep(loadQueue, currentMainPlotScheme);

        loadQueue.StartQueue();
        
        while(!loadQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        queue.UpdateQueue();
    }


    //Link this method to the event occuring every 2-3 turns TODO
    public void ChooseSecondaryPlots(EventQueue queue)
    {
        queue.events.Add(ChooseSecondaryPlotsRoutine(queue));
    }
    IEnumerator ChooseSecondaryPlotsRoutine(EventQueue queue)
    {
        EventQueue pickQueue = new EventQueue();
        List<CardData> pickedCards = new List<CardData>();

        CardManager.Instance.cardPicker.Pick(pickQueue, secondaryPlots, pickedCards, 1, false, "Choose a secondary plot");

        pickQueue.StartQueue();

        while(!pickQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        EventQueue appearQueue = new EventQueue();

        //Send the picked card to hand and init 
        if(pickedCards.Count != 0)
        {
            for (int i = 0; i < pickedCards.Count; i++)
            {
                currentPickedCard = pickedCards[i]; //<-- This doesn't work if more than one pickedCard TODO fix this
                pickedCards[i].onCardAppear(appearQueue, pickedCards[i]); //This manages the appear animation + all the junk apparition

                secondaryPlots.Remove(pickedCards[i]); //TEMP  
            }

        }
        else//If the picked card is null send a random plot card to deck
        {
            int r = Random.Range(0, secondaryPlots.Count - 1);

            PlotCard card = secondaryPlots[r] as PlotCard;
            card.onCardAppear -= card.OnPlotAppear; //Unsubscribe from the onAppear event since it wont gbe useful later

            card.onCardDraw += card.OnPlotAppear;//Subscribe to the onDraw event to spawn correctly the junk cards;

            //animate card to deck
            //for now only add it to deck list
            SendPlotToDeck(appearQueue, card);
        }


        appearQueue.StartQueue();

        while(!appearQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        queue.UpdateQueue();
    }


    #region Utility
    public void SendPlotToHand(EventQueue queue, CardData card)
    {
        queue.events.Add(SendPlotToHandRoutine(queue, card));
    }
    IEnumerator SendPlotToHandRoutine(EventQueue queue, CardData card)
    {
        yield return null;

        EventQueue toHandQueue = new EventQueue();
        //Make the card appear
        PlotAppear(toHandQueue, card);

        if(CardManager.Instance.cardHand.currentHand.Count + 1 > CardManager.Instance.cardHand.maxHandSize)
        {
            //Overdraw
            CardManager.Instance.cardDeck.OverDraw(toHandQueue, card);
        }
        else
        {
            //Deal
            CardManager.Instance.cardDeck.Deal(toHandQueue, card);
        }

        toHandQueue.StartQueue();
        while(!toHandQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        queue.UpdateQueue();
    }

    public void SendPlotToDeck(EventQueue queue, CardData card)
    {
        queue.events.Add(SendPlotToDeckRoutine(queue, card));
    }
    IEnumerator SendPlotToDeckRoutine(EventQueue queue, CardData card)
    {
        yield return null;

        //Make the card appear and send it to the deck
        EventQueue appearQueue = new EventQueue();
        PlotAppear(appearQueue, card);
        PlotToDeck(appearQueue, card);
        appearQueue.StartQueue();
        while (!appearQueue.resolved) { yield return new WaitForEndOfFrame(); }

        if(!CardManager.Instance.cardDeck.cardDeck.Contains(card)) CardManager.Instance.cardDeck.cardDeck.Add(card);

        queue.UpdateQueue();
    }

    public void PlotAppear(EventQueue queue, CardData card)
    {
        queue.events.Add(PlotAppearRoutine(queue, card));
    }
    IEnumerator PlotAppearRoutine(EventQueue queue, CardData card)
    {
        #region Init Container
        EventQueue initQueue = new EventQueue();
        CardManager.Instance.cardHand.InitCard(initQueue, card, false);
        initQueue.StartQueue();
        while (!initQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion
        card.currentContainer.rectTransform.localPosition = CardManager.Instance.plotAppearTransform.localPosition;

        EventQueue appearFeedback = new EventQueue();
        CardManager.Instance.cardTweening.MoveCard(card.currentContainer, CardManager.Instance.plotAppearTransform.localPosition, true, true, appearFeedback);
        while (!appearFeedback.resolved) { yield return new WaitForEndOfFrame(); }


        queue.UpdateQueue();
    }

    public void PlotToDeck(EventQueue queue, CardData card)
    {
        queue.events.Add(PlotToDeckRoutine(queue, card));
    }
    IEnumerator PlotToDeckRoutine(EventQueue queue, CardData card)
    {

        EventQueue toDeckFeedback = new EventQueue();
        CardManager.Instance.cardTweening.MoveCard(card.currentContainer, CardManager.Instance.deckTransform.localPosition, true, false, toDeckFeedback);
        while (!toDeckFeedback.resolved) { yield return new WaitForEndOfFrame(); }

        card.currentContainer.ResetContainer();
        queue.UpdateQueue();
    }

    #endregion

}
