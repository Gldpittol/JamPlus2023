using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ComboBar : MonoBehaviour
{
    public static ComboBar Instance;
    [SerializeField] private float delayBeforeDecreasing;
    [SerializeField] private float incrementOnJump;
    [SerializeField] private float decrementPerFrame;
    [SerializeField] private float tweenDuration = 0.3f;
    [SerializeField] private float comboTextYIncrease = 1f;
    [SerializeField] private float comboTextTweenDuration = 1f;

    [SerializeField] private Ease tweenEase;

    [SerializeField] private SpriteRenderer barFill;
    [SerializeField] private TextMeshPro comboText;
    [FormerlySerializedAs("thresholdList")] [SerializeField] private List<float> tresholdList = new List<float>();
    [SerializeField] private List<float> multiplierList = new List<float>();
    [SerializeField] private List<Color> colorList = new List<Color>();
    [SerializeField] private Color defaultColor;
    [SerializeField] private GameObject comboTextPrefab;

    
    [Header("VFX")]
    [SerializeField] private List<GameObject> vfxList = new List<GameObject>();

    [SerializeField] private GameObject vfxDefault;


    private float originalY;
    private float currentDelay;

    private void Awake()
    {
        Instance = this;
        
        originalY = barFill.transform.localScale.y;
        barFill.transform.localScale = new Vector2(barFill.transform.localScale.x, 0);
    }

    private void Start()
    {
        //comboText.transform.parent = Coin.Instance.transform;
        //comboText.transform.localPosition = Vector2.zero;
        comboText.transform.parent = null;
        comboText.DOFade(0, 0);
    }
    
    private void Update()
    {
        currentDelay -= Time.deltaTime;
        CheckIfDecreasing();
        UpdateColor();
    }

    public void ResetDelay()
    {
        currentDelay = delayBeforeDecreasing;
    }

    public void Increment()
    {
        barFill.transform.DOKill();
        float currentPercentage = barFill.transform.localScale.y / originalY;
        currentPercentage += incrementOnJump / 100;
        if (currentPercentage > 1) currentPercentage = 1;

        barFill.transform.DOScale(new Vector2(barFill.transform.localScale.x, originalY * currentPercentage),
            tweenDuration);
        //barFill.transform.localScale = new Vector2(barFill.transform.localScale.x, originalY * currentPercentage);
    }

    public void CheckIfDecreasing()
    {
        if (currentDelay <= 0)
        {
            float currentPercentage = barFill.transform.localScale.y / originalY;
            currentPercentage -= decrementPerFrame / 100 * Time.deltaTime;
            if(currentPercentage < 0) currentPercentage = 0;
            barFill.transform.localScale = new Vector2(barFill.transform.localScale.x, originalY * currentPercentage);
        }
    }

    public float GetComboMultiplier()
    {
        if (multiplierList.Count < 0) return 1;
        if (tresholdList.Count < 0) return 1;
        if (multiplierList.Count != tresholdList.Count)
        {
            print("Different sized lists");
            return 1;
        }

        for (int i = 0; i < tresholdList.Count; i++)
        {
            float currentPercentage = barFill.transform.localScale.y / originalY * 100;
            if (currentPercentage > tresholdList[i])
            {
                return multiplierList[i];
            }
        }

        return 1;
    }

    public int GetListID(float multiplier)
    {
        return multiplierList.IndexOf(multiplier);
    }

    public void UpdateColor()
    {
        for (int i = 0; i < tresholdList.Count; i++)
        {
            float currentPercentage = barFill.transform.localScale.y / originalY * 100;
            if (currentPercentage > tresholdList[i])
            {
                barFill.color = new Color(colorList[i].r, colorList[i].g, colorList[i].b, barFill.color.a);
                comboText.color = new Color(colorList[i].r, colorList[i].g, colorList[i].b, comboText.color.a);

                comboText.text = GetComboMultiplier().ToString("F0") + "X";
                return;
            }
        }

        barFill.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, barFill.color.a);
        comboText.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, comboText.color.a);
        comboText.text = GetComboMultiplier().ToString("F0") + "X";
    }

    public float GetPercentage()
    {
        return barFill.transform.localScale.y / originalY;
    }

    public void DoComboText()
    {
        for (int i = 0; i < tresholdList.Count; i++)
        {
            float currentPercentage = barFill.transform.localScale.y / originalY * 100;
            if (currentPercentage > tresholdList[i])
            {
                GameObject tempVFX = Instantiate(vfxList[i], Coin.Instance.transform.position, Quaternion.identity);
                tempVFX.transform.eulerAngles = vfxList[i].transform.eulerAngles;
                Destroy(tempVFX, 1f);
                return;
            }
        }        
        GameObject tempVFX2 = Instantiate(vfxDefault, Coin.Instance.transform.position, Quaternion.identity);
        tempVFX2.transform.eulerAngles = vfxDefault.transform.eulerAngles;
        Destroy(tempVFX2, 1f);

        /*TextMeshPro tempComboText = Instantiate(comboTextPrefab).GetComponent<TextMeshPro>();
        tempComboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 1);
        tempComboText.text = comboText.text;
        tempComboText.DOFade(0, comboTextTweenDuration).OnComplete(()=>
            tempComboText.color = new Color(tempComboText.color.r, tempComboText.color.g, tempComboText.color.b, 0));

        tempComboText.transform.position = Coin.Instance.transform.position;
        tempComboText.transform.DOMoveY(tempComboText.transform.position.y + comboTextYIncrease,comboTextTweenDuration);*/
    }
}
