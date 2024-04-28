using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationSystem
{
    public static Language language = Language.Portuguese;
    public static Language searchLanguage = Language.Portuguese;

    private static Dictionary<string, string> localizedEN;
    private static Dictionary<string, string> localizedBR;
    private static Dictionary<string, string> localizedES;

    public static bool isInit;

    public static CSVLoader csvLoader;

    public static void Init()
    {
        csvLoader = new CSVLoader();
        csvLoader.LoadCSV();

        UpdateDictionaries();

        isInit = true;
    }

    public static void UpdateDictionaries()
    {
        localizedEN = csvLoader.GetDictionaryValues("en");
        localizedBR = csvLoader.GetDictionaryValues("br");
        localizedES = csvLoader.GetDictionaryValues("es");
    }
    public static string GetLocalizedValue(string key)
    {
        if(!isInit) 
        {
            Init();  
        }

        string value = key;

        switch(language)
        {
            case Language.English:
                localizedEN.TryGetValue(key, out value);
                break;
            case Language.Portuguese:
                localizedBR.TryGetValue(key, out value);
                break;
            case Language.Spanish:
                localizedES.TryGetValue(key, out value);
                break;
        }

        return value;
    }

    public static string GetSpecificLocalizedValue(string key, Language specificLanguage)
    {
        if (!isInit)
        {
            Init();
        }

        string value = key;

        switch (specificLanguage)
        {
            case Language.English:
                localizedEN.TryGetValue(key, out value);
                break;
            case Language.Portuguese:
                localizedBR.TryGetValue(key, out value);
                break;
            case Language.Spanish:
                localizedES.TryGetValue(key, out value);
                break;
        }

        return value;
    }

    public static string GetAllValues(string key)
    {
        if (!isInit)
        {
            Init();
        }

        string value = key;
        string newValue = "";
        string totalValue = "";

        localizedEN.TryGetValue(key, out newValue);
        totalValue = "EN: " +  "\"" + totalValue + newValue + "\"" + "\n";

        totalValue += "\n";

        localizedBR.TryGetValue(key, out newValue);
        totalValue = totalValue + "PT: " + "\"" + newValue + "\"" + "\n";
        
        totalValue += "\n";

        localizedES.TryGetValue(key, out newValue);
        totalValue = totalValue + "ES: " + "\"" + newValue + "\"";

        return totalValue;
    }

#if UNITY_EDITOR

    public static void Add(string key, string value)
    {
        if(value.Contains("\""))
        {
            value.Replace('"', '\"');
        }

        if(csvLoader == null)
        {
            csvLoader = new CSVLoader();
        }

        csvLoader.LoadCSV();
        csvLoader.Add(key, value);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }

    public static void Replace(string key, string value)
    {
        if (value.Contains("\""))
        {
            value.Replace('"', '\"');
        }

        if (csvLoader == null)
        {
            csvLoader = new CSVLoader();
        }

        csvLoader.LoadCSV();
        csvLoader.Edit(key, value);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }

    public static void Remove(string key)
    {
        if (csvLoader == null)
        {
            csvLoader = new CSVLoader();
        }

        csvLoader.LoadCSV();
        csvLoader.Remove(key);
        csvLoader.LoadCSV();

        UpdateDictionaries();
    }

    public static Dictionary<string, string> GetDictionaryForEditor()
    {
        if (!isInit) Init();

        switch (searchLanguage)
        {
            case Language.English:
                return localizedEN;
            case Language.Portuguese:
                return localizedBR;
            case Language.Spanish:
                return localizedES;
        }

        return localizedEN;
    }
#endif
}

