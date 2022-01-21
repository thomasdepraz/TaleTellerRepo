using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class HeroStatsModifierEffect : HeroStatsEffects
{
    public EffectValue gainValue;
    [Tooltip("Check this if you want to the modification to occure only until the end of the story")]
    public bool temporaryChange;

    public bool handCardsDependant;

    [ShowIf("handCardsDependant")]
    [SerializeField] List<TypeDependantInfos> typeDependants = new List<TypeDependantInfos>();

    [Serializable]
    struct TypeDependantInfos
    {
        internal enum CardType { Character, Location, Object }
        internal enum CharacterType { Peacefull, Agressive, Both }
        bool ShowCharacterType => typeToHave == CardType.Character;

        [SerializeField]
        internal CardType typeToHave;
        [SerializeField, ShowIf("ShowCharacterType"), AllowNesting]
        internal CharacterType characterType;
    }

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
        EventQueue feedbackQueue = new EventQueue();

        int finalGainValue = 0;

        if (handCardsDependant)
        {
            for (int i = 0; i < CardManager.Instance.cardHand.currentHand.Count; i++)
            {
                for(int x = 0; x < typeDependants.Count; x++)
                {
                    switch(typeDependants[x].typeToHave)
                    {
                        case TypeDependantInfos.CardType.Character:
                            if (CardManager.Instance.cardHand.currentHand[i].data.cardType.GetType() == typeof(CharacterType))
                            {
                                switch(typeDependants[x].characterType)
                                {
                                    case TypeDependantInfos.CharacterType.Both:
                                        finalGainValue++;
                                        break;
                                    case TypeDependantInfos.CharacterType.Peacefull:
                                        if((CardManager.Instance.cardHand.currentHand[i].data.cardType as CharacterType).behaviour == CharacterBehaviour.Peaceful)
                                            finalGainValue++;
                                        break;
                                    case TypeDependantInfos.CharacterType.Agressive:
                                        if ((CardManager.Instance.cardHand.currentHand[i].data.cardType as CharacterType).behaviour == CharacterBehaviour.Agressive)
                                            finalGainValue++;
                                        break;
                                }
                            }
                            break;
                        case TypeDependantInfos.CardType.Object:
                            if (CardManager.Instance.cardHand.currentHand[i].data.cardType.GetType() == typeof(ObjectType))
                                finalGainValue++;
                            break;
                        case TypeDependantInfos.CardType.Location:
                            if (CardManager.Instance.cardHand.currentHand[i].data.cardType.GetType() == typeof(LocationType))
                                finalGainValue++;
                            break;
                    }
                    
                }
                
            }

            finalGainValue *= gainValue.value;
        }
        else
            finalGainValue = gainValue.value;

        switch(gainValue.op)
        {
            case EffectValueOperator.Addition:
                valueChanged = reversed ? valueChanged - finalGainValue : valueChanged + finalGainValue;
                break;

            case EffectValueOperator.Substraction:
                valueChanged = reversed ? valueChanged + finalGainValue : valueChanged - finalGainValue;
                break;

            case EffectValueOperator.Product:
                valueChanged = reversed ? valueChanged / finalGainValue : valueChanged * finalGainValue;
                break;

            case EffectValueOperator.Division:
                valueChanged = reversed ? valueChanged * finalGainValue : valueChanged / finalGainValue;
                break;
        }

        return valueChanged;
    }
}
