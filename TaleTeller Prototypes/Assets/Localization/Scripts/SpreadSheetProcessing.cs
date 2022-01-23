#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpreadSheetProcessing : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (importedAssets == null) return;
        if (importedAssets.Length == 0) return;

        for (int i = 0; i < importedAssets.Length; i++)
        {
            if (!importedAssets[i].EndsWith(".tsv")) continue;

            string newPath = importedAssets[i].Replace(".tsv", ".csv");

            AssetDatabase.MoveAsset(importedAssets[i], newPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}
#endif