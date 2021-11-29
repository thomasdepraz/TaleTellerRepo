using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Trigger
{
    None,
    OnEnter,
    OnStoryStart,
    OnStoryEnd,
    OnCardDiscard,
    OnCardDrawn,
    OnEndTurn
}
public enum EffectTarget
{
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
    Addition,
    Substraction,
    Product,
    Division
}

[System.Serializable]
public class EffectValue
{
    public float value;
    public EffectValueType type;
    public EffectValueOperator op;
}
public class Effect : ScriptableObject
{
    [HideInInspector] public CardData linkedData;
    public string effectName;
    public Trigger trigger;
    public EffectTarget target;
    public BoardRange[] range;
    [HideInInspector] public List<EffectValue> values = new List<EffectValue>();
    //All of these methods needs to be overwritten
    public virtual void InitEffect(CardData card) // <-- Extend this base each time you want to support a new event
    {
        linkedData = card;
        //switch case that subscribes OnTriggerEffect() to the right delegate based on the effect trigger
        switch (trigger)
        {
            case Trigger.None:
                break;

            case Trigger.OnEnter:
                card.onEnterEvent += OnTriggerEffect;
                break;

            case Trigger.OnStoryStart:
                card.onStartEvent += OnTriggerEffect;
                break;

            case Trigger.OnStoryEnd:
                card.onEndEvent += OnTriggerEffect;
                break;

            case Trigger.OnCardDiscard:
                break;

            case Trigger.OnCardDrawn:
                break;

            case Trigger.OnEndTurn:
                break;

            default:
                break;
        }
    }

    public virtual void OnTriggerEffect()
    {
        //ajouter la coroutine à la queue
        CardManager.Instance.board.currentQueue.Add(EffectLogic());
    }

    public virtual IEnumerator EffectLogic()
    {
        //actual effect logic
        Debug.Log("EffectLogicBase");
        yield return null;
        //update queue
        CardManager.Instance.board.UpdateQueue();
    }

    public virtual void ResetEffect()
    {
        //unsubscribe from the delegate <-- actually its already done in the resetcardData method
        //subscribedEvent -= OnTriggerEffect;
    }

    public virtual List<CardData> GetTargets()
    {
        List<CardData> targets = new List<CardData>();

        switch (target)
        {
            case EffectTarget.Board:
                //Process with range
                #region BoardRange
                DraftBoard board = CardManager.Instance.board;
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
}



