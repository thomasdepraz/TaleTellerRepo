using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct KeyStringPair
{
    public string key;
    public string text;
}

[CreateAssetMenu(fileName = "New Instructions Database", menuName = "Database/Instructions Database", order = 2)]
public class InstructionsTextData : ScriptableObject
{
#if UNITY_EDITOR
    public TextAsset dataFile;
#endif

    public KeyStringPair[] instructions;

}
