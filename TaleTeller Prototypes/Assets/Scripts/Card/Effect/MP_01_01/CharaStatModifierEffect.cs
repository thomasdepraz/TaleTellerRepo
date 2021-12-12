using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Remi Secher - 12/10/2021 01:56 - Creation
/// </summary>

public class CharaStatModifierEffect : Effect
{
    public enum TargetBehaviour {Both, Peacefull, Agressive };

    public EffectValue modifierValue;
    public TargetBehaviour behaviourTargeted;

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        IEnumerable<CharacterType> targets = GetTargets()
            .Select(t => t.cardType)
            .Where(t => t.GetType() == typeof(CharacterType))
            .Cast<CharacterType>();

        switch (behaviourTargeted)
        {
            case TargetBehaviour.Peacefull:
                targets = targets.Where(t => t.behaviour == CharacterBehaviour.Peaceful);
                break;
            case TargetBehaviour.Agressive:
                targets = targets.Where(t => t.behaviour == CharacterBehaviour.Agressive);
                break;
            default:
                break;
        }

        //TODO: Inset queue for feedback
        foreach (CharacterType t in targets.ToList())
        {
            switch (modifierValue.type)
            {
                case EffectValueType.Attack:

                    switch (modifierValue.op)
                    {
                        case EffectValueOperator.Addition:
                            t.stats.baseAttackDamage += modifierValue.value;
                            break;
                        case EffectValueOperator.Division:
                            t.stats.baseAttackDamage /= modifierValue.value;
                            break;
                        case EffectValueOperator.Product:
                            t.stats.baseAttackDamage *= modifierValue.value;
                            break;
                        case EffectValueOperator.Substraction:
                            t.stats.baseAttackDamage -= modifierValue.value;
                            t.stats.baseAttackDamage = (int)Mathf.Clamp(t.stats.baseAttackDamage, 0, Mathf.Infinity);
                            break;
                        default:
                            break;
                    }

                    break;
                case EffectValueType.Life:

                    switch (modifierValue.op)
                    {
                        case EffectValueOperator.Addition:
                            t.stats.baseLifePoints += modifierValue.value;
                            break;
                        case EffectValueOperator.Division:
                            t.stats.baseLifePoints /= modifierValue.value;
                            break;
                        case EffectValueOperator.Product:
                            t.stats.baseLifePoints *= modifierValue.value;
                            break;
                        case EffectValueOperator.Substraction:
                            t.stats.baseLifePoints -= modifierValue.value;
                            break;
                        default:
                            break;
                    }

                    //NOTE: This situation justify that the process to damage a character need to be encapsulated somewhere
                    EventQueue characterDeathQueue = new EventQueue();

                    if (t.stats.baseLifePoints <= 0)
                    {
                        t.CharacterDeath(false, characterDeathQueue);//<-- Add character to queue events
                    }

                    characterDeathQueue.StartQueue();///<-- actually character death

                    while (!characterDeathQueue.resolved)//Wait 
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;

                default:
                    break;
            }
            t.data.currentContainer.UpdateCharacterInfo(t);
        }
        yield return null;
        currentQueue.UpdateQueue();
    }
}
