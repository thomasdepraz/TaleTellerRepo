using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterBehaviour //TO MOVE
{
    None,
    Peaceful, 
    Agressive
}
public enum CardEventTrigger//DEPRECATED
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
}//DEPRECATED

public enum CardRarity
{
    Common, 
    Rare, 
    Epic, 
    Legendary
}
public enum Archetype
{
    None, 
    Trading, 


}

[CreateAssetMenu(fileName = "Card", menuName = "Data/Card", order = 0)]
public class CardData : ScriptableObject
{
    public CardData dataReference;
    public string cardName;
    public CardRarity rarity;
    public Archetype archetype;

    //DEPRECATED
    public bool isKeyCard;
    [HideInInspector] public bool keyCardActivated;
    public int interestCooldown;
    [HideInInspector] public int currentInterestCooldown;
    public int creativityBurn;
    public CardType type;
    public CharacterBehaviour characterBehaviour;
    public CharacterStats characterStats;
    //---

    public CardEventTrigger trigger;
    public int creativityCost;//CHANGE NAME
    public CardEffect effect;//DEPR
    public CardEffect deadCardEffect;//DEPRE
    public Sprite cardGraph;
    [HideInInspector] public CardFeedback feedback;//DEPR
    public CardContainer currentContainer;

    [TextArea(2, 3)]
    public string description;

    //TEMP
    [Expandable]
    public List<Effect> effectsReferences = new List<Effect>();
    public List<Effect> effects = new List<Effect>();

    [Expandable]
    public CardTypes cardTypeReference;
    public CardTypes cardType;


    //Events
    public delegate void BoardEvent(EventQueue queue);
    public BoardEvent onStoryStart;
    public BoardEvent onStoryEnd;
    public BoardEvent onTurnEnd;
    public BoardEvent onTurnStart;

    public delegate void CardEvent(EventQueue queue);
    public CardEvent onCardEnter;
    public CardEvent onCardDraw;
    public CardEvent onCardDiscard;
    public CardEvent onCardAppear;


    //This is how a base card will be initialized (It's meant to be overwritten)
    public virtual CardData InitializeData(CardData _data)
    {
       CardData data = Instantiate(_data.dataReference);//make data an instance of itself
       data.dataReference = _data.dataReference;
        //Write logic to determine how the card subscribe to the events
        if(_data.cardTypeReference!= null)
        {
            CardTypes type = Instantiate(_data.cardTypeReference);
            data.cardType = type;
            data.cardTypeReference = _data.cardTypeReference;

            data.cardType.InitType(data);//<--Watch out, subscribing to events can happen in here
            data.onStoryEnd += data.cardType.OnEnd;
        }
        else //All the events that i subscribe in here must be the one that are overidden if I have a certain cardType
        {
            InitializeCardEffects(data);
            data.onStoryEnd += OnEnd;
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
        data.effects.Clear();
        //InitEffects
        for (int i = 0; i < data.effectsReferences.Count; i++)
        {
            if (effectsReferences[i] != null)
            {
                Effect effect = Instantiate(data.dataReference.effectsReferences[i]);
                data.effects.Add(effect);
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

        cardToReset = InitializeData(cardToReset);
    }

    public void UnsubscribeEvents()
    {
        if (onStoryStart != null)
        {
            foreach (var myDelegate in onStoryStart.GetInvocationList())
            {
                onStoryStart -= myDelegate as BoardEvent;
            }
        }
        if (onStoryEnd != null)
        {
            foreach (var myDelegate in onStoryEnd.GetInvocationList())
            {
                onStoryEnd -= myDelegate as BoardEvent;
            }
        }
        if (onTurnEnd != null)
        {
            foreach (var myDelegate in onTurnEnd.GetInvocationList())
            {
                onTurnEnd -= myDelegate as BoardEvent;
            }
        }
        if (onTurnStart != null)
        {
            foreach (var myDelegate in onTurnStart.GetInvocationList())
            {
                onTurnStart -= myDelegate as BoardEvent;
            }
        }

        if (onCardEnter != null)
        {
            foreach (var myDelegate in onCardEnter.GetInvocationList())
            {
                onCardEnter -= myDelegate as CardEvent;
            }
        }
        if (onCardAppear != null)
        {
            foreach (var myDelegate in onCardAppear.GetInvocationList())
            {
                onCardAppear -= myDelegate as CardEvent;
            }
        }
        if (onCardDraw != null)
        {
            foreach (var myDelegate in onCardDraw.GetInvocationList())
            {
                onCardDraw -= myDelegate as CardEvent;
            }
        }
        if (onCardDiscard != null)
        {
            foreach (var myDelegate in onCardDiscard.GetInvocationList())
            {
                onCardDiscard -= myDelegate as CardEvent;
            }
        }
    }

}