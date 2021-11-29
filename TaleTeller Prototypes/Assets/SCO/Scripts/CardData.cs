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
    [HideInInspector] public CardContainer currentContainer;

    [TextArea(2, 3)]
    public string description;

    //TEMP
    [Expandable]
    public List<Effect> effects = new List<Effect>();

    [Expandable]
    public CardTypes cardTypeReference;
    [HideInInspector]public CardTypes cardType;


    //Events specification
    public delegate void BoardEvent();
    public BoardEvent onStartEvent;
    public BoardEvent onEndEvent;
    public BoardEvent onEnterEvent;

    //This is how a base card will be initialized (It's meant to be overwritten)
    public virtual CardData InitializeData(CardData data)
    {
        data = Instantiate(dataReference);//make data an instance of itself

        //Instantiate other scriptables objects
        if(data.cardTypeReference!= null)
        {
            data.cardType = Instantiate(data.cardTypeReference);
            data.cardType.InitType(data);//<--Watch out, subscribing to events can happen in here
        }

        for (int i = 0; i < data.effects.Count; i++)
        {
            if(effects[i]!=null) data.effects[i] = Instantiate(dataReference.effects[i]);
        }


        //Write logic to determine how the card subscribe to the events
        if(data.cardTypeReference == null)//All the events that i subscribe in here must be the one that are overidden if I have a certain cardType
        {
            //Subscribe to onEnterEvent so it at least processes the events if any
            data.onEnterEvent += OnEnter;

            //Subscribe to OnEnd to Discard
            data.onEndEvent += OnEnd;

        }

        return data;
    }

    #region Generic Events

    #region OnEnter (Effect Trigger)
    public void OnEnter()
    {
        //add effects to board manager list
        for (int i = 0; i < effects.Count; i++)
        {
            //Init effect that adds a routine to the manager list

            CardManager.Instance.board.currentQueue.Add(tempEffect());//THIS IS TEMPORARY
        }
    }

    IEnumerator tempEffect()
    {
        Debug.Log("Trigger Effect");
        yield return null;
        CardManager.Instance.board.UpdateQueue();
    }
    #endregion

    #region OnEnd (Discard)
    public void OnEnd()
    {
        CardManager.Instance.board.currentQueue.Add(DiscardRoutine());
    }
    private IEnumerator DiscardRoutine()
    {
        bool routineEnded = false;
        CardManager.Instance.board.DiscardCardFromBoard(currentContainer, ref routineEnded);

        yield return new WaitForSeconds(0.5f);

        //Unqueue
        CardManager.Instance.board.UpdateQueue();
    }
    #endregion

    #endregion

    public void ResetCharacterStats()
    {
        characterStats.Reset();
    }

    public void ResetData(CardData cardToReset)
    {
        //Unsubscribe from all events <-- Extend these methods each time we add a new delegate
        #region Unsubscribe methods
        if (onStartEvent != null)
        {
            foreach(var myDelegate in onStartEvent.GetInvocationList())
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
            //Subscribe to onEnterEvent so it at least processes the events if any
            cardToReset.onEnterEvent += OnEnter;

            //Subscribe to OnEnd to Discard
            cardToReset.onEndEvent += OnEnd;
        }

        //-----------------------
    }

}