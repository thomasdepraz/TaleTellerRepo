using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instructions Profile", menuName ="Data/Instructions Profile")]
public class InstructionsData : ScriptableObject
{
    [TextArea(2,5)]
    public string chooseSchemeInstruction;
    public string chooseSchemeStepInstruction;
    public string chooseCardInstruction;
    public string choiceEffectInstruction;

}
