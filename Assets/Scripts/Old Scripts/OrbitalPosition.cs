using UnityEngine;
using System.Collections;

public class OrbitalPosition {
    private Vector2 positionVector;
    private double trueAnomaly;

    public OrbitalPosition(Vector2 positionVector, double trueAnomaly)
    {
        this.positionVector = positionVector;
        this.trueAnomaly = trueAnomaly;
    }

    public Vector2 getPositionVector()
    {
        return positionVector;
    }

    public double getTrueAnoamly()
    {
        return trueAnomaly;
    }

    public void setPositionVector(Vector2 newPosition)
    {
        this.positionVector = newPosition;
    }

    public void setTrueAnomaly(double newAnomaly)
    {
        this.trueAnomaly = newAnomaly;
    }

}
