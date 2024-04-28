using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinCanvas : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private Sprite spriteBronze;
    [SerializeField] private Sprite spriteSilver;
    [SerializeField] private Sprite spriteGold;
    [SerializeField] private float goldThreshold;
    [SerializeField] private float silverThreshold;
    [SerializeField] private float bronzeThreshold;
    [SerializeField] private TextMeshProUGUI achText;

    private void Start()
    {
        GetStarsAmount();
    }

    public void GetStarsAmount()
    {
        float percentage = PlayerDataManager.Instance.GetCompletionPercentage();
        
        var tempList = new List<object>();
        tempList.Add(percentage);
        
        achText.GetComponent<TextLocalizerUI>().UpdateParameters(tempList); 

        if (percentage >= goldThreshold)
        {
            img.sprite = spriteGold;
            return;
        }
        
        if (percentage >= silverThreshold)
        {
            img.sprite = spriteSilver;
            return;
        }
        
        if (percentage >= bronzeThreshold)
        {
            img.sprite = spriteBronze;
            return;
        }
    }

}
