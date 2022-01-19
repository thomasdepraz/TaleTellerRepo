using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalCardTooltip : UITooltip
{
    public CardContainer container;
    internal enum CardNature { DarkIdea, Plot, Junk, Normal };
    internal enum CardType {PeacefullCharacter, EnemyCharacter, Location, Object};
    CardNature dataNature => GetDataNature();
    CardType dataType => GetDataType();

    private CardType GetDataType()
    {
        var cardType = container.data.cardType.GetType();

        if (cardType == typeof(ObjectType))
            return CardType.Object;
        else if (cardType == typeof(LocationType))
            return CardType.Location;
        else if (cardType == typeof(CharacterType))
        {
            var characterData = container.data.cardType as CharacterType;
            if (characterData.behaviour == CharacterBehaviour.Agressive)
                return CardType.EnemyCharacter;
            else
                return CardType.PeacefullCharacter;
        }
        else return CardType.Object;
    }

    private CardNature GetDataNature()
    {
        var dataType = container.data.GetType();

        if (dataType == typeof(DarkIdeaCard))
            return CardNature.DarkIdea;
        else if (dataType == typeof(PlotCard))
            return CardNature.Plot;
        else if (dataType == typeof(JunkCard))
            return CardNature.Junk;
        else 
            return CardNature.Normal;
    }

    public override string GetTooltipDescription()
    {
        string result = string.Empty;
        var cardType = container.data.cardType;

        switch (tooltipTarget)
        {
            case UITooltipTarget.CARD_TYPE:

                switch (dataType)
                {
                    case CardType.PeacefullCharacter:
                        result = "This Card is an Ally.\nIt will attack nerby ennemies.\nIt comes back in your hand while it has stacks.";
                        break;
                    case CardType.EnemyCharacter:
                        result = "This Card is an Enemy.\nIt will attack you and your allies nerby.\nIt comes back to your hands while it has stacks.";
                        break;
                    case CardType.Location:
                        result = "This Card is a Location.\nThey trigger most of they effect right when the turn starts.";
                        break;
                    case CardType.Object:
                        result = "This Card is an Object.\nHis effects apply when your hero walks on it.\nIt will be destroyed after usage.";
                        break;
                }

                switch (dataNature)
                {
                    case CardNature.Junk:
                        result += "\nIt is also a Junk, it disapears when the plot it is linked to gets resolved.";
                        break;
                    case CardNature.Plot:
                        result += "\nIt is also a Plot, you need to achieve his objective before timer reach 0!.";
                        break;
                    default:
                        break;
                }

                break;

            case UITooltipTarget.CARD_TIMER:

                switch (dataNature)
                {
                    case CardNature.Plot:
                        result = "Timer of the Plot.\nIf it reaches 0, you loose.";
                        break;
                    default:
                        result = "Character stacks.\nWhen it reaches 0, character stops coming back to your hand.";
                        break;
                }

                break;

            case UITooltipTarget.CARD_INK:

                result = "Amount of Ink requiered to place this card on the track.";

                break;

            case UITooltipTarget.CARD_ATTACK:

                result = "Amount of damages dealt by this character when he hits.";

                break;

            case UITooltipTarget.CARD_HP:

                result = "Amount of life point of this character.\nHe is destroyed if it reaches 0";

                break;
        }

        return result;
    }
}
