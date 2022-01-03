using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InstructionsTextData))]
public class InstructionsTextDataInspector : Editor
{
    InstructionsTextData script;
    SerializedProperty file;
    SerializedProperty arrayList;

    public void OnEnable()
    {
        script = target as InstructionsTextData;
        file = serializedObject.FindProperty(nameof(script.dataFile));
        arrayList = serializedObject.FindProperty(nameof(script.instructions));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(file);
        if (GUILayout.Button("ParseCSV"))
        {
            List<string[]> csvstrings = CSVUtility.ParseCSV(script.dataFile);
            script.instructions = GenerateInstructions(csvstrings);
            EditorUtility.SetDirty(script);
        }

        EditorGUILayout.PropertyField(arrayList);


        serializedObject.ApplyModifiedProperties();
    }

    KeyStringPair[] GenerateInstructions(List<string[]> content)
    {
        if (content == null) return null;

        KeyStringPair[] list = new KeyStringPair[content.Count];
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = GenerateInstruction(content[i]);
        }

        return list;
    }
    KeyStringPair GenerateInstruction(string[] data)
    {
        KeyStringPair result = new KeyStringPair();
        result.key = data[0];
        result.text = data[1];

        return result;
    }
}
