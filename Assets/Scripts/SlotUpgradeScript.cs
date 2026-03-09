using System;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUpgradeScript : MonoBehaviour , IPointerClickHandler
{
    public TextMeshProUGUI PriceTMP;
    public TextMeshProUGUI AmountUpgrade;
    public Image BlackScene;

    public ScalePunchUI _scalePunchUI;


    public int PriceAddLater;
    public int basePrice;
    public int price;
    public float growthRate;
    public int level;

    public UnityEvent OnUpgrade;

    public bool useBuyScene;

    private void Awake()
    {
<<<<<<< Updated upstream
        
=======
>>>>>>> Stashed changes
        price = basePrice;
        AmountUpgrade.text = "0";
        PriceTMP.text = GetPrice().ToString();
        AmountUpgrade.text = level.ToString();
<<<<<<< Updated upstream

        
=======
>>>>>>> Stashed changes
    }

    private void Start()
    {
        if (GM_LikesScore.instance != null)
        {
            GM_LikesScore.instance.OnLikesUpdated += OnCanUpgrade; //subscribe
        }
        else
        {
            Debug.LogWarning("ไม่พบ GM_LikesScore.instance");
        }

        if (PriceTMP == null && AmountUpgrade == null && _scalePunchUI == null)
        {
            Debug.Log("Error : ยังไม่ได้กำหนด");
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (GM_LikesScore.instance != null)
        {
            GM_LikesScore.instance.OnLikesUpdated -= OnCanUpgrade;
        }
    }

    private void OnCanUpgrade(int currentLikes)
    {

        if (BlackScene != null && !useBuyScene)
        {
            Color color = BlackScene.color;
            if (currentLikes >= GetPrice())
            {
                color.a = 0;
<<<<<<< Updated upstream
                
=======
>>>>>>> Stashed changes
            }
            else
            {
                color.a = 0.5f;
            }
            BlackScene.color = color;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GM_LikesScore.instance.getScoreLikes() >= price)
        {
            _scalePunchUI.Activate();
            //Do something
            UpLevel(1);
            OnUpgrade.Invoke();
        }
    }

    private void UpLevel(int amount)
    {
        level += amount;
        GM_LikesScore.instance.setScoreLikes(GM_LikesScore.instance.getScoreLikes() - price);
        AmountUpgrade.text = level.ToString();
        PriceTMP.text = GetPrice().ToString();

    }

    public int GetPrice()
    {
        double _price = PriceAddLater + (basePrice * Mathf.Pow(growthRate, level));
        price = Convert.ToInt32(_price);
        
        return price;
    }
}
