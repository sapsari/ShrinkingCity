//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour {

    public string Name;
    public int InitialZoneCount;


    int ZoneCount;

    Polygon polygon;

    List<List<Building>> buildings;

    Finance finance;

    Text nameText;
    Text populationText;
    Text headerText;

    int floorCap { get { return Building.infrastructuralLevel; } }

    int year = 1;

    int score;

    int lastLoss;

	// Use this for initialization
	void Start () {

        finance = gameObject.GetComponent<Finance>();

        nameText = GameObject.Find("City Name").GetComponent<Text>();
        populationText = GameObject.Find("Population").GetComponent<Text>();
        headerText = GameObject.Find("Header").GetComponentInChildren<Text>();

        ZoneCount = InitialZoneCount;
        polygon = GameObject.FindObjectOfType<Polygon>();
        polygon.SetN(ZoneCount);

        buildings = new List<List<Building>>();
        for (var i =0;i<ZoneCount;i++)
        {
            buildings.Add(new List<Building>());
        }

        //StartCoroutine(Shrinker());
	}

    public void Reset()
    {
        StopCoroutine(Shrinker());

        while(ZoneCount > 0)
        {
            ShrinkAux(0);
        }
        
        year = 1;
        score = 0;
        ZoneCount = InitialZoneCount;
        polygon.SetN(ZoneCount);

        for (var i = 0; i < ZoneCount; i++)
        {
            buildings.Add(new List<Building>());
        }

        finance.Money = finance.InitialMoney;

        StartCoroutine(Shrinker());
    }

    // Update is called once per frame
    void Update () {

        float rotationMultiplier = -1f * Mathf.Pow(1.12f, year);
        this.transform.Rotate(0, 0, Time.deltaTime * rotationMultiplier);

        UpdatePopulation();

        HandleMouse();

        return;
        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            Reset();
        }
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            Shrink(0);
            year++;
            Debug.Log("year " + year);
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Build(0, Building.Type.Residental);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Build(1, Building.Type.Residental);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Build(2, Building.Type.Residental);
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            GG();
        }
    }

    void HandleMouse()
    {
        return;
        if (Input.GetMouseButtonDown(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var zoneNo = polygon.GetEdgeNo(pos);
            Debug.Log("zone " + zoneNo + " pos " + pos);

            Build(zoneNo, Building.Type.Commercial);
        }
    }

    void Build(Building.Type type)
    {
        var availableZones = GetAvailableZones();
        //Debug.Log("available zone count " + availableZones.Count());
        if (availableZones.Any())
        {
            var randomZone = availableZones.ElementAt(Random.Range(0, availableZones.Count()));
            Build(randomZone, type);
        }
    }
    public void BuildResidental()
    {
        Build(Building.Type.Residental);
    }
    public void BuildCommercial()
    {
        Build(Building.Type.Commercial);
    }
    public void BuildIndustrial()
    {
        Build(Building.Type.Industrial);
    }
    public void BuildInfrastructural()
    {
        Build(Building.Type.Infrastructural);
    }

    public void Build(int zoneNo, Building.Type type)
    {
        Debug.Assert(zoneNo < ZoneCount);

        var floor = buildings[zoneNo].Count;
        var pos = polygon.GetEdgePosition(zoneNo);
        var rot = polygon.GetEdgeAngle(zoneNo);

        const float floorHeight = 0.9f;
        var height = (floor + 0.5f)* floorHeight;
        pos += new Vector3(Mathf.Cos(rot) * height, Mathf.Sin(rot) * height);

        rot = rot / Mathf.Deg2Rad;
        rot += 90;

        var typeName = type.ToString();
        var name = typeName;

        switch (type)
        {
            case Building.Type.Residental:
                name += (++Building.residentalCount).ToString();
                break;
            case Building.Type.Commercial:
                name += (++Building.commercialCount).ToString();
                break;
            case Building.Type.Industrial:
                name += (++Building.industrialCount).ToString();
                break;
            case Building.Type.Infrastructural:
                name += (++Building.infrastructuralCount).ToString();
                break;
            default:
                break;
        }

        var obj = new GameObject(name);
        obj.transform.parent = this.transform;
        obj.transform.localPosition = pos;
        obj.transform.localRotation = Quaternion.Euler(0, 0, rot);
        obj.transform.localScale = Vector3.one * 0.7f;

        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>(typeName);

        var collider = obj.AddComponent<BoxCollider2D>();

        var building = obj.AddComponent<Building>();
        building.type = type;

        buildings[zoneNo].Add(building);

        finance.Money -= finance.GetBuildCost(type);
    }

    public void Shrink(int zoneNo)
    {
        ShrinkAux(zoneNo);

        polygon.SetN(buildings.Count);

        UpdateBuildingPositions();

        var end = Camera.main.WorldToScreenPoint(new Vector3(polygon.radius * 2, 0));
        var begin = Camera.main.WorldToScreenPoint(Vector3.zero);
        var width = (end.x - begin.x) * 0.9f;
        nameText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        populationText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 0.8f);
    }

    private void ShrinkAux(int zoneNo)
    {
        lastLoss = (int)buildings[zoneNo].Where(b => b.type == Building.Type.Residental).Sum(b => b.population);

        foreach (var building in buildings[zoneNo])
        {
            building.transform.parent = null;
            Destroy(building.gameObject);
        }

        buildings[zoneNo].Clear();
        buildings.RemoveAt(zoneNo);

        ZoneCount--;
    }

    void UpdateBuildingPositions()
    {
        for (var zoneNo = 0; zoneNo < buildings.Count; zoneNo++)
        {
            var buildingList = buildings[zoneNo];
            for (var floorNo = 0; floorNo < buildingList.Count; floorNo++)
            {
                var building = buildingList[floorNo];

                var floor = floorNo;
                var pos = polygon.GetEdgePosition(zoneNo);
                var rot = polygon.GetEdgeAngle(zoneNo);

                const float floorHeight = 0.9f;
                var height = (floor + 0.5f) * floorHeight;
                pos += new Vector3(Mathf.Cos(rot) * height, Mathf.Sin(rot) * height);

                rot = rot / Mathf.Deg2Rad;
                rot += 90;

                var obj = building.gameObject;
                obj.transform.localPosition = pos;
                obj.transform.localRotation = Quaternion.Euler(0, 0, rot);
                //obj.transform.localScale = Vector3.one * 0.7f;
            }
        }
    }

    public int GetPopulation()
    {
        return buildings.Sum(bl => bl.Where(b => b.type == Building.Type.Residental).Sum(b => (int)b.population));
    }
    public int GetPopulationCap()
    {
        return buildings.Sum(bl => bl.Where(b => b.type == Building.Type.Residental).Sum(b => (int)b.populationCap));
    }
    public int GetGrowRate()
    {
        return (int)buildings.Sum(bl => bl.Where(b => b.type == Building.Type.Residental).Sum(b => b.GetGrowRate()));
    }
    public float GetTaxRateOf(Building.Type type)
    {
        return buildings.Sum(bl => bl.Where(b => b.type == type).Sum(b => b.GetTaxRate()));
    }

    void UpdatePopulation()
    {
        var population = GetPopulation();
        populationText.text = "P " + population;
        populationText.text = "";
        nameText.text = Name + "\nPopulation " + population;

        score = Mathf.Max(score, population);
    }


    public void UpgradeResidental()
    {
        var cost = finance.GetUpgradeCost(Building.Type.Residental);
        Building.residentalLevel++;
        finance.Money -= cost;
    }
    public void UpgradeCommercial()
    {
        var cost = finance.GetUpgradeCost(Building.Type.Commercial);
        Building.commercialLevel++;
        finance.Money -= cost;
    }
    public void UpgradeIndustrial()
    {
        var cost = finance.GetUpgradeCost(Building.Type.Industrial);
        Building.industrialLevel++;
        finance.Money -= cost;
    }
    public void UpgradeInfrastructural()
    {
        var cost = finance.GetUpgradeCost(Building.Type.Infrastructural);
        Building.infrastructuralLevel++;
        finance.Money -= cost;
    }

    IEnumerable<int>GetAvailableZones()
    {
        for(var zoneNo=0;zoneNo<ZoneCount;zoneNo++)
        {
            if (buildings[zoneNo].Count < floorCap)
                yield return zoneNo;
        }
    }


    public float GetBuildingBooster(Building.Type type)
    {
        var numResidental = buildings.Sum(bl => bl.Count(b => b.type == Building.Type.Residental));
        var numCommercial = buildings.Sum(bl => bl.Count(b => b.type == Building.Type.Commercial));
        var numIndustrial = buildings.Sum(bl => bl.Count(b => b.type == Building.Type.Industrial));

        var boost = 1f;
        switch (type)
        {
            case Building.Type.Residental:
                boost += GetBuildingBoost(numResidental, numCommercial) * GetLevelHandicap(Building.residentalLevel, Building.commercialLevel);
                boost += GetBuildingBoost(numResidental, numIndustrial) * GetLevelHandicap(Building.residentalLevel, Building.industrialLevel);
                break;
            case Building.Type.Commercial:
                boost += GetBuildingBoost(numCommercial, numResidental) * GetLevelHandicap(Building.commercialLevel, Building.residentalLevel);
                boost += GetBuildingBoost(numCommercial, numIndustrial) * GetLevelHandicap(Building.commercialLevel, Building.industrialLevel);
                break;
            case Building.Type.Industrial:
                boost += GetBuildingBoost(numIndustrial, numResidental) * GetLevelHandicap(Building.industrialLevel, Building.residentalLevel);
                boost += GetBuildingBoost(numIndustrial, numCommercial) * GetLevelHandicap(Building.industrialLevel, Building.commercialLevel);
                break;
            default:
                break;
        }

        return boost;
    }

    private static float GetBuildingBoost(int numPrimary, int numSecondary)
    {
        if (numPrimary >= numSecondary)
        {
            return 1f * numSecondary / numPrimary;
        }
        else
        {
            return 1f + (1f * (numSecondary - numPrimary) / numPrimary) * 0.5f;
        }
    }

    private static float GetLevelHandicap(int primaryLevel, int secondaryLevel)
    {
        if (secondaryLevel >= primaryLevel)
            return 1f;
        else
            return Mathf.Pow(0.8f, primaryLevel - secondaryLevel);
    }

    public bool IsUpgradeable(Building.Type type)
    {
        if (type == Building.Type.Infrastructural)
        {
            var count1 = buildings.Sum(bl => bl.Count(b => b.type == Building.Type.Residental));
            var count2 = buildings.Sum(bl => bl.Count(b => b.type == Building.Type.Commercial));
            var count3 = buildings.Sum(bl => bl.Count(b => b.type == Building.Type.Industrial));
            var count = Mathf.Min(count1, count2, count3);
            var level = Building.infrastructuralLevel;
            return count >= level;
        }
        else
        {
            var count = buildings.Sum(bl => bl.Count(b => b.type == type));
            var level = Building.getLevelOf(type);
            return count >= level;
        }
    }

    IEnumerator Shrinker()
    {
        while (true)
        {
            System.Func<string, string> getText = (timeIndicator) =>
            {
                var str = "Year " + year + ", Next Shrink in less than a " + timeIndicator;
                if (year > 1)
                {
                    if (lastLoss == 0)
                        str += ", No Loss in the last Shrink";
                    else
                        str += ", " + lastLoss + " Lossed in the last Shrink";
                }
                return str;
            };

            // make it faster each passing year
            var time = 62 - year - Random.value * year;

            time -= 30;
            headerText.text = getText("year");
            if (time >= 0)
                yield return new WaitForSeconds(time);

            headerText.text = getText("season");
            yield return new WaitForSeconds(20);

            headerText.text = getText("week");
            yield return new WaitForSeconds(9);

            headerText.text = getText("day");
            yield return new WaitForSeconds(1);

            int zoneNo;
            // ensure minimal loss for player for the first 3 years
            if (year <= 3)
            {
                var min = buildings.Min(bl => bl.Count);
                var list = buildings.First(bl => bl.Count == min);
                zoneNo = buildings.IndexOf(list);
            }
            else
            {
                zoneNo = Random.Range(0, ZoneCount);
            }

            Shrink(zoneNo);
            year++;

            if (year == 17)
            {
                //gg
                GG();
                break;
            }
        }
    }

    GameObject menuCanvas;
    public void StartFromMenu()
    {
        var nameInput = GameObject.Find("Name Input").GetComponent<InputField>().text;
        if (nameInput == "") nameInput = "Detroit Rock City";
        this.Name = nameInput;
        menuCanvas = GameObject.Find("Canvas of Menu");
        menuCanvas.SetActive(false);
        Reset();
    }

    void GG()
    {
        menuCanvas.SetActive(true);

        var highscore = GameObject.Find("Highscore").GetComponent<Text>();
        highscore.text = "Highscore of " + this.Name + " is " + score;
    }
}

/*
to do
let buildings fly away when they got shrinked
arrow for showing available slots for buildings when build is clicked (double click builds at a random zone)
name zones, no numbers
high score = population
show tax on building when its collected
triangle buttons on corners, so screen can be rectangle, show money inside the world too
initial/high score screen, name input
shrink info, how many ppl, how many buildings etc
*/

    
/*
our planet has shrinked. we come to this small world. we should host as many people as possible
*/