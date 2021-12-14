using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Remi Secher - 12/10/2021 00:46 - Creation
/// </summary>

public class LastFightEffect : CharacterCombatEffect
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
        CharacterType character = linkedData.cardType as CharacterType;
        if (foughtHeroLast)
        {
            GameManager.Instance.currentHero.lifePoints -= character.stats.baseAttackDamage;//TODO Implement queuing for feedback
        }
        else if (lastCharFought != null)
        {
            CharacterType other = lastCharFought.cardType as CharacterType;

            other.stats.baseLifePoints -= character.stats.baseAttackDamage;
        }

        yield return null;
        currentQueue.UpdateQueue();
    }
}
