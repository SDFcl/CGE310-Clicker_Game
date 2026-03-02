using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SlotUpgradeScript : MonoBehaviour , IPointerClickHandler
{
    public TextMeshProUGUI PriceTMP;
    public TextMeshProUGUI AmountUpgrade;

    public ScalePunchUI _scalePunchUI;


    public int PriceAddLater;
    public int basePrice;
    public int price;
    public float growthRate;
    public int level;

    public UnityEvent OnUpgrade;

    private void Awake()
    {
        if (PriceTMP == null && AmountUpgrade == null && _scalePunchUI == null)
        {
            Debug.Log("Error : ยังไม่ได้กำหนด");
            gameObject.SetActive(false);
        }
        price = basePrice;
        AmountUpgrade.text = "0";
        PriceTMP.text = GetPrice().ToString();
        AmountUpgrade.text = level.ToString();

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GM_LikesScore.instance.getScoreLikes() >= price)
        {
            _scalePunchUI.Activate();
            //Do something
            OnUpgrade.Invoke();
            UpLevel(1);
        }
    }

    private void UpLevel(int amount)
    {
        level += amount;
        GM_LikesScore.instance.setScoreLikes(GM_LikesScore.instance.getScoreLikes() - price);
        AmountUpgrade.text = level.ToString();
        PriceTMP.text = GetPrice().ToString();

    }

    private int GetPrice()
    {
        double _price = PriceAddLater + (basePrice * Mathf.Pow(growthRate, level));
        price = Convert.ToInt32(_price);
        
        return price;
    }
}
