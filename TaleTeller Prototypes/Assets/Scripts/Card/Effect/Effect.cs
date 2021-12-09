using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum Trigger
{
    None,
    OnCardEnter,
    OnStoryStart,
    OnStoryEnd,
    OnCardDiscard,
    OnCardDrawn,
    OnTurnEnd,
    OnTurnStart
}
public enum EffectTarget
{
    None,
    Board,
    Hand,
    Deck,
    Discard
}
public enum BoardRange
{
    None,
    Self,
    All,
    AllLeft,
    AllRight,
    FirstLeft,
    SecondLeft,
    ThirdLeft,
    FourthLeft,
    FirstRight,
    SecondRight,
    ThirdRight,
    FourthRight
}
public enum EffectValueType
{
    Life,
    Attack,
    Gold, 
    Card
}
public enum EffectValueOperator
{
    None,
    Addition,
    Substraction,
    Product,
    Division
}

[System.Serializable]
public class EffectValue
{
    public int value;
    public EffectValueType type;
    public EffectValueOperator op;
}
public class Effect : ScriptableObject
{
    [HideInInspector] public CardData linkedData;
    public string description;
    public Trigger trigger;
    public EffectTarget target;

    [ShowIf("ShowRange")]
    public BoardRange[] range;

    [HideInInspector] public List<EffectValue> values = new List<EffectValue>();

    #region NaughtyAttributesMethods
    public bool ShowRange()
    {
        if (target == EffectTarget.Board) return true;
        else return false;
    }
    #endregion

    //All of these methods needs to be overwritten
    public virtual void InitEffect(CardData card) // <-- Extend this base each time you want to support a new event
    {
        linkedData = card;
        //switch case that subscribes OnTriggerEffect() to the right delegate based on the effect trigger
        switch (trigger)
        {
            case Trigger.None:
                break;

            case Trigger.OnCardEnter:
                card.onCardEnter += OnTriggerEffect;
                break;

            case Trigger.OnStoryStart:
                card.onStoryStart += OnTriggerEffect;
                break;

            case Trigger.OnStoryEnd:
                card.onStoryEnd += OnTriggerEffect;
                break;

            case Trigger.OnCardDiscard:
                card.onCardDiscard += OnTriggerEffect;
                break;

            case Trigger.OnCardDrawn:
                card.onCardDraw += OnTriggerEffect;
                break;

            case Trigger.OnTurnStart:
                card.onTurnStart += OnTriggerEffect;
                break;

            case Trigger.OnTurnEnd:
                card.onTurnEnd += OnTriggerEffect;
                break;

            default:
                break;
        }
    }

    public virtual void OnTriggerEffect(EventQueue queue) //TODO REMOVE VIRTUAL
    {
        //ajouter la coroutine à la queue
        queue.events.Add(EffectLogic(queue));
    }

    public virtual IEnumerator EffectLogic(EventQueue currentQueue)
    {
        //actual effect logic
        Debug.Log("EffectLogicBase");
        yield return null;
        //update queue
        currentQueue.UpdateQueue();
    }

