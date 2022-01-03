using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct KeyStringPair
{
    public int key;
    public int text;
}

[CreateAssetMenu(fileName = "New Instructions Database", menuName = "Instruction Database", order = 100)]
public class InstructionsTextData : ScriptableObject
{
#if UNITY_EDITOR
    public TextAsset dataFile;
#endif

    public KeyStringPair instructions;

}
