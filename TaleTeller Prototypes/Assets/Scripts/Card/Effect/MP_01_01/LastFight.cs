using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastFight : Effect
{
    private CardData lastCharFought = null;
    private bool foughtHeroLast = false;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        linkedData.onCharFight += SaveLastTarget;
    }

    private void SaveLastTarget(EventQueue queue, CardData data)
    {
        if (data == null)
        {
            foughtHeroLast = true;
            lastCharFought = null;
        }
        else
        {
            foughtHeroLast = false;
            lastCharFought = data;
        }
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        if (foughtHeroLast)
            GameManager.Instance.currentHero.lifePoints -= linkedData.characterStats.baseAttackDamage;
        else if(lastCharFought != null)
            lastCharFought.characterStats.baseLifePoints -= linkedData.characterStats.baseAttackDamage;

        yield return null;
        currentQueue.UpdateQueue();
    }
}
