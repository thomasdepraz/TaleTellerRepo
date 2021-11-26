using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatsBonusEffect", menuName = "Card Effect/StatsBonusEffect")]
public class StatsBonusEffect : CardEffect
{
    public enum AffectedStats
    {
        Creativity, 
        Health, 
        Attack, 
        BonusAttack, 
    }

    [System.Serializable]
    public struct Effect
    {
        [SerializeField] public AffectedStats affectedStat;
        [SerializeField] public int value;
    }


    [Header("Stats")]
    public List<Effect> affectedStatistics = new List<Effect>();

    public override void TriggerEffect(CardData target = null)
    {
        for (int i = 0; i < affectedStatistics.Count; i++)
        {
            switch (affectedStatistics[i].affectedStat)
            {
                case AffectedStats.Creativity:
                    GameManager.Instance.creativityManager.creativity += affectedStatistics[i].value;
                    break;
                case AffectedStats.Health:
                    if (target != null)
                    {
                        target.characterStats.baseLifePoints += affectedStatistics[i].value;
                        target.feedback.UpdateText(target);
                    }
                    else
                        GameManager.Instance.currentHero.lifePoints += affectedStatistics[i].value;
                    break;
                case AffectedStats.Attack:
                    if (target != null)
                    {
                        target.characterStats.baseAttackDamage += affectedStatistics[i].value;
                        target.feedback.UpdateText(target);
                    }
                    else
                        GameManager.Instance.currentHero.attackDamage += affectedStatistics[i].value;
                    break;
                case AffectedStats.BonusAttack:
                    if (target != null)
                    {
                        target.characterStats.baseAttackDamage += affectedStatistics[i].value;////FOR NOW
                        target.feedback.UpdateText(target); ////FOR NOW
                    }
                    else
                        GameManager.Instance.currentHero.bonusDamage += affectedStatistics[i].value;
                    break;
                default:
                    break;
            }
        }
    }

}
