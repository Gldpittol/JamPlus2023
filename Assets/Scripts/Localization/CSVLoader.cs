using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.IO;
using UnityEditor;
public class CSVLoader
{
    private TextAsset csvFile;
    private char lineSeparator = '\n';
    private char surround = '"';
    private string[] fieldSeparator = { "," };
 
    public void LoadCSV()
    {
        csvFile = Resources.Load<TextAsset>("Localization");
    }

    /// <summary>
    /// Splits the string using \n which is at end of line and ignores the \n which are inside quotes. So we can support multi-line csv fields.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string[] GetLines(string text)
    {
        // This is matching \n only if it is outside double quotes. For regex is doing a look ahead for even number of quotes after \n.
        return Regex.Split(text, "(?=(?:(?:[^\"]*\"){2})*[^\"]*$)\\n");
    }

    public Dictionary<string, string> GetDictionaryValues(string attributeId)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        string[] lines = GetLines(csvFile.text);
        int attributeIndex = -1;

        string[] headers = lines[0].Split(fieldSeparator, StringSplitOptions.None);

        for(int i = 0; i < headers.Length; i++)
        {
            if(headers[i].Contains(attributeId))
            {
                attributeIndex = i;
                break;
            }
        }

        Regex csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        for(int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = csvParser.Split(line);

            for (int j = 0; j < fields.Length; j++)
            {
                fields[j] = fields[j].TrimStart(' ', surround);
                fields[j] = fields[j].TrimEnd(surround);

            }

            if(fields.Length > attributeIndex)
            {
                var key = fields[0];

                if (dictionary.ContainsKey(key)) continue;

                var value = fields[attributeIndex];
                
                value = value.TrimEnd((char)13);
                value = value.TrimEnd((char)34);

                dictionary.Add(key, value);
            }
        }

        return dictionary;
    }

#if UNITY_EDITOR

    public void Add(string key, string value)
    {
        string appended = "\n" + "\"" + key + "\"," + value;
        //string appended = string.Format("\n\"{0}\",\"{1}\",\"\"", key, value);

        File.AppendAllText("Assets/Resources/Localization.csv", appended);

        AssetDatabase.Refresh();
        LocalizationSystem.Init();
    }

    public void Remove(string key)
    {
        string[] lines = GetLines(csvFile.text);
        string[] keys = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            
            keys[i] = line.Split(fieldSeparator, StringSplitOptions.None)[0];
        }

        int index = -1;

        for (int i = 0; i < keys.Length; i++)
        {
            if(keys[i].Contains(key))
            {
                index = i;
                break;
            }
        }

        if(index > -1)
        {
            string[] newLines;
            newLines = lines.Where(w => w != lines[index]).ToArray();
            string replaced = string.Join(lineSeparator.ToString(), newLines);
            File.WriteAllText("Assets/Resources/Localization.csv", replaced);
        }

        AssetDatabase.Refresh();
        LocalizationSystem.Init();
    }

    public void Edit(string key, string value)
    {
        string appended = "\"" + key + "\"," + value;

        string[] lines = GetLines(csvFile.text);
        string[] keys = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            keys[i] = line.Split(fieldSeparator, StringSplitOptions.None)[0];
        }

        int index = -1;

        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i].Contains(key))
            {
                index = i;
                break;
            }
        }

        if (index > -1)
        {
            lines[index] = appended;
            string[] newLines;
            newLines = lines.ToArray();
            string replaced = string.Join(lineSeparator.ToString(), newLines);
            File.WriteAllText("Assets/Resources/Localization.csv", replaced);
        }

        AssetDatabase.Refresh();
        LocalizationSystem.Init();

        //Remove(key);
        //Add(key, value);
    }
#endif
}
