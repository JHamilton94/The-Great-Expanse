using UnityEngine;
using System.Collections;

public class Node {

    private GravityElementsClass maneuver;
    private double trueAnomaly;
    private Vector2 nodePosition;

    public Node(GravityElementsClass maneuver, double trueAnomaly, Vector2 nodePosition)
    {
        this.maneuver = maneuver;
        this.nodePosition = nodePosition;
        this.trueAnomaly = trueAnomaly;
    }
    
    public Vector2 getNodePosition()
    {
        return nodePosition;
    }

    public double getTrueAnomaly()
    {
        return trueAnomaly;
    }

    public GravityElementsClass getManeuver()
    {
        return maneuver;
    }
}
