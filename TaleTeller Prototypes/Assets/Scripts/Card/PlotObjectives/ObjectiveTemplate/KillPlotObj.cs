using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KillPlotObj : PlotDrivenObj
{
    private enum RestrctionType { TypeAmount, JunkPos }

    [Serializable]
    struct TypeAmountRestrictionInfos
    {
        internal enum CardType { Character, Location, Object }
        internal enum CharacterType { Peacefull, Agressive, Both }
        internal enum RestrictionMethod { Maximum, Minimum}
        bool ShowCharacterType => typeToHave == CardType.Character; 

        [SerializeField]
        internal CardType typeToHave;
        [SerializeField, ShowIf("ShowCharacterType")]
        internal CharacterType characterType;
        [SerializeField]
        internal RestrictionMethod restrictionMethod;
        [SerializeField]
        internal int count;
    }

    [Serializable]
    struct JunkPosRestrictionInfos
    {
        internal enum PlotPos { Anywhere, NextToJunk }

        public PlotPos junkPossiblePos;

        public CardData junkToHaveOnBoard;

        [SerializeField]
        internal int count;
    }

    bool ShowTypeAmountRestrictionList => restriction == RestrctionType.TypeAmount;
    bool ShowJunkPosRestrictionList => restriction == RestrctionType.JunkPos;

    [Header("Restriction")]
    [Tooltip("Check this if you want to apply board restriction to the kill objective")]
    public bool boardRestriction;
    [ShowIf("boardRestriction"), SerializeField]
    private RestrctionType restriction;
    [Header("Type Amount Restriction")]
    [SerializeField, ShowIf("ShowTypeAmountRestrictionList")]
    List<TypeAmountRestrictionInfos> typeRestrictions = new List<TypeAmountRestrictionInfos>();

    [Header("Junk Pos Restriction")]
    [SerializeField, ShowIf("ShowJunkPosRestrictionList")]
    List<JunkPosRestrictionInfos> junkPosRestrictions = new List<JunkPosRestrictionInfos>();

    public override void SubscribeUpdateStatus(PlotCard data)
    {
        data.onCharDeath += UpdateStatus;
    }

    public override IEnumerator UpdateStatusRoutine(EventQueue currentQueue, CardData data)
    {
        if (TestCompletion())
        {
            EventQueue completeQueue = new EventQueue();

            linkedPlotData.CompletePlot(completeQueue);
            completeQueue.StartQueue();

            while (!completeQueue.resolved)
                yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }

    bool TestCompletion()
    {
        bool complete = true;

        if (boardRestriction)
        {
            switch (restriction)
            {
                case RestrctionType.TypeAmount:

                    var cardOnBoard = CardManager.Instance.board.slots
                        .Where(s => s.currentPlacedCard != null)
                        .Select(s => s.currentPlacedCard.data);

                    foreach (TypeAmountRestrictionInfos restriction in typeRestrictions)
                    {
                        switch (restriction.typeToHave)
                        {
                            case TypeAmountRestrictionInfos.CardType.Character:

                                var characterOnBoard = cardOnBoard.Where(d => d.cardType.GetType() == typeof(CharacterType));
                                var characterTypes = characterOnBoard.Select(d => d.cardType);

                                switch (restriction.characterType)
                                {
                                    case TypeAmountRestrictionInfos.CharacterType.Agressive:

                                        int agressiveCount = 0;

                                        foreach (CardTypes type in characterTypes)
                                        {
                                            var characType = type as CharacterType;
                                            if (characType.behaviour == CharacterBehaviour.Agressive)
                                                agressiveCount++;
                                        }

                                        if (restriction.restrictionMethod == TypeAmountRestrictionInfos.RestrictionMethod.Minimum
                                            && agressiveCount < restriction.count)
                                            return false;
                                        else if (restriction.restrictionMethod == TypeAmountRestrictionInfos.RestrictionMethod.Maximum
                                            && agressiveCount > restriction.count)
                                            return false;

                                            break;

                                    case TypeAmountRestrictionInfos.CharacterType.Peacefull:

                                        int peacefullCount = 0;

                                        foreach (CardTypes type in characterTypes)
                                        {
                                            var characType = type as CharacterType;
                                            if (characType.behaviour == CharacterBehaviour.Peaceful)
                                                peacefullCount++;
                                        }

                                        if (restriction.restrictionMethod == TypeAmountRestrictionInfos.RestrictionMethod.Minimum
                                            && peacefullCount < restriction.count)
                                            return false;
                                        else if (restriction.restrictionMethod == TypeAmountRestrictionInfos.RestrictionMethod.Maximum
                                            && peacefullCount > restriction.count)
                                            return false;

                                            break;

                                    case TypeAmountRestrictionInfos.CharacterType.Both:
                                        if (restriction.restrictionMethod == TypeAmountRestrictionInfos.RestrictionMethod.Minimum
                                            && characterOnBoard.Count() < restriction.count)
                                            return false;
                                        else if (restriction.restrictionMethod == TypeAmountRestrictionInfos.RestrictionMethod.Maximum
                                            && characterOnBoard.Count() > restriction.count)
                                            return false;

                                        break;

                                    default:
                                        break;
                                }

                                break;

                            case TypeAmountRestrictionInfos.CardType.Location:

                                var locationOnBard = cardOnBoard.Where(d => d.cardType.GetType() == typeof(LocationType));

                                if (restriction.restrictionMethod == TypeAmountRestrictionInfos.RestrictionMethod.Minimum
                                           && locationOnBard.Count() < restriction.count)
                                    return false;
                                else if (restriction.restrictionMethod == TypeAmountRestrictionInfos.RestrictionMethod.Maximum
                                    && locationOnBard.Count() > restriction.count)
                                    return false;

                                break;

                            case TypeAmountRestrictionInfos.CardType.Object:

                                var objectOnBard = cardOnBoard.Where(d => d.cardType.GetType() == typeof(ObjectType));

                                if (restriction.restrictionMethod == TypeAmountRestrictionInfos.RestrictionMethod.Minimum
                                           && objectOnBard.Count() < restriction.count)
                                    return false;
                                else if (restriction.restrictionMethod == TypeAmountRestrictionInfos.RestrictionMethod.Maximum
                                    && objectOnBard.Count() > restriction.count)
                                    return false;

                                break;

                            default:
                                break;
                        }
                    }
                    break;

                case RestrctionType.JunkPos:
                    foreach( JunkPosRestrictionInfos restriction in junkPosRestrictions)
                    {
                        int junksCount = 0;

                        switch (restriction.junkPossiblePos)
                        {
                            case JunkPosRestrictionInfos.PlotPos.NextToJunk:
                                if(CardManager.Instance.board.slots[linkedPlotData.currentContainer.currentSlot.slotIndex - 1].currentPlacedCard.data!= null)
                                    if (CardManager.Instance.board.slots[linkedPlotData.currentContainer.currentSlot.slotIndex - 1].currentPlacedCard.data.dataReference == restriction.junkToHaveOnBoard)
                                    {
                                        junksCount++;
                                    }
                                if (CardManager.Instance.board.slots[linkedPlotData.currentContainer.currentSlot.slotIndex + 1].currentPlacedCard.data != null)
                                    if (CardManager.Instance.board.slots[linkedPlotData.currentContainer.currentSlot.slotIndex + 1].currentPlacedCard.data.dataReference == restriction.junkToHaveOnBoard)
                                    {
                                        junksCount++;
                                    }
                                break;
                            case JunkPosRestrictionInfos.PlotPos.Anywhere:
                                for(int i = 0; i < CardManager.Instance.board.slots.Count; i++)
                                {
                                    if (CardManager.Instance.board.slots[i].currentPlacedCard.data != null)
                                        if (CardManager.Instance.board.slots[i].currentPlacedCard.data.dataReference == restriction.junkToHaveOnBoard)
                                        {
                                            junksCount++;
                                        }
                                }
                                break;
                        }

                        if(junksCount < restriction.count)
                        {
                            return false;
                        }
                    }
                   
                
                    break;

                default:
                    break;
            }
        }

        return complete;
    }
}
