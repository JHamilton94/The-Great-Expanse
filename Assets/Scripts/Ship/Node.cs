using UnityEngine;
using System.Collections;

public class Node {

    private float meanAnomaly;
    private Vector2 thrustVector;

    public Node(float meanAnomaly, Vector2 thrustVector)
    {
        this.meanAnomaly = meanAnomaly;
        this.thrustVector = thrustVector;
    }
    
    public float getMeanAnomaly()
    {
        return meanAnomaly;
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
