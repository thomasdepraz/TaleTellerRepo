using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : Singleton<ScreenManager>
{
    public void Awake()
    {
        CreateSingleton();
    }
    public RewardScreenVisuals rewardScreenVisuals;
    public ChapterScreenVisuals chapterScreenVisuals;
    public ChooseCardRewardScreenVisuals chooseCardRewardScreenVisuals;
    public CardPickerScreenVisual pickerScreenVisual;

    public GameScreen currentScreen;
}
