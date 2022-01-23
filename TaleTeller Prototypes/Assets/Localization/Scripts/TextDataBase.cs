using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Text Database", menuName = "Database/Text Database", order = 2)]
public class TextDataBase : ScriptableObject
{
#if UNITY_EDITOR
    public TextAsset dataFile;
#endif

    public KeyStringPair[] database;
}
