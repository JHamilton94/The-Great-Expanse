using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Encounters : MonoBehaviour {

    public List<Encounter> predictedEncounters;
    public List<Encounter> maneuverEncounters;

	// Use this for initialization
	void Start () {
        predictedEncounters = new List<Encounter>();
        maneuverEncounters = new List<Encounter>();
	}
}

public class Encounter
{
    public Encounter(GravityElementsClass gravElements, double startingTrueAnomaly, double endingTrueAnomaly, double timeOfEncounter, OrbitalPOI perigeeIcon)
    {
        this.gravElements = gravElements;
        this.startingTrueAnomaly = startingTrueAnomaly;
        this.endingTrueAnomaly = endingTrueAnomaly;
        this.timeOfEncounter = timeOfEncounter;
        this.perigeeIcon = perigeeIcon;
    }

    private OrbitalPOI perigeeIcon;
    private OrbitalPOI apogeeIcon;

    private GravityElementsClass gravElements;
    private double startingTrueAnomaly;
    private double endingTrueAnomaly;
    private double timeOfEncounter;


    public OrbitalPOI ApogeeIcon
    {
        get
        {
            return apogeeIcon;
        }

        set
        {
            apogeeIcon = value;
        }
    }
    public OrbitalPOI PerigeeIcon
    {
        get
        {
            return perigeeIcon;
        }

        set
        {
            perigeeIcon = value;
        }
    }

    public GravityElementsClass GravElements
    {
        get
        {
            return gravElements;
        }

        set
        {
            gravElements = value;
        }
    }

    public double TimeOfEncounter
    {
        get
        {
            return timeOfEncounter;
        }

        set
        {
            timeOfEncounter = value;
        }
    }

    public double StartingTrueAnomaly
    {
        get
        {
            return startingTrueAnomaly;
        }

        set
        {
            startingTrueAnomaly = value;
        }
    }
    public double EndingTrueAnomaly
    {
        get
        {
            return endingTrueAnomaly;
        }

        set
        {
            endingTrueAnomaly = value;
        }
    }


}
