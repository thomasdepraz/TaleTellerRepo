using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instructions Profile", menuName ="Data/Instructions Profile")]
public class InstructionsData : ScriptableObject
{
    public string chooseSchemeInstruction;
    public string chooseSchemeStepInstruction;
    public string chooseSecondayPlotInstruction;
    public string chooseCardInstruction;
    public string chooseXCardToDiscardInstruction;
    public string choiceEffectInstruction;
}
