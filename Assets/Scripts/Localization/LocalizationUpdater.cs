using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sirenix.OdinInspector;

public class LocalizationUpdater : MonoBehaviour
{
    public Language searchLanguage;

#if UNITY_EDITOR
   // [Button("Update")]
    public void UpdateTexts()
    {
        LocalizationSystem.searchLanguage = searchLanguage;
        UnityEditor.AssetDatabase.Refresh();
        LocalizationSystem.Init();
    }
#endif
}
