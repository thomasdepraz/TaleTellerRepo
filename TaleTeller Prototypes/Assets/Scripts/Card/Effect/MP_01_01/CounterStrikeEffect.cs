using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Remi Secher - 12/10/2021 01:14 - Creation
/// </summary>
public class CounterStrikeEffect : Effect
{
    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        CharacterType character = linkedData.cardType as CharacterType;
        CharacterType other = data.cardType as CharacterType;

        //TODO: Implement Queue for feedback
        other.stats.baseLifePoints -= character.stats.baseAttackDamage;

        yield return null;
        currentQueue.UpdateQueue();
    }
}
