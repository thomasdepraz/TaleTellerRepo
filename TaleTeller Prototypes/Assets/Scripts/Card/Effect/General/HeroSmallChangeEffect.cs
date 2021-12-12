using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSmallChangeEffect : BonusEffect
{
    public EffectValue gainValue;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);

        values.Add(gainValue);
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

    int OperateFromValueOperator(int valueChanged)
    {
        switch(gainValue.op)
        {
            case EffectValueOperator.Addition:
                valueChanged += gainValue.value;
                break;

            case EffectValueOperator.Substraction:
                valueChanged -= gainValue.value;
                break;

            case EffectValueOperator.Product:
                valueChanged *= gainValue.value;
                break;

            case EffectValueOperator.Division:
                valueChanged /= gainValue.value;
                break;
        }

        return valueChanged;
    }
}
