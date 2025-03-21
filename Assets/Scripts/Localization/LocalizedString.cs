using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LocalizedString 
{
    public string key;
    public LocalizedString(string _key)
    {
        key = _key;
    }

    public string value
    {
        get
        {
            return LocalizationSystem.GetLocalizedValue(key);
        }
    }

    public static implicit operator LocalizedString (string key)
    {
        return new LocalizedString(key);
    }
}
