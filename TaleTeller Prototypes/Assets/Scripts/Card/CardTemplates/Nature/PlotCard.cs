using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plot Card", menuName = "Data/PlotCard")]
public class PlotCard : CardData
{
    [Expandable]
    public PlotObjective objective;
    public int completionTimer;
    public bool isMainPlot;
    public bool isFinal;
    public List<CardData> legendaryCardRewards = new List<CardData>();

    //Eventually CardData malusCardToSpawn 

    //TEMP
    public override CardData InitializeData(CardData data)
    {
        PlotCard plot = Instantiate(data.dataReference) as PlotCard;//make data an instance of itself
        plot.dataReference = data.dataReference;

        data.currentContainer = null;

        //Write logic to determine how the card subscribe to the events
        if (data.cardTypeReference != null)
        {
            CardTypes type = Instantiate(data.cardTypeReference);
            plot.cardType = type;
            plot.cardTypeReference = data.cardTypeReference;
            
            plot.cardType.InitType(plot);//<--Watch out, subscribing to events can happen in here
        }
        else //All the events that i subscribe in here must be the one that are overidden if I have a certain cardType
        {
            InitializeCardEffects(plot);
        }

        plot.onStoryEnd += plot.OnEndPlot;
        plot.onTurnEnd += plot.UpdateTimer;
        plot.onCardAppear += plot.OnPlotAppear;


        plot.objective = Instantiate(plot.objective);
        plot.objective.InitObjective(plot, plot.objective);


        //Init legendary rewards
        for (int i = 0; i < plot.legendaryCardRewards.Count; i++)
        {
            plot.legendaryCardRewards[i].InitializeData(plot.legendaryCardRewards[i]);
        }


        return plot;
    }


    public void OnPlotAppear(EventQueue queue, CardData data)
    {
        queue.events.Add(OnPlotAppearRoutine(queue, data));
    }
    IEnumerator OnPlotAppearRoutine(EventQueue currentQueue, CardData data)
    {
        yield return null;

        //if deck dont contains this card then animate to hand
        if(!CardManager.Instance.cardDeck.cardDeck.Contains(PlotsManager.Instance.currentPickedCard))
        {
            EventQueue toHandQueue = new EventQueue();

            PlotsManager.Instance.SendPlotToHand(toHandQueue, PlotsManager.Instance.currentPickedCard);

            toHandQueue.StartQueue();
            while(!toHandQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }


        //add all junk cards to deck for now -- TODO later call the appropriate method on plotObjective so it chooses where to send the cards TODO 
        for (int i = 0; i < objective.linkedJunkedCards.Count; i++)
        {
            //objective.linkedJunkedCards[i] = objective.linkedJunkedCards[i].InitializeData(objective.linkedJunkedCards[i]) as JunkCard;
            CardManager.Instance.cardDeck.cardDeck.Add(objective.linkedJunkedCards[i]);
        }


        currentQueue.UpdateQueue();
    }

    public void OnEndPlot(EventQueue queue)
    {
        queue.events.Add(OnEndPlotRoutine(queue));
    }
    IEnumerator OnEndPlotRoutine(EventQueue currentQueue)
    {
        yield return null;
        EventQueue updateCardStatusQueue = new EventQueue();

        CardManager.Instance.board.ReturnCardToHand(currentContainer, true, updateCardStatusQueue);

        updateCardStatusQueue.StartQueue();
        while(!updateCardStatusQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }


        currentQueue.UpdateQueue();
    }

    public void OnEndPlotComplete(EventQueue queue)
    {
        queue.events.Add(OnEndPlotCompleteRoutine(queue));
    }
    IEnumerator OnEndPlotCompleteRoutine(EventQueue currentQueue)
    {
        
        //Destroy
        currentContainer.ResetContainer();
        StoryManager.Instance.cardsToDestroy.Add(this);

        //Choose next card choose next card if not last //TODO
        if(PlotsManager.Instance.currentMainPlotScheme.currentStep < PlotsManager.Instance.currentMainPlotScheme.schemeSteps.Count)
        {
            EventQueue updateQueue = new EventQueue();
            PlotsManager.Instance.currentMainPlotScheme.UpdateScheme(updateQueue, PlotsManager.Instance.currentMainPlotScheme);

            updateQueue.StartQueue();
            while(!updateQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else //If last then go to next acte
        {
            Debug.LogError("NORMALEMENT C'EST LA FIN DE l'ACTE");
        }

        currentQueue.UpdateQueue();
    }

    public void CompletePlot(EventQueue queue)
    {
        queue.events.Add(CompletePlotRoutine(queue));
    }
    IEnumerator CompletePlotRoutine(EventQueue currentQueue)
    {
        yield return null;

        //destroy all linked junk cards
        EventQueue destroyQueue = new EventQueue();

        objective.DestroyJunk(destroyQueue);

        destroyQueue.StartQueue();

        while(!destroyQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        //Reward
        //TODO IMPLEMENT QUEUEING IN HERE
        EventQueue rewardQueue = new EventQueue();

        if (isMainPlot)
        {
            if (isFinal)
            {
                //Final Step of main plot reward
                Debug.Log("Complete Final main plot");
                RewardManager.Instance.ChooseMainPlotRewardFinal(rewardQueue, this);
            }
            else
            {
                //Main Plot reward
                Debug.Log("Complete main plot");
                RewardManager.Instance.ChooseMainPlotReward(rewardQueue, this);
            }
        }
        else
        {
            //Secondary plot reward
            Debug.Log("Secondary Plot");
            RewardManager.Instance.ChooseSecondaryPlotReward(rewardQueue, this);
        }
        rewardQueue.StartQueue();
        while(!rewardQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }
        
        //Send to oblivion or wait for end of story to send to oblivion and pickj new plot scheme //TODO implement queuing
        if(isMainPlot)
        {
            //unsubscribe to basic onEndTurn
            onStoryEnd -= OnEndPlot;
            onStoryEnd += OnEndPlotComplete;
        }
        else
        {
            //implement queuing so it takes time
            currentContainer.ResetContainer();
            StoryManager.Instance.cardsToDestroy.Add(this);
        }

        currentQueue.UpdateQueue();
    }

    public void FailPlot(EventQueue queue)
    {
        queue.events.Add(FailPilotRoutine(queue));
    }
    public IEnumerator FailPilotRoutine(EventQueue currentQueue)//TODO
    {
        yield return null;
        if (isMainPlot)
        {
            //GameOver

        }
        else
        {
            //Add malus card to player discard pile

            //DestroyCard
        }

        currentQueue.UpdateQueue();
    }

    public void UpdateTimer(EventQueue queue)
    {
        queue.events.Add(UpdateTimerRoutine(queue));
    }
    IEnumerator UpdateTimerRoutine(EventQueue currentQueue)
    {
        yield return null;
        completionTimer--;
        if(completionTimer <= 0)
        {
            EventQueue failQueue = new EventQueue();
            FailPlot(failQueue);

            failQueue.StartQueue();
            while(!failQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        currentQueue.UpdateQueue();
    }
}
