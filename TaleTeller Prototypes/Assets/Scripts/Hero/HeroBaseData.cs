using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroData", menuName = "Data/Hero", order =2)]
public class HeroBaseData : ScriptableObject
{
    [Header("Stats")]
    public int baseLifePoints;
    public int baseAttackDamage;
    public int baseGold;
    public int baseMaxGold;
}
