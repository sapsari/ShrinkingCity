using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    bool isName;
    bool isMoney;
    Building.Type type;
    bool isUpgrade;

    Text text;

    Finance finance;
    City city;

	// Use this for initialization
	void Start () {

        finance = GameObject.FindObjectOfType<Finance>();
        city = GameObject.FindObjectOfType<City>();
        text = GameObject.Find("Footer").GetComponentInChildren<Text>();

        isMoney = (gameObject.name == "Money");
        isName = (gameObject.name == "City Name");

        if (!isMoney && !isName)
        {
            //type = gameObject.name.Contains("Residental")
            var types = Enum.GetValues(typeof(Building.Type));
            foreach (var t in types)
            {
                if (gameObject.name.Contains(t.ToString()))
                {
                    this.type = (Building.Type)t;
                    break;
                }
            }
            isUpgrade = gameObject.name.Contains("Upgrade");
        }
    }

    // Update is called once per frame
    void Update () {

        if (!isMouseOver) return;

        string str;
        if (isName)
        {
            str = city.Name + ", Population: " + city.GetPopulation() + "/" + city.GetPopulationCap() + ", Grow rate: " + city.GetGrowRate() + " per second";
        }
        else if (isMoney)
        {
            var residentalF = city.GetTaxRateOf(Building.Type.Residental);
            var commercialF = city.GetTaxRateOf(Building.Type.Commercial);
            var industrialF = city.GetTaxRateOf(Building.Type.Industrial);

            var residental = (residentalF >= 1 ? ((int)residentalF).ToString() : (residentalF.ToString("0.0")));
            var commercial = (commercialF >= 1 ? ((int)commercialF).ToString() : (commercialF.ToString("0.0")));
            var industrial = (industrialF >= 1 ? ((int)industrialF).ToString() : (industrialF.ToString("0.0")));

            var total = (int)(residentalF + commercialF + industrialF);

            str = "Taxes: $" + total + " per second ($" + residental + " from Residental, $" + commercial + " from Commercial, $" + industrial + " from Industrial)";
        }
        else
        {
            str = "Cost: " + (isUpgrade ? finance.GetUpgradeCost(type) : finance.GetBuildCost(type));

            if (isUpgrade)
            {
                if (type != Building.Type.Infrastructural)
                    //str += " , Increases Max " + Building.GetPopulationText(type) + " to " + Building.GetPopulationCapAtLevel(Building.getLevelOf(type) + 1) + " per " + type.ToString();
                    str += " , Increases " + Building.GetPopulationText(type) + " Capacity";// to " + Building.GetPopulationCapAtLevel(Building.getLevelOf(type) + 1);
                else
                    str += " , Increases Floor Capacity";

                switch (type)
                {
                    case Building.Type.Residental:
                        str += " , Increases Grow Amount";
                        break;
                    case Building.Type.Commercial:
                        str += " , Increases Tax Amount";
                        break;
                    case Building.Type.Industrial:
                        str += " , Increases Tax Rate";
                        break;
                    case Building.Type.Infrastructural:
                        str += " , Increases Grow Rate";
                        break;
                }
            }
        }

        text.text = str;
    }

    bool isMouseOver;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        text.text = "";
    }


}
