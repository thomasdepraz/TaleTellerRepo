using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public class DrawDependingCardTypeEffect : DrawTypeEffects
{
    [SerializeField] List<TypeDependantInfos> typeDependants = new List<TypeDependantInfos>();

    [Serializable]
    struct TypeDependantInfos
    {
        internal enum CardType { All, Character, Location, Object }
        internal enum CharacterType { Peacefull, Agressive, Both }
        bool ShowCharacterType => typeToHave == CardType.Character;

        [SerializeField]
        internal CardType typeToHave;
        [SerializeField, ShowIf("ShowCharacterType"), AllowNesting]
        internal CharacterType characterType;
    }
    public EffectValue drawValue;

    public override void InitEffect(CardData card)
    {
        base.InitEffect(card);
        values.Add(drawValue);
    }

    protected override int GetAmountToDraw()
    {
        int amountToDraw = 0;
        var possibleTargets = GetTargets();

        for(int i = 0; i < typeDependants.Count; i++)
        {
            switch (typeDependants[i].typeToHave)
            {
                case TypeDependantInfos.CardType.All:
                    amountToDraw += possibleTargets.Count;
                    break;

                case TypeDependantInfos.CardType.Character:
                    List<CharacterType> targetCharacterTypes = new List<CharacterType>();
                    
                    for(int x = 0; x < possibleTargets.Count; x++)
                    {
                        if(possibleTargets[x].cardType.GetType() == typeof(CharacterType))
                        {
                            targetCharacterTypes.Add((possibleTargets[x].cardType as CharacterType));
                        }
                    }

                    switch(typeDependants[i].characterType)
                    {
                        case TypeDependantInfos.CharacterType.Both:
                            amountToDraw += targetCharacterTypes.Count();
                            break;
                        case TypeDependantInfos.CharacterType.Peacefull:
                            for (int x = 0; x < targetCharacterTypes.Count(); x++)
                            {
                                if (targetCharacterTypes[x].behaviour == CharacterBehaviour.Peaceful)
                                    amountToDraw++;
                            }
                            break;
                        case TypeDependantInfos.CharacterType.Agressive:
                            for(int x = 0; x < targetCharacterTypes.Count(); x++)
                            {
                                if (targetCharacterTypes[x].behaviour == CharacterBehaviour.Agressive)
                                    amountToDraw++;
                            }
                            break;
                    }

                    break;

                case TypeDependantInfos.CardType.Location:
                    amountToDraw += possibleTargets
                        .Select(t => t.cardType)
                        .Where(ct => ct.GetType() == typeof(LocationType))
                        .Count();
                    break;

                case TypeDependantInfos.CardType.Object:
                    amountToDraw += possibleTargets
                        .Select(t => t.cardType)
                        .Where(ct => ct.GetType() == typeof(ObjectType))
                        .Count();
                    break;
            }
        }
        

        return amountToDraw * drawValue.value;
    }
}

