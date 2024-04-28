using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using Sirenix.OdinInspector;

[RequireComponent(typeof(TMP_Text))]
[RequireComponent(typeof(LocalizationUpdater))]
public class TextLocalizerUI : MonoBehaviour
{
    private TMP_Text textField;

    [SerializeField] private LocalizedString localizedString;
    [SerializeField] List<object> parametersList = new List<object>();

    private void Awake()
    {
        GameManager.Instance.OnUpdateLanguage += UpdateText;
    }

    private void Start()
    {
        UpdateText();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnUpdateLanguage -= UpdateText;
    }

    public string GetLocalizedValue()
    {
        return localizedString.value;
    }

    public void UpdateText(string newLocalizedString)
    {
        localizedString = newLocalizedString;
        UpdateText();
    }

    public void UpdateText()
    {
        if(!textField )textField = GetComponent<TMP_Text>();
        
        if (parametersList.Count != 0)
        {
            textField.text = string.Format(localizedString.value, parametersList.ToArray());
        }
        else
        {
            textField.text = localizedString.value;
        }
    }

    public void UpdateParameters(List<object> _parametersList)
    {
        parametersList = new List<object>(_parametersList);
        UpdateText();
    }

    public void UpdateSpecificParameter(object objectToEdit, int objectIndex)
    {
        if(parametersList == null)
        {
            Debug.LogWarning("Error, List not Initialized");
            return;
        }

        if(objectIndex >= parametersList.Count)
        {
            Debug.LogWarning("Error, Index Out of Range");
            return;
        }

        parametersList[objectIndex] = objectToEdit;
    }

    public void SetNewKey(string newKey)
    {
        localizedString = new LocalizedString(newKey);
        UpdateText();
    }
}
