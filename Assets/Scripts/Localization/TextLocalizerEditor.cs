using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class TextLocalizerEditWindow : EditorWindow
{
    public static TextLocalizerEditWindow window;
    public static void Open(string key)
    {
        if (window != null)
        {
            window.Close();
            window = null;
        }
        window = CreateInstance<TextLocalizerEditWindow>();
        window.titleContent = new GUIContent("Localizer Window");
        window.ShowUtility();
        window.key = key;

        if (!string.IsNullOrEmpty(LocalizationSystem.GetLocalizedValue(key)))
        {
            valueEN = LocalizationSystem.GetSpecificLocalizedValue(key, Language.English);
            valueBR = LocalizationSystem.GetSpecificLocalizedValue(key, Language.Portuguese);
            valueES = LocalizationSystem.GetSpecificLocalizedValue(key, Language.Spanish);
        }
        else
        {
            valueEN = "";
            valueBR = "";
            valueES = "";
        }
    }

    public string key;
    public string value;
    public static string valueEN;
    public static string valueBR;
    public static string valueES;

    public void OnGUI()
    {
        key = EditorGUILayout.TextField("Key :", key);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Text EN:", GUILayout.MaxWidth(100));
        EditorStyles.textArea.wordWrap = true;
        valueEN = EditorGUILayout.TextArea(valueEN, EditorStyles.textArea, GUILayout.Height(47), GUILayout.Width(350));

        EditorGUILayout.LabelField("Text BR:", GUILayout.MaxWidth(100));
        EditorStyles.textArea.wordWrap = true;
        valueBR = EditorGUILayout.TextArea(valueBR, EditorStyles.textArea, GUILayout.Height(47), GUILayout.Width(350));
        
        EditorGUILayout.LabelField("Text ES:", GUILayout.MaxWidth(100));
        EditorStyles.textArea.wordWrap = true;
        valueES = EditorGUILayout.TextArea(valueES, EditorStyles.textArea, GUILayout.Height(47), GUILayout.Width(350));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Add"))
        {
            value = " \"" + valueEN.Trim(' ') +"\",\"" + valueBR.Trim(' ') + "\", \"" + valueES.Trim(' ') + "\"";

            if (!string.IsNullOrEmpty(LocalizationSystem.GetLocalizedValue(key)))
            {
                if (EditorUtility.DisplayDialog("Warning", "Warning: Key \"" + key + "\" Already Exists. Overwrite Key?", "Yes"))
                {
                    LocalizationSystem.Replace(key, value);
                    if (EditorUtility.DisplayDialog("Success", "Key \"" + key + "\" Overwritten Successfully", "Close"))
                    {
                        window.Close();
                    }
                }
            }
            else
            {
                LocalizationSystem.Add(key, value);
                if (EditorUtility.DisplayDialog("Success", "Key \"" + key + "\" Added Successfully", "Close"))
                {
                    window.Close();
                }
            }
        }

        minSize = new Vector2(460, 250);
        maxSize = minSize;
    }
}

public class TextLocalizerSearchWindow :EditorWindow
{
    public static TextLocalizerSearchWindow window;
    public static void Open()
    {
        window = new TextLocalizerSearchWindow();
        window.titleContent = new GUIContent("Localization Search");
        window.ShowUtility();

        Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        Rect r = new Rect(mouse.x - 450, mouse.y + 10, 10, 10);
        window.ShowAsDropDown(r, new Vector2(500, 300));
    }

    public string value;
    public Vector2 scroll;
    public Dictionary<string,string> dictionary;

    private void OnEnable()
    {
        dictionary = LocalizationSystem.GetDictionaryForEditor();
    }

    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("Search: ", EditorStyles.boldLabel);
        value = EditorGUILayout.TextField(value);
        EditorGUILayout.EndHorizontal();

        GetSearchResults();
    }

    private void GetSearchResults()
    {
        if (value == null) value = "";

        EditorGUILayout.BeginVertical();
        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach(KeyValuePair<string, string> element in dictionary)
        {
            if(element.Key.ToLower().Contains(value.ToLower()) || element.Value.ToLower().Contains(value.ToLower()))
            {
                EditorGUILayout.BeginHorizontal("Box");
                Texture closeIcon = (Texture)Resources.Load("close");

                GUIContent content = new GUIContent(closeIcon);

                if (GUILayout.Button(content, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                {
                    if (EditorUtility.DisplayDialog("Remove Key " + element.Key + "?", "This will remove the Key \"" + element.Key + "\" from the Localization file, are you sure?", "Remove"))
                    {
                        LocalizationSystem.Remove(element.Key);
                        AssetDatabase.Refresh();
                        LocalizationSystem.Init();
                        dictionary = LocalizationSystem.GetDictionaryForEditor();

                        if (EditorUtility.DisplayDialog("Success", "Key \"" + element.Key + "\" Removed Successfully", "Close"))
                        {
                            window.Close();
                        }
                    }
                }

                Texture storeIcon = (Texture)Resources.Load("store");
                GUIContent storeContent = new GUIContent(storeIcon);

                if (GUILayout.Button(storeContent, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                {
                    TextLocalizerEditWindow.Open(element.Key);
                }

                EditorGUILayout.TextField(element.Key);
                EditorGUILayout.LabelField(element.Value);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

}

#endif