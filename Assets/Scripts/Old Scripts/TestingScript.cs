using UnityEngine;
using System.Collections;
using System;

public class TestingScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector2 eccentricity = new Vector2(1, 1.1f);
        double eccentricAnomaly = 0; //trying to find this one
        double eccentricAnomalyAtPerigee = 0;
        double semiMajorAxis = -10;
        double mu = 900;
        while (eccentricAnomaly < 10)
        {
            Debug.Log("Time: " + 2);
            eccentricAnomaly += 0.1d;
            Debug.Log("eccentric anomaly: " + eccentricAnomaly);
            Debug.Log("Calculated time: " + Math.Sqrt(Math.Pow(-semiMajorAxis, 3)/mu) * (eccentricity.magnitude * (Math.Sinh(eccentricAnomaly) - eccentricAnomaly)));
        }
        Debug.Break();
	}
}
