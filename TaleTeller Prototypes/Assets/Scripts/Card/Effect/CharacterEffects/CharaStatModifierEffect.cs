using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using NaughtyAttributes;

/// <summary>
/// Remi Secher - 12/10/2021 01:56 - Creation
/// </summary>

public class CharaStatModifierEffect : CharacterStatsEffect
{
    public enum TargetBehaviour {Both, Peacefull, Agressive };

    public EffectValue modifierValue;
    public TargetBehaviour behaviourTargeted;

    public bool forSpecificCard;
    [ShowIf("forSpecificCard")]
    public CardData specificCard;

    public bool temporaryChange;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(modifierValue);

        if (temporaryChange)
            card.onStoryEnd += StartReverseModification;
    }

    private void StartReverseModification(EventQueue queue)
    {
        queue.events.Add(ReverseModification(queue));
    }

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

        EventQueue feedbackQueue = new EventQueue();

        foreach (CharacterType t in targets.ToList())
        {
            if (forSpecificCard)
                if (t.data.cardName != specificCard.cardName)
                {
                    feedbackQueue.resolved = true;
                    break;
                }
                    

            switch (modifierValue.type)
            {
                case EffectValueType.Attack:

                    switch (modifierValue.op)
                    {
                        case EffectValueOperator.Addition:
                            t.stats.baseAttackDamage += modifierValue.value;
                            CardManager.Instance.cardTweening.EffectChangeFeedback(t.data.currentContainer, 1, modifierValue.value, feedbackQueue, false);

                            break;
                        case EffectValueOperator.Division:
                            t.stats.baseAttackDamage /= modifierValue.value;
                            CardManager.Instance.cardTweening.EffectChangeFeedback(t.data.currentContainer, -1, modifierValue.value, feedbackQueue, false);
                            break;
                        case EffectValueOperator.Product:
                            t.stats.baseAttackDamage *= modifierValue.value;
                            CardManager.Instance.cardTweening.EffectChangeFeedback(t.data.currentContainer, 1, modifierValue.value, feedbackQueue, false);
                            break;
                        case EffectValueOperator.Substraction:
                            t.stats.baseAttackDamage -= modifierValue.value;
                            t.stats.baseAttackDamage = (int)Mathf.Clamp(t.stats.baseAttackDamage, 0, Mathf.Infinity);
                            CardManager.Instance.cardTweening.EffectChangeFeedback(t.data.currentContainer, -1, modifierValue.value, feedbackQueue, false);
                            break;
                        default:
                            feedbackQueue.resolved = true;
                            break;
                    }

                    //t.data.currentContainer.UpdateBaseInfo();
                    t.data.currentContainer.UpdateCharacterInfo(t);

                    break;
                case EffectValueType.Life:

                    switch (modifierValue.op)
                    {
                        case EffectValueOperator.Addition:
                            t.stats.baseLifePoints += modifierValue.value;
                            CardManager.Instance.cardTweening.EffectChangeFeedback(t.data.currentContainer, 1, modifierValue.value, feedbackQueue, false);

                            break;
                        case EffectValueOperator.Division:
                            t.stats.baseLifePoints /= modifierValue.value;
                            CardManager.Instance.cardTweening.EffectChangeFeedback(t.data.currentContainer, -1, modifierValue.value, feedbackQueue, false);
                            break;
                        case EffectValueOperator.Product:
                            t.stats.baseLifePoints *= modifierValue.value;
                            CardManager.Instance.cardTweening.EffectChangeFeedback(t.data.currentContainer, 1, modifierValue.value, feedbackQueue, false);

                            break;
                        case EffectValueOperator.Substraction:
                            t.stats.baseLifePoints -= modifierValue.value;
                            CardManager.Instance.cardTweening.EffectChangeFeedback(t.data.currentContainer, -1, modifierValue.value, feedbackQueue, false);
                            break;
                        default:
                            feedbackQueue.resolved = true;
                            break;
                    }

                    t.data.currentContainer.UpdateCharacterInfo(t);

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
            //t.data.currentContainer.UpdateCharacterInfo(t);
        }

        if(targets.ToArray().Length == 0)
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

    private IEnumerator ReverseModification(EventQueue queue)
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
                            t.stats.baseAttackDamage -= modifierValue.value;
                            t.stats.baseAttackDamage = (int)Mathf.Clamp(t.stats.baseAttackDamage, 0, Mathf.Infinity);
                            break;
                        case EffectValueOperator.Division:
                            t.stats.baseAttackDamage *= modifierValue.value;
                            break;
                        case EffectValueOperator.Product:
                            t.stats.baseAttackDamage /= modifierValue.value;
                            break;
                        case EffectValueOperator.Substraction:
                            t.stats.baseAttackDamage += modifierValue.value;
                            break;
                        default:
                            break;
                    }

                    break;
                case EffectValueType.Life:

                    switch (modifierValue.op)
                    {
                        case EffectValueOperator.Addition:
                            t.stats.baseLifePoints -= modifierValue.value;
                            break;
                        case EffectValueOperator.Division:
                            t.stats.baseLifePoints *= modifierValue.value;
                            break;
                        case EffectValueOperator.Product:
                            t.stats.baseLifePoints /= modifierValue.value;
                            break;
                        case EffectValueOperator.Substraction:
                            t.stats.baseLifePoints += modifierValue.value;
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
        queue.UpdateQueue();
    }
}
