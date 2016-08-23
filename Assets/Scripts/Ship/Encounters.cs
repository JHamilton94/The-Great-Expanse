using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Encounters : MonoBehaviour {

    public Queue<Encounter> predictedEncounters;
    public Queue<Encounter> maneuverEncounters;

	// Use this for initialization
	void Start () {
        predictedEncounters = new Queue<Encounter>();
        maneuverEncounters = new Queue<Encounter>();
	}
}

public class Encounter
{
    public Encounter(GravityElementsClass gravElements, double startingTrueAnomaly, double endingTrueAnomaly, double timeOfEncounter)
    {
        this.gravElements = gravElements;
        this.startingTrueAnomaly = startingTrueAnomaly;
        this.endingTrueAnomaly = endingTrueAnomaly;
        this.timeOfEncounter = timeOfEncounter;
    }

    private GravityElementsClass gravElements;
    private double startingTrueAnomaly;
    private double endingTrueAnomaly;
    private double timeOfEncounter;


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
