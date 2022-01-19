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
                        result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$CARD_TYPE_ALLY");
                        break;
                    case CardType.EnemyCharacter:
                        result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$CARD_TYPE_ENEMY");
                        break;
                    case CardType.Location:
                        result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$CARD_TYPE_LOCATION");
                        break;
                    case CardType.Object:
                        result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$CARD_TYPE_OBJECT");
                        break;
                }

                switch (dataNature)
                {
                    case CardNature.Junk:
                        result += LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$CARD_NATURE_JUNK");
                        break;
                    case CardNature.Plot:
                        result += LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$CARD_NATURE_PLOT");
                        break;
                    default:
                        break;
                }

                break;

            case UITooltipTarget.CARD_TIMER:

                switch (dataNature)
                {
                    case CardNature.Plot:
                        result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$CARD_TIMER_PLOT");
                        break;
                    default:
                        result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$CARD_TIMER_CHARA");
                        break;
                }

                break;

            case UITooltipTarget.CARD_INK:

                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$CARD_INK");

                break;

            case UITooltipTarget.CARD_ATTACK:

                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$CARD_ATK");

                break;

            case UITooltipTarget.CARD_HP:

                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$CARD_HP");

                break;
        }

        return result;
    }
}
