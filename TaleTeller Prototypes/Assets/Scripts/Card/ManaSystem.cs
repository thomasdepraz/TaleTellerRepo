using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaSystem : MonoBehaviour
{
    [Header("Data")]
    public int _maxManaBase;
    public int _maxManaModifier;
    public int _currentMana;

    public int maxManaBase
    {
        get => _maxManaBase;
        set
        {
            _maxManaBase = value;
            UpdateManaText();
        }
    }
    public int maxManaModifier
    {
        get => _maxManaModifier;
        set
        {
            _maxManaModifier = value;
            if (currentMana > ActualMaxMana)
                currentMana = ActualMaxMana;
            UpdateManaText();
        }
    }
    public int currentMana
    {
        get => _currentMana;
        set
        {
            _currentMana = value;
            UpdateManaText();
        }
    }
    public int ActualMaxMana
    {
        get 
        {
            int manaToReturn = (int)Mathf.Clamp(maxManaBase + maxManaModifier, 0, Mathf.Infinity);
            return manaToReturn; 
        }
    }

    [Header("References")]
    public Image manaFrame;
    public Image manaFill;
    public TextMeshProUGUI manaCountText;

    //ManaPool Modifier CallStackSystem
    public class ManaPoolModifier
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
        //Init Text correctly
        UpdateManaText();
    }

    public void StartTurnManaInit()
    {
        maxManaModifier = 0;

        for (int i = 0; i < poolModifiers.Count(); i++)
        {
            var modifier = poolModifiers[i];

            if (modifier.turnBeforeFade < 0)
                poolModifiers.Remove(modifier);
            else
            {
                maxManaModifier += modifier.modifierValue;
                modifier.turnBeforeFade--;
            }
        }

        currentMana = ActualMaxMana;
    }

    public void AddManaPoolModifier(ManaPoolModifier modifier, bool triggerNextTurn)
    {
        poolModifiers.Add(modifier);

        if (!triggerNextTurn)
        {
            maxManaModifier += modifier.modifierValue;
            modifier.turnBeforeFade -= 1;
        }
    }

    public ManaPoolModifier CreateManaPoolModifier(int _value, int _turn, Effect _source, string _explainer = null)
    {
        var modifier = new ManaPoolModifier(_value, _turn, _source, _explainer);
        return modifier;
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

    void UpdateManaText()
    {
        manaCountText.text = currentMana.ToString() + "/" + ActualMaxMana.ToString();
        manaFill.fillAmount = (float)currentMana / (float)ActualMaxMana;
    }
}