    public virtual List<CardData> GetTargets()
    {
        List<CardData> targets = new List<CardData>();

        switch (target)
        {
            case EffectTarget.Board:
                //Process with range
                #region BoardRange
                Board board = CardManager.Instance.board;
                int currentBoardIndex = linkedData.currentContainer.currentSlot.slotIndex; 
                for (int index = 0; index < range.Length; index++)
                {
                    switch (range[index])
                    {
                        case BoardRange.None:
                            break;
                        case BoardRange.Self:
                            targets.Add(linkedData);
                            break;
                        case BoardRange.All:
                            for (int i = 0; i < board.slots.Count; i++)
                            {
                                if (board.slots[i].currentPlacedCard != null)
                                    targets.Add(board.slots[i].currentPlacedCard.data);
                            }
                            break;
                        case BoardRange.AllLeft:
                            for (int i = 0; i < currentBoardIndex; i ++)
                            {
                                if (board.slots[i].currentPlacedCard != null)
                                    targets.Add(board.slots[i].currentPlacedCard.data);
                            }
                            break;
                        case BoardRange.AllRight:
                            for (int i = currentBoardIndex + 1; i < board.slots.Count; i++)
                            {
                                if (board.slots[i].currentPlacedCard != null)
                                    targets.Add(board.slots[i].currentPlacedCard.data);
                            }
                            break;
                        case BoardRange.FirstLeft:
                            if(currentBoardIndex - 1 >= 0 && board.slots[currentBoardIndex-1].currentPlacedCard != null)
                            {
                                targets.Add(board.slots[currentBoardIndex - 1].currentPlacedCard.data);
                            }
                            break;
                        case BoardRange.SecondLeft:
                            if (currentBoardIndex - 2 >= 0 && board.slots[currentBoardIndex - 2].currentPlacedCard != null)
                            {
                                targets.Add(board.slots[currentBoardIndex - 2].currentPlacedCard.data);
                            }
                            break;
                        case BoardRange.ThirdLeft:
                            if (currentBoardIndex - 3 >= 0 && board.slots[currentBoardIndex - 3].currentPlacedCard != null)
                            {
                                targets.Add(board.slots[currentBoardIndex - 3].currentPlacedCard.data);
                            }
                            break;
                        case BoardRange.FourthLeft:
                            if (currentBoardIndex - 1 >= 4 && board.slots[currentBoardIndex - 4].currentPlacedCard != null)
                            {
                                targets.Add(board.slots[currentBoardIndex - 4].currentPlacedCard.data);
                            }
                            break;
                        case BoardRange.FirstRight:
                            if (currentBoardIndex + 1 < board.slots.Count && board.slots[currentBoardIndex + 1].currentPlacedCard != null)
                            {
                                targets.Add(board.slots[currentBoardIndex + 1].currentPlacedCard.data);
                            }
                            break;
                        case BoardRange.SecondRight:
                            if (currentBoardIndex + 2 < board.slots.Count && board.slots[currentBoardIndex + 2].currentPlacedCard != null)
                            {
                                targets.Add(board.slots[currentBoardIndex + 2].currentPlacedCard.data);
                            }
                            break;
                        case BoardRange.ThirdRight:
                            if (currentBoardIndex + 3 < board.slots.Count && board.slots[currentBoardIndex + 3].currentPlacedCard != null)
                            {
                                targets.Add(board.slots[currentBoardIndex + 3].currentPlacedCard.data);
                            }
                            break;
                        case BoardRange.FourthRight:
                            if (currentBoardIndex + 4 < board.slots.Count && board.slots[currentBoardIndex + 4].currentPlacedCard != null)
                            {
                                targets.Add(board.slots[currentBoardIndex + 4].currentPlacedCard.data);
                            }
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                break;

            case EffectTarget.Hand:
                //return list of cards in hand
                #region Hand
                for (int i = 0; i < CardManager.Instance.cardHand.currentHand.Count; i++)
                {
                    targets.Add(CardManager.Instance.cardHand.currentHand[i].data);
                }
                #endregion
                break;

            case EffectTarget.Deck:
                //return list of cards in deck
                #region Deck
                for (int i = 0; i < CardManager.Instance.cardDeck.cardDeck.Count; i++)
                {
                    targets.Add(CardManager.Instance.cardDeck.cardDeck[i]);
                }
                #endregion
                break;

            case EffectTarget.Discard:
                //return list of cards in discard
                #region Discard
                for (int i = 0; i < CardManager.Instance.cardDeck.discardPile.Count; i++)
                {
                    targets.Add(CardManager.Instance.cardDeck.discardPile[i]);
                }
                #endregion
                break;

            default:
                break;
        }

        //if range is null maybe look at other parameter
        return targets;
    }

    public virtual string GetDescription(Effect effect)
    {
        string result = string.Empty;
        string[] temp = effect.description.Split('$');

        for (int i = 0; i < temp.Length; i++)
        {
            string value = string.Empty; 
            if(i%2 == 0)
            {
                value = temp[i];
            }
            else
            {
                Type type = effect.GetType();
                FieldInfo prop = type.GetField(temp[i]);
                EffectValue val = prop.GetValue(effect) as EffectValue;

                value = val.value.ToString();
            }
            result += value;
        }
        
        return result;
    }

}



