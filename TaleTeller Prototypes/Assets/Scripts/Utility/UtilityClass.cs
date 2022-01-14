using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UtilityClass
{
    public static List<Type> GetSubClasses(Type type)
    {
        List<Type> result = new List<Type>();
        Type[] temp = Assembly.GetAssembly(type).GetTypes();

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].IsSubclassOf(type) && temp[i].BaseType == type)//get only if direct child type (remove base type check if all childs needed)
            {
                result.Add(temp[i]);
            }
        }
        return result;
    }

    public static bool HasSubClasses(Type type)
    {
        Type[] temp = Assembly.GetAssembly(type).GetTypes();

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].IsSubclassOf(type))
            {
                return true;
            }
        }
        return false;
    }

    public static string ToBold(string text)
    {
        return $"<b>{text}</b>";
    }

    public static string ToColor(string text, string color)
    {
        return $"<Color={color}>{text}";
    }

    public static void InitCardList(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = list[i].InitializeData(list[i]);
        }
    }

    public static void ResetCardList(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = list[i].dataReference;
        }
    }
}
