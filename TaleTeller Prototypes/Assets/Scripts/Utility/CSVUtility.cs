using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CSVUtility
{
    public static List<string[]> ParseCSV(TextAsset csvFile)
    {
        string rawContent = csvFile.text;

        char[] separators = new char[] { '\n' };
        string[] lines = rawContent.Split(separators, StringSplitOptions.None);

        List<string[]> result = new List<string[]>();

        string[] cellSeparator = new string[] { "\t" };
        for (int i = 1; i < lines.Length; i++)
        {
            string[] cells = lines[i].Split(cellSeparator, StringSplitOptions.None);
            result.Add(cells);
        }
        return result;
    }
}
