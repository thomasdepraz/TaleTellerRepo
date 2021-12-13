using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceObj : BoardDrivenObj
{
    private enum RestrctionType { TypeAmount }

    [Serializable]
    struct TypeAmountRestrictionInfos
    {
        internal enum CardType { Character, Location, Object }
        internal enum CharacterType { Peacefull, Agressive, Both }
        internal enum RestrictionMethod { Maximum, Minimum, Exact }
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

    public override void SubscribeUpdateStatus(PlotCard data)
    {
        data.onCardEnter += UpdateStatus;
        data.onStoryStart += UpdateStatus;
    }

    [SerializeField]
    bool plotOnBoard;
    [SerializeField]
    private RestrctionType restriction;
    [Header("Type Amount Restriction")]
    [SerializeField]
    List<TypeAmountRestrictionInfos> typeRestrictions = new List<TypeAmountRestrictionInfos>();

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

            default:
                break;
        }

        if (plotOnBoard)
        {
            if (!(CardManager.Instance.board.IsCardOnBoard(linkedPlotData)))
                complete = false;
        }

        return complete;
    }
}




