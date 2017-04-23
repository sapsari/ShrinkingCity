using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour {

    public enum Type
    {
        Residental,
        Commercial,
        Industrial,
        Infrastructural,
    };

    public Type type;

    float taxTime { get { return 10f * Mathf.Pow(0.9f, industrialLevel - 1); } }
    float growTime { get { return 1f * Mathf.Pow(0.9f, residentalLevel - 1); } }

    Finance finance;
    City city;

    public float population { get; private set; }
    public int populationCap { get { return GetPopulationCapAtLevel(Level); } }

    public static int GetPopulationCapAtLevel(int level)
    {
        return 100 * level;
    }

    Text text;

    internal static int residentalCount, commercialCount, industrialCount, infrastructuralCount;
    internal static int residentalLevel = 1, commercialLevel = 1, industrialLevel = 1, infrastructuralLevel = 1;

    internal static int getLevelOf(Type type)
    {
        switch (type)
        {
            case Type.Residental:
                return residentalLevel;
            case Type.Commercial:
                return commercialLevel;
            case Type.Industrial:
                return industrialLevel;
            case Type.Infrastructural:
                return infrastructuralLevel;
            default:
                return 1;
        }
    }

    int Level
    {
        get
        {
            return getLevelOf(type);            
        }
    }

	// Use this for initialization
	void Start () {
        finance = GameObject.FindObjectOfType<Finance>();
        city = GameObject.FindObjectOfType<City>();
        text = GameObject.Find("Footer").GetComponentInChildren<Text>();

        StartCoroutine(GrowPopulation());
        StartCoroutine(CollectTax());
    }
	
	// Update is called once per frame
	void Update () {
	
        if (isMouseOver)
        {
            text.text = name + " " + GetPopulationText(type) + ":" + (int)population + "/" + populationCap;
        }
    }

    public static string GetPopulationText(Type type)
    {
        switch (type)
        {
            case Type.Residental:
                return "Population";
            case Type.Commercial:
                return "Customer";
            case Type.Industrial:
                return "Employee";
            default:
                return "";
        }
    }


    float GetTaxAmount()
    {
        return population * Mathf.Pow(1.1f, commercialLevel - 1);
    }

    public float GetTaxRate()
    {
        return GetTaxAmount() / taxTime;
    }

    IEnumerator CollectTax()
    {
        while (true)
        {
            yield return new WaitForSeconds(taxTime);

            finance.Money += (int)GetTaxAmount();
        }
    }


    float GetGrowAmount()
    {
        return 1f * city.GetBuildingBooster(type) * Mathf.Pow(1.1f, residentalLevel);
    }

    public float GetGrowRate()
    {
        return 1f / growTime * GetGrowAmount();
    }

    IEnumerator GrowPopulation()
    {
        while(true)
        {
            yield return new WaitForSeconds(growTime);

            if ((int)population > populationCap)
            {
                population -= 1;
            }
            else
            {
                population += GetGrowAmount();
                population = Mathf.Min(population, populationCap);
            }

        }
    }

    bool isMouseOver;
    void OnMouseEnter()
    {
        isMouseOver = true;
        //text.text = name + " Population:" + population;


        /*
        zone
        cur pop
        pop grow rate
        pop cap
        next tax in n secs
        total tax
        */
    }

    void OnMouseExit()
    {
        isMouseOver = false;
        text.text = "";
    }
}
