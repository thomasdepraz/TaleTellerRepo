using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor
{
    public CardData script;

    public SerializedProperty effectList;

    private void OnEnable()
    {
        script = target as CardData;
        effectList = serializedObject.FindProperty(nameof(script.effectsReferences));
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(effectList);
        if(GUILayout.Button("AddEffect"))
        {
            GenericMenu menu = new GenericMenu();
            List<Type> effectTypes = UtilityClass.GetSubClasses(typeof(Effect));
            for (int i = 0; i < effectTypes.Count; i++)
            {
                if(UtilityClass.HasSubClasses(effectTypes[i]))
                {
                    menu.AddSeparator("");
                    List<Type> nestedTypes = UtilityClass.GetSubClasses(effectTypes[i]);
                    for (int j = 0; j < nestedTypes.Count; j++)
                    {
                        AddMenuItem(menu, effectTypes[i].Name + "/" + nestedTypes[j].Name, nestedTypes[j]);
                    }
                }
                else
                {
                    AddMenuItem(menu, effectTypes[i].Name, effectTypes[i]);
                }
            }



            //Display the menu
            menu.ShowAsContext();
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void AddMenuItem(GenericMenu menu, string path, Type type)
    {
        menu.AddItem(new GUIContent(path), false, OnTypeSelected, type);
    }
    void OnTypeSelected(object type)
    {
        var instance = Activator.CreateInstance((Type)type);
        script.effectsReferences.Add(instance as Effect);
    }
}
