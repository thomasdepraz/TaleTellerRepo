using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextDataBase))]
public class TextDataBaseInspector : Editor
{
    TextDataBase script;

    public void OnEnable()
    {
        script = target as TextDataBase;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("ParseCSV"))
        {
            List<string[]> csvstrings = CSVUtility.ParseCSV(script.dataFile);
            script.database = GeneratePairs(csvstrings);
            EditorUtility.SetDirty(script);
        }
    }

    KeyStringPair[] GeneratePairs(List<string[]> content)
    {
        if (content == null) return null;

        KeyStringPair[] list = new KeyStringPair[content.Count];
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = GeneratePair(content[i]);
        }

        return list;
    }
    KeyStringPair GeneratePair(string[] data)
    {
        KeyStringPair result = new KeyStringPair();
        result.key = data[0];
        result.text = data[1];

        return result;
    }
}
