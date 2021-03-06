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
    [TextArea(2, 3)] public string plotChoiceDescription;
    public List<CardData> legendaryCardRewards = new List<CardData>();

    public override CardData InitializeData(CardData data)
    {
        PlotCard plot = Instantiate(data.dataReference) as PlotCard;//make data an instance of itself
        plot.dataReference = data.dataReference;

        data.currentContainer = null;

        //load text 
        if(plotChoiceDescription!=string.Empty)
            plot.plotChoiceDescription = LocalizationManager.Instance.GetString(LocalizationManager.Instance.schemesDescriptionsDictionary, plot.plotChoiceDescription);

        plot.archetype = (Archetype)Random.Range(2, (int)Archetype.NumberOfArchetypes );
        
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
            plot.legendaryCardRewards[i] = plot.legendaryCardRewards[i].InitializeData(plot.legendaryCardRewards[i]);
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
        if(!CardManager.Instance.cardDeck.cardDeck.Contains(data))
        {
            EventQueue toHandQueue = new EventQueue();

            CardManager.Instance.CardAppearToHand(data, toHandQueue, CardManager.Instance.plotAppearTransform.position);

            toHandQueue.StartQueue();
            while(!toHandQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        //Refill deck 
        EventQueue refillQueue = new EventQueue();
        CardManager.Instance.cardDeck.Refill(refillQueue);
        refillQueue.StartQueue();
        while (!refillQueue.resolved) { yield return new WaitForEndOfFrame(); }


        //add all junk cards to deck for now
        for (int i = 0; i < objective.linkedJunkedCards.Count; i++)
        {
            switch(objective.junkSpawnLocations[i])
            {
                case PlotObjective.JunkSpawnLocation.EndDeck:
                    CardManager.Instance.cardDeck.cardDeck.Add(objective.linkedJunkedCards[i]);
                    
                    EventQueue endDeckfeedback = new EventQueue();
                    CardManager.Instance.CardAppearToDeck(objective.linkedJunkedCards[i], endDeckfeedback, CardManager.Instance.plotAppearTransform.position, false);
                    endDeckfeedback.StartQueue();
                    while(!endDeckfeedback.resolved) { yield return new WaitForEndOfFrame(); }


                    break;
                case PlotObjective.JunkSpawnLocation.DeckRandom:
                    CardManager.Instance.cardDeck.cardDeck.Insert(Random.Range(0, CardManager.Instance.cardDeck.cardDeck.Count),objective.linkedJunkedCards[i]);

                    EventQueue deckRandomFeedback = new EventQueue();
                    CardManager.Instance.CardAppearToDeck(objective.linkedJunkedCards[i], deckRandomFeedback, CardManager.Instance.plotAppearTransform.position, false);

                    deckRandomFeedback.StartQueue();
                    while (!deckRandomFeedback.resolved) { yield return new WaitForEndOfFrame();}


                    break;
                case PlotObjective.JunkSpawnLocation.XInDeck:
                    int r = Random.Range(objective.junksPositionsInDeck[i] - 2, objective.junksPositionsInDeck[i] + 2);
                    
                    if(r < 0)
                        CardManager.Instance.cardDeck.cardDeck.Insert(0, objective.linkedJunkedCards[i]);
                    else if (r > CardManager.Instance.cardDeck.cardDeck.Count)
                        CardManager.Instance.cardDeck.cardDeck.Insert(CardManager.Instance.cardDeck.cardDeck.Count, objective.linkedJunkedCards[i]);
                    else
                        CardManager.Instance.cardDeck.cardDeck.Insert(r, objective.linkedJunkedCards[i]);

                    EventQueue xInDeckFeedback = new EventQueue();
                    CardManager.Instance.CardAppearToDeck(objective.linkedJunkedCards[i], xInDeckFeedback, CardManager.Instance.plotAppearTransform.position, false);
                    xInDeckFeedback.StartQueue();
                    while (!xInDeckFeedback.resolved) { yield return new WaitForEndOfFrame(); }


                    break;
                case PlotObjective.JunkSpawnLocation.Hand:

                    EventQueue drawQueue = new EventQueue();

                    CardManager.Instance.CardAppearToHand(objective.linkedJunkedCards[i], drawQueue, CardManager.Instance.plotAppearTransform.position);

                    drawQueue.StartQueue();//Actual draw
                    while (!drawQueue.resolved)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    break;
                case PlotObjective.JunkSpawnLocation.FromCard:

                    for(int x = i; x >= 0; x--)
                    {
                        if(objective.junkSpawnLocations[x] != PlotObjective.JunkSpawnLocation.FromCard)
                        {
                            for(int z = 0; z < objective.linkedJunkedCards[x].effects.Count; z++)
                            {
                                if(objective.linkedJunkedCards[x].effects[z].GetType() == typeof(AddJunkToHandEffect))
                                {
                                    AddJunkToHandEffect junkToHandEffect = objective.linkedJunkedCards[x].effects[z] as AddJunkToHandEffect;

                                    junkToHandEffect.junkCardsToSpawn.Add(objective.linkedJunkedCards[i]);
                                }
                            }

                            break;
                        }
                    }

                    break;
            }
            
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

        CardManager.Instance.CardBoardToHand(currentContainer, true, updateCardStatusQueue);

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

        //destroy all linked junk cards
        EventQueue destroyQueue = new EventQueue();

        CardManager.Instance.CardToOblivion(destroyQueue, this);
        objective.DestroyJunk(destroyQueue);

        destroyQueue.StartQueue();

        while (!destroyQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }


        if(GameManager.Instance.currentState == GameState.GAME)
        {
            //Choose next card if not last
            if (PlotsManager.Instance.currentMainPlotScheme.currentStep < PlotsManager.Instance.currentMainPlotScheme.schemeSteps.Count)
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
                StoryManager.Instance.NextStoryArc();
            }

            currentQueue.UpdateQueue();
        }
        else
        {
            GameManager.Instance.tutorialManager.EndTutorial();
        }

    }

    public void CompletePlot(EventQueue queue)
    {
        queue.events.Add(CompletePlotRoutine(queue));
    }
    IEnumerator CompletePlotRoutine(EventQueue currentQueue)
    {
        if(GameManager.Instance.currentState == GameState.GAME)
        {
            //Reward
            EventQueue rewardQueue = new EventQueue();

            if (isMainPlot)
            {
                if (isFinal)
                {
                    //Final Step of main plot reward
                    RewardManager.Instance.ChooseMainPlotRewardFinal(rewardQueue, this);
                }
                else
                {
                    //Main Plot reward
                    RewardManager.Instance.ChooseMainPlotReward(rewardQueue, this);
                }
            }
            else
            {
                //Secondary plot reward
            }
            rewardQueue.StartQueue();
            while (!rewardQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
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

    public void FailPlot(EventQueue queue, GameOverType gameOverType)
    {
        queue.events.Add(FailPlotRoutine(queue, gameOverType));
    }
    public IEnumerator FailPlotRoutine(EventQueue currentQueue, GameOverType gameOverType)
    {
        CardManager.Instance.activatedCard = this;

        //Show the card + why the player lost TODO


        //GameOver
        EventQueue gameOverQueue = new EventQueue();

        GameManager.Instance.GameOver(gameOverQueue, gameOverType);

        gameOverQueue.StartQueue();
        while(!gameOverQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
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

        currentContainer.UpdatePlotInfo(this);
        if(completionTimer == 1)
        {
            currentContainer.UpdateTimerTweening(this, true);
        }
        else
        {
            if (currentContainer.audioSource == null) currentContainer.audioSource = SoundManager.Instance.GenerateAudioSource(currentContainer.gameObject);
            Sound intervert = new Sound(currentContainer.audioSource, "SFX_CLOCKTICK01", SoundType.SFX, false, false);
            intervert.Play();

            CardManager.Instance.cardTweening.ScaleBounce(currentContainer.visuals.cardTimerFrame.gameObject, 1.5f);
        }
        yield return new WaitForSeconds(0.4f);
        if(completionTimer <= 0)
        {
            EventQueue failQueue = new EventQueue();
            FailPlot(failQueue, GameOverType.PLOT_TIMER);

            failQueue.StartQueue();
            while(!failQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        currentQueue.UpdateQueue();
    }
}
