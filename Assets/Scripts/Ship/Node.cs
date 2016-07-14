using UnityEngine;
using System.Collections;

public class Node {

    private double trueAnomaly;
    private Vector2 thrustVector;
    private Vector2 nodePosition;

    public Node(double trueAnomaly, Vector2 thrustVector, Vector2 nodePosition)
    {
        this.trueAnomaly = trueAnomaly;
        this.thrustVector = thrustVector;
        this.nodePosition = nodePosition;
    }
    
    public Vector2 getNodePosition()
    {
        return nodePosition;
    }

    public double getTrueAnomaly()
    {
        return trueAnomaly;
    }

    public Vector2 getThrustVector()
    {
        return thrustVector;
    }

    public void setThrustVector(Vector2 thrustVector)
    {
        this.thrustVector = thrustVector;
    }
}
