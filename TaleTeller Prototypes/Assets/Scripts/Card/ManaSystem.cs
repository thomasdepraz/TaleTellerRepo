using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    [Header("Data")]
    public int maxMana;
    public int currentMana;

    void Start()
    {

    }

    public void RefillMana()
    {
        currentMana = maxMana;
    }

    public void GainMana(int amount)
    {
        if (currentMana + amount >= maxMana) currentMana = maxMana;
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
