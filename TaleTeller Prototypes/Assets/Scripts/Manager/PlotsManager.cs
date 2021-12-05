using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotsManager : Singleton<PlotsManager>
{

    public List<CardData> secondaryPlots = new List<CardData>();
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

    }

    //Link this method to the act beggining TODO
    public void ChooseMainPlot(EventQueue queue, List<MainPlotScheme> schemesToChooseFrom)
    {
        queue.events.Add(ChooseMainPlotRoutine(queue, schemesToChooseFrom));
    }
    IEnumerator ChooseMainPlotRoutine(EventQueue queue,List<MainPlotScheme> schemes)
    {
        EventQueue pickQueue = new EventQueue();
        List<CardData> pickTargets = new List<CardData>();
        List<CardData> pickedCard = new List<CardData>();
        for (int i = 0; i < schemes.Count; i++)
        {
            pickTargets.Add(schemes[i].schemeSteps[0].stepOptions[0]);
        }

        CardManager.Instance.cardPicker.Pick(pickQueue, pickTargets, pickedCard, 1, false);

        pickQueue.StartQueue();
        while(!queue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        int index = pickTargets.IndexOf(pickedCard[0]);

        //Load MainScheme
        EventQueue loadQueue = new EventQueue();

        schemes[index].InitScheme(schemes[index]);
        schemes[index].LoadStep(loadQueue, schemes[index]);

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

        CardManager.Instance.cardPicker.Pick(pickQueue, secondaryPlots, pickedCards, 1, false);

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
                pickedCards[i].onCardAppear(appearQueue); //This manages the appear animation + all the junk apparition
            }
        }
        else//If the picked card is null send a random plot card to deck
        {
            int r = Random.Range(0, secondaryPlots.Count - 1);

            PlotCard card = secondaryPlots[r] as PlotCard;
            card.onCardAppear -= card.OnPlotAppear; //Unsubscribe from the onAppear event since it wont gbe useful later
            card.onCardDraw += card.onCardAppear;//Subscribe to the onDraw event to spawn correctly the junk cards;

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

        CardManager.Instance.cardDeck.cardDeck.Add(card);

        queue.UpdateQueue();
    }
    #endregion
}
