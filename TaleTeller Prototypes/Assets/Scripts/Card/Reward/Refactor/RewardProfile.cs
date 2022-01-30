using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/Reward Profile", fileName= "New Reward Profile")]
public class RewardProfile : ScriptableObject
{
    [Header("Colors")]
    public Color commonColor;
    public Color rareColor;
    public Color epicColor;

    [Header("Sprites")]
    public Sprite inspireIcon;
    public Sprite maxStatsIcon;
    public Sprite tempStatsIcon;

    [Header("Data")]
    public Vector3 rewardTypeRates;
    public List<Vector3> actDropRates = new List<Vector3>();
    public List<Vector3> tempStatsRewardValues = new List<Vector3>();
    public List<Vector3> maxStatsRewardValues = new List<Vector3>();




}
