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
    CARD_TYPE,
    CARD_INK,
    CARD_TIMER,
    CARD_ATTACK,
    CARD_HP
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
                result = LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$ATTACK_STAT");
                result = string.Format(result, GameManager.Instance.currentHero.attackDamage);
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
