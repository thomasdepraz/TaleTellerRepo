using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlotCard))]
public class PlotCardInspector : Editor
{
    PlotCard script;
    SerializedProperty cardDataReference;
    SerializedProperty cardName;
    SerializedProperty cardCost;
    SerializedProperty cardRarity;
    SerializedProperty cardArchetype;
    SerializedProperty cardGraph;
    SerializedProperty effects;
    SerializedProperty cardDescription;

    SerializedProperty cardType;

    //PlotCard variables
    SerializedProperty plotObjective;
    SerializedProperty completionTimer;
    SerializedProperty isMainPlot;
    SerializedProperty isFinal;
    SerializedProperty legendaryList;

    private void OnEnable()
    {
        script = target as PlotCard;

        cardDataReference = serializedObject.FindProperty(nameof(script.dataReference));
        cardType = serializedObject.FindProperty(nameof(script.cardTypeReference));
        cardName = serializedObject.FindProperty(nameof(script.cardName));
        cardCost = serializedObject.FindProperty(nameof(script.manaCost));
        cardRarity = serializedObject.FindProperty(nameof(script.rarity));
        cardArchetype = serializedObject.FindProperty(nameof(script.archetype));
        cardGraph = serializedObject.FindProperty(nameof(script.cardGraph));
        effects = serializedObject.FindProperty(nameof(script.effectsReferences));
        cardDescription = serializedObject.FindProperty(nameof(script.description));


        plotObjective = serializedObject.FindProperty(nameof(script.objective));
        completionTimer = serializedObject.FindProperty(nameof(script.completionTimer));
        isMainPlot = serializedObject.FindProperty(nameof(script.isMainPlot));
        isFinal = serializedObject.FindProperty(nameof(script.isFinal));
        legendaryList = serializedObject.FindProperty(nameof(script.legendaryCardRewards));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(cardDataReference);
        //if (cardDataReference.objectReferenceValue == null) EditorGUILayout.HelpBox("Card Data Reference must not be null !", MessageType.Error);
        EditorGUILayout.LabelField("Card Base", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(cardName);
        EditorGUILayout.PropertyField(cardCost);
        EditorGUILayout.PropertyField(cardRarity);
        EditorGUILayout.PropertyField(cardArchetype);
        EditorGUILayout.PropertyField(cardGraph);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.PropertyField(effects);

        EditorGUILayout.BeginHorizontal();
        //Add effect
        if (GUILayout.Button("AddEffect", GUILayout.MaxWidth(90)))
        {
            GenericMenu menu = new GenericMenu();
            List<Type> effectTypes = UtilityClass.GetSubClasses(typeof(Effect));
            for (int i = 0; i < effectTypes.Count; i++)
            {
                if (UtilityClass.HasSubClasses(effectTypes[i]))
                {
                    menu.AddSeparator("");
                    List<Type> nestedTypes = UtilityClass.GetSubClasses(effectTypes[i]);
                    for (int j = 0; j < nestedTypes.Count; j++)
                    {
                        AddMenuEffectItem(menu, effectTypes[i].Name + "/" + nestedTypes[j].Name, nestedTypes[j]);
                    }
                }
                else
                {
                    AddMenuEffectItem(menu, effectTypes[i].Name, effectTypes[i]);
                }
            }

            //Display the menu
            menu.ShowAsContext();
        }
        if (effects.arraySize > 0)
        {
            if (GUILayout.Button("RemoveEffect", GUILayout.MaxWidth(90)))
            {
                //delete the child asset
                AssetDatabase.RemoveObjectFromAsset(script.effectsReferences[script.effectsReferences.Count - 1]);
                script.effectsReferences.RemoveAt(script.effectsReferences.Count - 1);
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(script);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(cardDescription);

        serializedObject.ApplyModifiedProperties();

        serializedObject.Update();


        GUILayout.Space(20);
        EditorGUILayout.LabelField("Plot Card Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(plotObjective);
        if (plotObjective.objectReferenceValue == null)
        {
            if (GUILayout.Button("Add Objective", GUILayout.MaxWidth(90)))
            {
                GenericMenu menu = new GenericMenu();
                List<Type> objectiveTypes = UtilityClass.GetSubClasses(typeof(PlotObjective));
                for (int i = 0; i < objectiveTypes.Count; i++)
                {
                    if (UtilityClass.HasSubClasses(objectiveTypes[i]))
                    {
                        menu.AddSeparator("");
                        List<Type> nestedTypes = UtilityClass.GetSubClasses(objectiveTypes[i]);
                        for (int j = 0; j < nestedTypes.Count; j++)
                        {
                            AddMenuObjectiveItem(menu, objectiveTypes[i].Name + "/" + nestedTypes[j].Name, nestedTypes[j]);
                        }
                    }
                    else
                    {
                        AddMenuObjectiveItem(menu, objectiveTypes[i].Name, objectiveTypes[i]);
                    }
                }

                //Display the menu
                menu.ShowAsContext();
            }
        }
        else
        {
            if (GUILayout.Button("Remove Objective", GUILayout.MaxWidth(120)))
            {
                //delete the child asset
                AssetDatabase.RemoveObjectFromAsset(script.objective);
                script.objective = null;
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(script);
            }
        }

        GUILayout.Space(10);
        EditorGUILayout.PropertyField(completionTimer);
        if (completionTimer.intValue <= 0)
        {
            completionTimer.intValue = 0;
            EditorGUILayout.HelpBox("Completion Timer value should at least be 1", MessageType.Warning);
        }


        EditorGUILayout.PropertyField(isMainPlot);
        if (isMainPlot.boolValue == true)
        {
            EditorGUILayout.PropertyField(isFinal);
            EditorGUILayout.PropertyField(legendaryList);
        }
        
        serializedObject.ApplyModifiedProperties();


        GUILayout.Space(10);
        EditorGUILayout.LabelField("Card Type", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Type", GUILayout.MaxWidth(90)))
        {
            GenericMenu menu = new GenericMenu();
            AddMenuItem(menu, "None", typeof(CardTypes));
            List<Type> cardTypes = UtilityClass.GetSubClasses(typeof(CardTypes));
            for (int i = 0; i < cardTypes.Count; i++)
            {
                AddMenuItem(menu, cardTypes[i].Name, cardTypes[i]);
            }

            //Display the menu
            menu.ShowAsContext();
        }

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.PropertyField(cardType);

        serializedObject.ApplyModifiedProperties();
    }

    #region Menu Methods
    public void AddMenuItem(GenericMenu menu, string path, Type type)
    {
        menu.AddItem(new GUIContent(path), false, OnTypeSelected, type);
    }

    public void AddMenuEffectItem(GenericMenu menu, string path, Type type)
    {
        menu.AddItem(new GUIContent(path), false, OnEffectSelected, type);
    }

    public void AddMenuObjectiveItem(GenericMenu menu, string path, Type type)
    {
        menu.AddItem(new GUIContent(path), false, OnObjectiveSelected, type);
    }

    void OnEffectSelected(object type)
    {
        //var instance = Activator.CreateInstance((Type)type);
        ScriptableObject instance = ScriptableObject.CreateInstance((Type)type);
        instance.name = "Effect" + effects.arraySize;

        //AssetDatabase.CreateAsset(instance, "Assets/" + nameof(type) +".asset");
        AssetDatabase.AddObjectToAsset(instance, serializedObject.targetObject);

        // Reimport the asset after adding an object.
        // Otherwise the change only shows up when saving the project
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(instance));
        AssetDatabase.SaveAssets();

        //cardType.objectReferenceValue = instance as CardTypes;
        script.effectsReferences.Add(instance as Effect);
        EditorUtility.SetDirty(script);
    }

    void OnTypeSelected(object type)
    {
        if ((Type)type == typeof(CardTypes))
        {
            if (script.cardTypeReference != null)
            {
                //delete the child asset
                AssetDatabase.RemoveObjectFromAsset(script.cardTypeReference);
                AssetDatabase.SaveAssets();
                script.cardTypeReference = null;
            }
        }
        else
        {
            if (script.cardTypeReference != null)
            {
                //delete the child asset
                AssetDatabase.RemoveObjectFromAsset(script.cardTypeReference);
                AssetDatabase.SaveAssets();
                script.cardTypeReference = null;
            }

            //var instance = Activator.CreateInstance((Type)type);
            ScriptableObject instance = ScriptableObject.CreateInstance((Type)type);
            instance.name = "Card Type";

            //AssetDatabase.CreateAsset(instance, "Assets/" + nameof(type) +".asset");
            AssetDatabase.AddObjectToAsset(instance, serializedObject.targetObject);

            // Reimport the asset after adding an object.
            // Otherwise the change only shows up when saving the project
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(instance));
            AssetDatabase.SaveAssets();

            //cardType.objectReferenceValue = instance as CardTypes;
            script.cardTypeReference = instance as CardTypes;
            EditorUtility.SetDirty(script);
        }

    }

    void OnObjectiveSelected(object type)
    {
        //var instance = Activator.CreateInstance((Type)type);
        ScriptableObject instance = ScriptableObject.CreateInstance((Type)type);
        instance.name = "PlotObjective_" + type.ToString();

        AssetDatabase.AddObjectToAsset(instance, serializedObject.targetObject);

        // Reimport the asset after adding an object.
        // Otherwise the change only shows up when saving the project
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(instance));
        AssetDatabase.SaveAssets();

        //cardType.objectReferenceValue = instance as CardTypes;
        script.objective = instance as PlotObjective;
        EditorUtility.SetDirty(script);
    }
    #endregion

}
