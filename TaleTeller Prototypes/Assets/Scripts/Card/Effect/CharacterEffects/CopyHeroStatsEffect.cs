using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System;

public class CopyHeroStatsEffect : CharacterStatsEffect
{
    internal enum TargetBehaviour { Both, Peacefull, Agressive };
    internal enum Operation { Copy, Add, Substract };
    internal enum HeroStats { Hp, MaxHp, BaseAttack, TempAttack, Gold, MaxGold}

    [Serializable]
    internal struct HeroToCharStatsInfos
    {
        [SerializeField, Tooltip("Check this to modify this param through effect")]
        internal bool modifyThisStat;
        [SerializeField, ShowIf("modifyThisStat"), AllowNesting]
        internal Operation howToImpact;
        [SerializeField, ShowIf("modifyThisStat"), AllowNesting]
        internal HeroStats heroStatToTake;
    }

    [Header("Target Selection Params")]
    [SerializeField]
    internal TargetBehaviour behaviourTargeted;

    [Header("Copy stats param")]
    [SerializeField]
    internal HeroToCharStatsInfos changeAttackInto;
    [SerializeField]
    internal HeroToCharStatsInfos changeHealthInto;

    List<CharacterType> GetTargetsExtension()
    {
        var possibleTargets = GetTargets();
        var possibleCharacters = possibleTargets
            .Where(t => t.cardType.GetType() == typeof(CharacterType))
            .Select(t => (CharacterType)t.cardType);

        List<CharacterType> targets = new List<CharacterType>();

        switch (behaviourTargeted)
        {
            case TargetBehaviour.Agressive:
                targets.AddRange(possibleCharacters.Where(c => c.behaviour == CharacterBehaviour.Agressive));
                break;

            case TargetBehaviour.Peacefull:
                targets.AddRange(possibleCharacters.Where(c => c.behaviour == CharacterBehaviour.Peaceful));
                break;

            case TargetBehaviour.Both:
                targets = possibleCharacters.ToList();
                break;
        }

        return targets;
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        var targets = GetTargetsExtension();

        EventQueue feedbackQueue = new EventQueue();

        foreach (CharacterType chara in targets)
        {
            if (changeAttackInto.modifyThisStat)
                CharacStatsChanger(ref chara.stats.baseAttackDamage, changeAttackInto);
            if (changeHealthInto.modifyThisStat)
                CharacStatsChanger(ref chara.stats.baseLifePoints, changeHealthInto);

            chara.data.currentContainer.visuals.EffectChangeFeedback(chara.data.currentContainer, 1, feedbackQueue);

            chara.data.currentContainer.visuals.UpdateBaseElements(chara.data);
        }

        if (targets.Count == 0)
        {
            feedbackQueue.resolved = true;
        }

        while (!feedbackQueue.resolved)//Wait 
        {
            yield return new WaitForEndOfFrame();
        }

        yield return null;
        currentQueue.UpdateQueue();
    }

    void CharacStatsChanger(ref int stat, HeroToCharStatsInfos infos)
    {
        switch (infos.howToImpact)
        {
            case Operation.Add:
                stat += GetHeroStats(infos.heroStatToTake); 
                break;

            case Operation.Substract:
                stat -= GetHeroStats(infos.heroStatToTake);
                break;

            case Operation.Copy:
                stat = GetHeroStats(infos.heroStatToTake);
                break;

            default:
                break;
        }
    }

    int GetHeroStats(HeroStats statToTake)
    {
        var hero = GameManager.Instance.currentHero;

        switch (statToTake)
        {
            case HeroStats.BaseAttack:
                return hero.attackDamage;

            case HeroStats.TempAttack:
                return hero.bonusDamage;

            case HeroStats.MaxHp:
                return hero.maxLifePoints;

            case HeroStats.Hp:
                return hero.lifePoints;

            case HeroStats.MaxGold:
                return hero.maxGoldPoints;

            case HeroStats.Gold:
                return hero.goldPoints;

            default:
                return 0;
        }
    }

}
