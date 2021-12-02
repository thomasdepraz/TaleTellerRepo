using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterBehaviour
{
    None,
    Peaceful, 
    Agressive
}
public enum CardEventTrigger
{
    None, 
    OnEncounter,
    OnDeath, 
    OnAppear
}

public enum CardType
{
    Character, 
    Object, 
    Location
}
[CreateAssetMenu(fileName = "Card", menuName = "Data/Card", order = 0)]
public class CardData : ScriptableObject
{
    public CardData dataReference;
    public string cardName;
    public bool isKeyCard;
    [HideInInspector] public bool keyCardActivated;
    public int interestCooldown;
    [HideInInspector] public int currentInterestCooldown;
    public int creativityBurn;
    public CardType type;
    public CharacterBehaviour characterBehaviour;
    public CharacterStats characterStats;

    public CardEventTrigger trigger;
    public int creativityCost;
    public CardEffect effect;
    public CardEffect deadCardEffect;
    public Sprite cardGraph;
    [HideInInspector] public CardFeedback feedback;
     public CardContainer currentContainer;

    [TextArea(2, 3)]
    public string description;

    //TEMP
    [Expandable]
    public List<Effect> effects = new List<Effect>();

    [Expandable]
    public CardTypes cardTypeReference;
    [HideInInspector]public CardTypes cardType;


    //Events specification
    public delegate void BoardEvent(EventQueue queue);
    public BoardEvent onStartEvent;
    public BoardEvent onEndEvent;
    public BoardEvent onEnterEvent;

    //This is how a base card will be initialized (It's meant to be overwritten)
    public virtual CardData InitializeData(CardData data)
    {
        data = Instantiate(dataReference);//make data an instance of itself

        //Write logic to determine how the card subscribe to the events
        if(data.cardTypeReference!= null)
        {
            data.cardType = Instantiate(data.cardTypeReference);
            data.cardType.InitType(data);//<--Watch out, subscribing to events can happen in here

            data.onEndEvent += data.cardType.OnEnd;
        }
        else //All the events that i subscribe in here must be the one that are overidden if I have a certain cardType
        {
            InitializeCardEffects(data);
            data.onEndEvent += OnEnd;
        }

        return data;
    }

    public void OnEnd(EventQueue queue)
    {
        queue.events.Add(DiscardOnEndRoutine(queue));
    }
    public IEnumerator DiscardOnEndRoutine(EventQueue currentQueue)
    {
        EventQueue discardQueue = new EventQueue();

        CardManager.Instance.board.DiscardCardFromBoard(currentContainer, discardQueue);

        discardQueue.StartQueue();

        while (!discardQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }

    public void ResetCharacterStats()
    {
        characterStats.Reset();
    }

    public void InitializeCardEffects(CardData data)
    {
        //InitEffects
        for (int i = 0; i < data.effects.Count; i++)
        {
            if (effects[i] != null)
            {
                data.effects[i] = Instantiate(dataReference.effects[i]);
                data.effects[i].InitEffect(data); //<--This handles the subscription for all effects
            }
        }
    }

    public virtual void ResetData(CardData cardToReset)
    {
        //Unsubscribe from all events <-- Extend these methods each time we add a new delegate
        #region Unsubscribe methods
        UnsubscribeEvents();

        #endregion

        //Reset data----------------------------------

        cardToReset = Instantiate(cardToReset.dataReference);//make data an instance of itself

        //Instantiate other scriptables objects
        if (cardToReset.cardTypeReference != null)
        {
            cardToReset.cardType = Instantiate(cardToReset.cardTypeReference);
            cardToReset.cardType.InitType(cardToReset);//<--Watch out, subscribing to events can happen in here
        }

        for (int i = 0; i < cardToReset.effects.Count; i++)
        {
            if (effects[i] != null) cardToReset.effects[i] = Instantiate(cardToReset.dataReference.effects[i]);
        }

        //Write logic to determine how the card subscribe to the events
        if (cardToReset.dataReference.cardType == null)//All the events that i subscribe in here must be the one that are overidden if I have a certain cardType
        {
            InitializeCardEffects(cardToReset);
        }
        //-----------------------
    }

    public void UnsubscribeEvents()
    {
        if (onStartEvent != null)
        {
            foreach (var myDelegate in onStartEvent.GetInvocationList())
            {
                onStartEvent -= myDelegate as BoardEvent;
            }
        }
        if (onEnterEvent != null)
        {
            foreach (var myDelegate in onEnterEvent.GetInvocationList())
            {
                onEnterEvent -= myDelegate as BoardEvent;
            }
        }
        if (onEndEvent != null)
        {
            foreach (var myDelegate in onEndEvent.GetInvocationList())
            {
                onEndEvent -= myDelegate as BoardEvent;
            }
        }
    }

}