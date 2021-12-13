using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    [Header("Data")]
    public int maxManaBase;
    public int maxManaModifier;
    public int currentMana;

    public int ActualMaxMana
    {
        get 
        {
            int manaToReturn = (int)Mathf.Clamp(maxManaBase + maxManaModifier, 0, Mathf.Infinity);
            return manaToReturn; 
        }
    }

    //ManaPool Modifier CallStackSystem
    public struct ManaPoolModifier
    {
        internal int modifierValue;
        internal int turnBeforeFade;
        internal Effect source;
        internal string explainerText;

        public ManaPoolModifier(int _value, int _turn, Effect _source, string _explainer = null)
        {
            modifierValue = _value;
            turnBeforeFade = _turn;
            source = _source;
            explainerText = _explainer;
        }
    }
    private List<ManaPoolModifier> poolModifiers = new List<ManaPoolModifier>();

    void Start()
    {

    }

    public void StartTurnManaInit()
    {
        for (int i = 0; i < poolModifiers.Count(); i++)
        {
            var modifier = poolModifiers[i];

            if (modifier.turnBeforeFade <= 0)
                poolModifiers.Remove(modifier);
            else
            {
                maxManaModifier += modifier.modifierValue;
                modifier.turnBeforeFade--;
            }
        }

        currentMana = ActualMaxMana;
    }

    public void AddManaPoolModifier(ManaPoolModifier modifier)
    {
        poolModifiers.Add(modifier);
    }

    public void GainMana(int amount)
    {
        if (currentMana + amount >= ActualMaxMana) currentMana = ActualMaxMana;
        else currentMana += amount;
    }

    public void LoseMana(int amount)
    {
        if (currentMana - amount <= 0) currentMana = 0;
        else currentMana -= amount;
    }
    public bool CanUseCard(int cardCost)
    {
        if (currentMana - cardCost < 0) return false;
        else return true;
    }
}
