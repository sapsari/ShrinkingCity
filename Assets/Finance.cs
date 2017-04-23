using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Finance : MonoBehaviour {

    public int InitialMoney;

    Text moneyText;
    City city;

    Button residental, commercial, industrial, infrastructural;
    Button residentalUpgrade, commercialUpgrade, industrialUpgrade, infrastructuralUpgrade;

    int _money;
    public int Money
    {
        get { return _money; }
        set { SetMoney(value); }
    }

	// Use this for initialization
	void Start () {

        city = GameObject.FindObjectOfType<City>();
        moneyText = GameObject.Find("Money").GetComponent<Text>();

        residental = GameObject.Find("Button Residental").GetComponent<Button>();
        commercial = GameObject.Find("Button Commercial").GetComponent<Button>();
        industrial = GameObject.Find("Button Industrial").GetComponent<Button>();
        infrastructural = GameObject.Find("Button Infrastructural").GetComponent<Button>();

        residentalUpgrade = GameObject.Find("Button Residental Upgrade").GetComponent<Button>();
        commercialUpgrade = GameObject.Find("Button Commercial Upgrade").GetComponent<Button>();
        industrialUpgrade = GameObject.Find("Button Industrial Upgrade").GetComponent<Button>();
        infrastructuralUpgrade = GameObject.Find("Button Infrastructural Upgrade").GetComponent<Button>();

        SetMoney(InitialMoney);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetMoney(int money)
    {
        this._money = money;

        moneyText.text = "$ " + Money;


        residental.interactable = (money >= GetBuildCost(Building.Type.Residental));
        commercial.interactable = (money >= GetBuildCost(Building.Type.Commercial));
        industrial.interactable = (money >= GetBuildCost(Building.Type.Industrial));
        infrastructural.interactable = false && (money >= GetBuildCost(Building.Type.Infrastructural));

        residentalUpgrade.interactable = (money >= GetUpgradeCost(Building.Type.Residental)) && city.IsUpgradeable(Building.Type.Residental);
        commercialUpgrade.interactable = (money >= GetUpgradeCost(Building.Type.Commercial)) && city.IsUpgradeable(Building.Type.Commercial);
        industrialUpgrade.interactable = (money >= GetUpgradeCost(Building.Type.Industrial)) && city.IsUpgradeable(Building.Type.Industrial);
        infrastructuralUpgrade.interactable = (money >= GetUpgradeCost(Building.Type.Infrastructural)) && city.IsUpgradeable(Building.Type.Infrastructural);


    }

    public int GetUpgradeCost(Building.Type type)
    {
        Func<int, int> getCost = (level) =>
         {
             //return ((level - 1) * 2 + 1) * 1000;
             return (int)Mathf.Pow(2, level) * 1000;
         };

        switch (type)
        {
            case Building.Type.Residental:
                return getCost(Building.residentalLevel);
            case Building.Type.Commercial:
                return getCost(Building.commercialLevel);
            case Building.Type.Industrial:
                return getCost(Building.industrialLevel);
            case Building.Type.Infrastructural:
                //return (Building.infrastructuralLevel + 1) * 1000;
                return (int)Mathf.Pow(4, Building.infrastructuralLevel) * 1000;
            default:
                return 1000;
        }
    }

    public int GetBuildCost(Building.Type type)
    {
        return 1000;
    }

}
