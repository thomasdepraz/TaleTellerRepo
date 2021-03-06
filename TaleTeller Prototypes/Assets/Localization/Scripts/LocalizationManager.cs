using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : Singleton<LocalizationManager>
{
    private void Awake()
    {
        CreateSingleton();
    }

    [Header("Databases")]
    [SerializeField] private TextDataBase instructionDataBase;
    [SerializeField] private TextAsset schemesDatabase;
    [SerializeField] private TextAsset tooltipDatabase;
    [SerializeField] private TextAsset cardsDatabase;
    [SerializeField] private TextAsset popupDatabase;
    [SerializeField] private TextAsset screenDatabase;
    [SerializeField] private TextAsset tutorielDataBase;

    public Dictionary<string, string> instructionsDictionary;
    public Dictionary<string, string> schemesDescriptionsDictionary;
    public Dictionary<string, string> tooltipDictionary;
    public Dictionary<string, string> cardsDictionary;
    public Dictionary<string, string> popupDictionary;
    public Dictionary<string, string> screenDictionary;
    public Dictionary<string, string> tutorielDictionary;
    //Create as meany as needed


    private void Start()
    {
        instructionsDictionary = InitDictionary(instructionDataBase.database);
        schemesDescriptionsDictionary = InitDictionary(schemesDatabase);
        tooltipDictionary = InitDictionary(tooltipDatabase);
        cardsDictionary = InitDictionary(cardsDatabase);
        popupDictionary = InitDictionary(popupDatabase);
        screenDictionary = InitDictionary(screenDatabase);
        tutorielDictionary = InitDictionary(tutorielDataBase);
    }

    public Dictionary<string, string> InitDictionary(KeyStringPair[] pairs) ///Add methods overrides
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        for (int i = 0; i < pairs.Length; i++)
        {
            result.Add(pairs[i].key, pairs[i].text);
        }

        return result;
    }

    public Dictionary<string, string> InitDictionary(TextAsset textFile) ///Add methods overrides
    {
        List<string[]> list =  CSVUtility.ParseCSV(textFile);
        Dictionary<string, string> result = new Dictionary<string, string>();

        for (int i = 0; i < list.Count; i++)
        {
            result.Add(list[i][0], list[i][1]);
        }

        return result;
    }

    public string GetString(Dictionary<string , string> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            string result = dictionary[key];
            return result;
        }
        else
        {
            Debug.LogWarning(key + " is not a valid key");
            return key;
        }
    }
}
