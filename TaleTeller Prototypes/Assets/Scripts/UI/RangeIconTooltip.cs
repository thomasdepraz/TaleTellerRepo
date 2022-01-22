using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIconTooltip : UITooltip
{
    public EffectDescriptionBlock linkedBlock;

    public override string GetTooltipDescription()
    {
        switch (linkedBlock.rangeType)
        {
            case EffectRangeType.All:
                return(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$EFFECT_RANGE_ALL"));

            case EffectRangeType.AllLeft:
                return (LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$EFFECT_RANGE_ALL-LEFT"));

            case EffectRangeType.AllRight:
                return (LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$EFFECT_RANGE_ALL-RIGHT"));

            case EffectRangeType.Hero:
                return (LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$EFFECT_RANGE_HERO"));

            case EffectRangeType.Left:
                return (LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$EFFECT_RANGE_LEFT"));

            case EffectRangeType.Plot:
                return (LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$EFFECT_RANGE_PLOT"));

            case EffectRangeType.Right:
                return (LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$EFFECT_RANGE_RIGHT"));

            case EffectRangeType.RightAndLeft:
                return (LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$EFFECT_RANGE_RIGHT-LEFT"));

            case EffectRangeType.Self:
                return (LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, "$EFFECT_RANGE_SELF"));

            default:
                return (null);
        }
    }
}
