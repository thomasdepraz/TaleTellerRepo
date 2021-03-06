using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UITooltipTarget
{
    HEALTH_STAT, 
    ATTACK_STAT, 
    GOLD_STAT,
    DECK, 
    DISCARD,
    INK,
    GOBUTTON,
    BOARDSLOT, 
    HERO,
    INSPIRE,
    CARD_TYPE,
    CARD_INK,
    CARD_TIMER,
    CARD_ATTACK,
    CARD_HP,
    CARD_RANGE_RIGHT,
    CARD_RANGE_LEFT,
    CARD_RANGE_ALLRIGHT,
    CARD_RANGE_ALLLEFT,
    CARD_RANGE_HERO,
    CARD_RANGE_RIGHTANDLEFT,
    CARD_RANGE_ALL,
    CARD_RANGE_PLOT,
    REWARD
}
public class UITooltip : MonoBehaviour
{
    public UITooltipTarget tooltipTarget;
    public float delay;
    bool open;
    public void Update()
    {
        if(open)
        {
            UpdateTooltipDescription();
        }
    }

    public void ShowTooltip()
    {
        string description = GetTooltipDescription();
        GameManager.Instance.pointer.ShowTooltip(description, delay);
        open = true;
    }

    public void HideTooltip()
    {
        //closetooltip
        GameManager.Instance.pointer.HideTooltip();
        open = false;
    }

    public virtual string GetTooltipDescription()
    {
        string result  = string.Empty;

        switch (tooltipTarget)
        {
            case UITooltipTarget.HEALTH_STAT:
                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$HEALTH_STAT");
                result = string.Format(result, GameManager.Instance.currentHero.lifePoints);
                break;
            case UITooltipTarget.ATTACK_STAT:
                int baseAttack = GameManager.Instance.currentHero.attackDamage;
                int bonusAttack = GameManager.Instance.currentHero.bonusDamage;
                int totalAttack = baseAttack + bonusAttack;
                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$ATTACK_STAT");
                result = string.Format(result,totalAttack, baseAttack, bonusAttack);
                break;
            case UITooltipTarget.GOLD_STAT:
                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$GOLD_STAT");
                result = string.Format(result, GameManager.Instance.currentHero.goldPoints);
                break;
            case UITooltipTarget.DECK:
                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$DECK");
                result = string.Format(result, CardManager.Instance.cardDeck.cardDeck.Count);
                break;
            case UITooltipTarget.DISCARD:
                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$DISCARD");
                result = string.Format(result, CardManager.Instance.cardDeck.discardPile.Count);
                break;
            case UITooltipTarget.INK:
                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$INK");
                result = string.Format(result, CardManager.Instance.cardDeck.discardPile.Count);
                break;
            case UITooltipTarget.GOBUTTON:
                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$GOBUTTON");
                result = string.Format(result, CardManager.Instance.cardDeck.discardPile.Count);
                break;
            case UITooltipTarget.BOARDSLOT:
                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$BOARDSLOT");
                break;
            case UITooltipTarget.HERO:
                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$HERO");
                break;
            case UITooltipTarget.INSPIRE:
                var inspire = CardManager.Instance.inspire;
                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$INSPIRE");
                result = string.Format(result, inspire.drawCardsCount, inspire.darkIdeasCount, inspire.useCount);
                break;
            default:
                break;
        }

        return result;
    }

    public void UpdateTooltipDescription()
    {
        string description = GetTooltipDescription();
        GameManager.Instance.pointer.tooltipText.SetText(description);
    }
}
