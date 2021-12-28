using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStatsModifierEffect : HeroStatsEffects
{
    public EffectValue gainValue;
    [Tooltip("Check this if you want to the modification to occure only until the end of the story")]
    public bool temporaryChange;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(gainValue);

        if (temporaryChange)
            card.onStoryEnd += StartReverseModification;
    }

    public void StartReverseModification(EventQueue queue)
    {
        queue.events.Add(ReverseModification(queue));
    }

    private IEnumerator ReverseModification(EventQueue queue)
    {

        switch (gainValue.type)
        {
            case EffectValueType.Attack:
                GameManager.Instance.currentHero.bonusDamage = OperateFromValueOperator(GameManager.Instance.currentHero.bonusDamage, true);
                break;

            case EffectValueType.Life:
                GameManager.Instance.currentHero.lifePoints = OperateFromValueOperator(GameManager.Instance.currentHero.lifePoints, true);
                break;

            case EffectValueType.Gold:
                GameManager.Instance.currentHero.goldPoints = OperateFromValueOperator(GameManager.Instance.currentHero.goldPoints, true);
                break;

            case EffectValueType.BaseAttack:
                GameManager.Instance.currentHero.attackDamage = OperateFromValueOperator(GameManager.Instance.currentHero.attackDamage, true);
                break;

            case EffectValueType.MaxLife:
                GameManager.Instance.currentHero.maxLifePoints = OperateFromValueOperator(GameManager.Instance.currentHero.maxLifePoints, true);
                break;

            case EffectValueType.MaxGold:
                GameManager.Instance.currentHero.maxGoldPoints = OperateFromValueOperator(GameManager.Instance.currentHero.maxGoldPoints, true);
                break;
        }

        yield return null;
        queue.UpdateQueue();
    }

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData cardData = null)
    {
        Debug.Log("Hero Gain Effect");
        yield return null;

        switch (gainValue.type)
        {
            case EffectValueType.Attack:
                GameManager.Instance.currentHero.bonusDamage = OperateFromValueOperator(GameManager.Instance.currentHero.bonusDamage);
                break;

            case EffectValueType.Life:
                GameManager.Instance.currentHero.lifePoints = OperateFromValueOperator(GameManager.Instance.currentHero.lifePoints);
                break;

            case EffectValueType.Gold:
                GameManager.Instance.currentHero.goldPoints = OperateFromValueOperator(GameManager.Instance.currentHero.goldPoints);
                break;

            case EffectValueType.BaseAttack:
                GameManager.Instance.currentHero.attackDamage = OperateFromValueOperator(GameManager.Instance.currentHero.attackDamage);
                break;

            case EffectValueType.MaxLife:
                GameManager.Instance.currentHero.maxLifePoints = OperateFromValueOperator(GameManager.Instance.currentHero.maxLifePoints);
                break;

            case EffectValueType.MaxGold:
                GameManager.Instance.currentHero.maxGoldPoints = OperateFromValueOperator(GameManager.Instance.currentHero.maxGoldPoints);
                break;
        }

        currentQueue.UpdateQueue();
    }

    int OperateFromValueOperator(int valueChanged, bool reversed = false)
    {
        switch(gainValue.op)
        {
            case EffectValueOperator.Addition:
                valueChanged = reversed ? valueChanged - gainValue.value : valueChanged + gainValue.value;
                break;

            case EffectValueOperator.Substraction:
                valueChanged = reversed ? valueChanged + gainValue.value : valueChanged - gainValue.value;
                break;

            case EffectValueOperator.Product:
                valueChanged = reversed ? valueChanged / gainValue.value : valueChanged * gainValue.value;
                break;

            case EffectValueOperator.Division:
                valueChanged = reversed ? valueChanged * gainValue.value : valueChanged / gainValue.value;
                break;
        }

        return valueChanged;
    }
}
