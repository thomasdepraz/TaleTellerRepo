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

    private AudioSource audioSource;

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

        ScaleFrame();
        
    }

    public void LoseMana(int amount)
    {
        if (currentMana - amount <= 0) currentMana = 0;
        else currentMana -= amount;

        ScaleFrame();
    }

    public bool CanUseCard(int cardCost)
    {
        if (currentMana - cardCost < 0)
        {
            ShakeFrame();
            if (audioSource == null) audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
            Sound intervert = new Sound(audioSource, "SFX_OUTOFINK", SoundType.SFX, false, false);
            intervert.Play();
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ScaleFrame()
    {
        LeanTween.cancel(manaFrame.gameObject);
        manaFrame.gameObject.transform.localScale = Vector3.one;
        manaFrame.gameObject.transform.rotation = Quaternion.identity;
        LeanTween.scale(manaFrame.gameObject, Vector3.one * 1.2f, 0.2f).setEaseInOutQuint().setLoopPingPong(1);
    }

    public void ShakeFrame()
    {
        LeanTween.cancel(manaFrame.gameObject);
        manaFrame.gameObject.transform.localScale = Vector3.one;
        manaFrame.gameObject.transform.rotation = Quaternion.identity;
        LeanTween.rotateZ(manaFrame.gameObject, -3, 0.02f).setEaseInOutCubic().setOnComplete(
           value => LeanTween.rotateZ(manaFrame.gameObject, 3, 0.04f).setEaseInOutCubic().setLoopPingPong(2).setOnComplete(
               val => LeanTween.rotateZ(manaFrame.gameObject, 0, 0.02f).setEaseInOutCubic()));
    }

    void UpdateManaText()
    {
        manaCountText.text = currentMana.ToString() + "/" + ActualMaxMana.ToString();
        manaFill.fillAmount = (float)currentMana / (float)ActualMaxMana;
    }
}
