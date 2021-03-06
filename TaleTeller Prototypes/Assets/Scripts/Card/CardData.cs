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

public enum CardRarity
{
    None = 0,
    Common = 1, 
    Uncommon = 2,
    Rare = 3, 
    Epic = 4, 
    Legendary = 5
}
public enum Archetype
{
    None, 
    Base,
    Trading, 
    Vision,
    Berserk,

    NumberOfArchetypes//Always last Archetype
}

public class CardData : ScriptableObject
{
    public CardData dataReference;
    public string cardName;
    public CardRarity rarity;
    public Archetype archetype;

    public int manaCost;
    public Sprite cardGraph;
    public CardContainer currentContainer;

    [TextArea(2, 3)]
    public string description;

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
    public BoardEvent illumination;
    public BoardEvent overload;

    public delegate void CardEvent(EventQueue queue, CardData data);
    public CardEvent onCardEnter;
    public CardEvent onCardDraw;
    public CardEvent onCardDiscard;
    public CardEvent onCardAppear;

    public CardEvent onCharDeath;
    public CardEvent onCharFight;
    public CardEvent onCharHit;


    //This is how a base card will be initialized (It's meant to be overwritten)
    public virtual CardData InitializeData(CardData _data)
    {
        CardData data = Instantiate(_data.dataReference);//make data an instance of itself
        data.dataReference = _data.dataReference;

        data.currentContainer = null;
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

        CardManager.Instance.CardBoardToDiscard(currentContainer, discardQueue);
        discardQueue.StartQueue();

        while (!discardQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
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

    public virtual CardData ResetData(CardData cardToReset)
    {
        //Unsubscribe from all events <-- Extend these methods each time we add a new delegate
        #region Unsubscribe methods
        UnsubscribeEvents(cardToReset);

        #endregion

        //Reset data----------------------------------

        
        return InitializeData(cardToReset); 
    }

    public void UnsubscribeEvents(CardData cardToReset)
    {
        if (cardToReset.onStoryStart != null)
        {
            foreach (var myDelegate in cardToReset.onStoryStart.GetInvocationList())
            {
                cardToReset.onStoryStart -= myDelegate as BoardEvent;
            }
        }
        if (cardToReset.onStoryEnd != null)
        {
            foreach (var myDelegate in cardToReset.onStoryEnd.GetInvocationList())
            {
                cardToReset.onStoryEnd -= myDelegate as BoardEvent;
            }
        }
        if (cardToReset.onTurnEnd != null)
        {
            foreach (var myDelegate in cardToReset.onTurnEnd.GetInvocationList())
            {
                cardToReset.onTurnEnd -= myDelegate as BoardEvent;
            }
        }
        if (cardToReset.onTurnStart != null)
        {
            foreach (var myDelegate in cardToReset.onTurnStart.GetInvocationList())
            {
                cardToReset.onTurnStart -= myDelegate as BoardEvent;
            }
        }
        if (cardToReset.illumination != null)
        {
            foreach (var myDelegate in cardToReset.illumination.GetInvocationList())
            {
                cardToReset.illumination -= myDelegate as BoardEvent;
            }
        }
        if (cardToReset.overload != null)
        {
            foreach (var myDelegate in cardToReset.overload.GetInvocationList())
            {
                cardToReset.overload -= myDelegate as BoardEvent;
            }
        }


        if (cardToReset.onCardEnter != null)
        {
            foreach (var myDelegate in cardToReset.onCardEnter.GetInvocationList())
            {
                cardToReset.onCardEnter -= myDelegate as CardEvent;
            }
        }
        if (cardToReset.onCardAppear != null)
        {
            foreach (var myDelegate in cardToReset.onCardAppear.GetInvocationList())
            {
                cardToReset.onCardAppear -= myDelegate as CardEvent;
            }
        }
        if (cardToReset.onCardDraw != null)
        {
            foreach (var myDelegate in cardToReset.onCardDraw.GetInvocationList())
            {
                cardToReset.onCardDraw -= myDelegate as CardEvent;
            }
        }
        if (cardToReset.onCardDiscard != null)
        {
            foreach (var myDelegate in cardToReset.onCardDiscard.GetInvocationList())
            {
                cardToReset.onCardDiscard -= myDelegate as CardEvent;
            }
        }
        if (cardToReset.onCharDeath != null)
        {
            foreach (var myDelegate in cardToReset.onCharDeath.GetInvocationList())
            {
                cardToReset.onCharDeath -= myDelegate as CardEvent;
            }
        }
        if(cardToReset.onCharFight != null)
        {
            foreach (var myDelegate in cardToReset.onCharFight.GetInvocationList())
            {
                cardToReset.onCharFight -= myDelegate as CardEvent;
            }
        }
        if (cardToReset.onCharHit != null)
        {
            foreach (var myDelegate in cardToReset.onCharHit.GetInvocationList())
            {
                cardToReset.onCharHit -= myDelegate as CardEvent;
            }
        }
    }

}