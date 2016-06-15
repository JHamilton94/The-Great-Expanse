using UnityEngine;
using System.Collections;
using System;

public class TestHyperbolaScript : MonoBehaviour {

    public double GRAV_CONST;
    public Vector2 position;
    public Vector2 velocity;

    private Vector2 eccentricity;

    private double time;
    private double timeStep;

    private double meanAnomaly;
    private double anomalyAtEpoch;
    private double eccentricAnomaly;
    private double trueAnomaly;

    private double semiMajorAxis;
    private Vector2 perigee;
    private double mechanicalEnergy;
    private double semiLatusRectum;

    private double angularMomentum; 


	// Use this for initialization
	void Start () {
        time = 0;
        timeStep = 0.01d;

        meanAnomaly = 0;
        anomalyAtEpoch = 0;
        double x = (eccentricity.magnitude + Math.Cos(trueAnomaly)) / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
        eccentricAnomaly = Math.Log(x + Math.Sqrt(x * x - 1));
        eccentricity = ((position * (float)(Vector2.SqrMagnitude(velocity) - ((GRAV_CONST) / position.magnitude))) - ((float)(Vector2.Dot(position, velocity)) * velocity)) *
            (float)(1 / (GRAV_CONST));

        mechanicalEnergy = (Vector2.SqrMagnitude(velocity) / 2) - ((GRAV_CONST) / position.magnitude);

        semiMajorAxis = -((GRAV_CONST) / (2 * mechanicalEnergy));

        angularMomentum = velocity.magnitude*position.magnitude;

        double altitudeOfPerigee = -semiMajorAxis * (eccentricity.magnitude - 1);
        perigee = eccentricity.normalized * (float)altitudeOfPerigee;
        semiLatusRectum = semiMajorAxis * (1 - Vector2.SqrMagnitude(eccentricity));
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        meanAnomaly = (Math.Pow(GRAV_CONST, 2) * Math.Pow(Vector2.SqrMagnitude(eccentricity) - 1, 1.5) * time) / Math.Pow(angularMomentum, 3);

        double eccentricAnomaly1 = 0;
        while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3)/GRAV_CONST) * 
            (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) - 
            ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly1)) - eccentricAnomaly1)) > timeStep)
        {
            Debug.Log(eccentricAnomaly1);
            eccentricAnomaly1 += 1.0f;
        }

        while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / GRAV_CONST) *
            (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) -
            ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly1)) - eccentricAnomaly1)) < timeStep)
        {
            eccentricAnomaly1 -= 0.1f;
        }

        while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / GRAV_CONST) *
            (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) -
            ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly1)) - eccentricAnomaly1)) > timeStep)
        {
            eccentricAnomaly1 += 0.01f;
        }

        while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / GRAV_CONST) *
            (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) -
            ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly1)) - eccentricAnomaly1)) < timeStep)
        {
            eccentricAnomaly1 -= 0.001f;
        }

        while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / GRAV_CONST) *
            (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) -
            ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly1)) - eccentricAnomaly1)) > timeStep)
        {
            eccentricAnomaly1 += 0.0001f;
        }
        while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / GRAV_CONST) *
            (((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly) -
            ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly1)) - eccentricAnomaly1)) < timeStep)
        {
            eccentricAnomaly1 -= 0.00001f;
        }

        trueAnomaly = Math.Atan(Math.Tanh(eccentricAnomaly1 / 2) / Math.Sqrt((eccentricity.magnitude - 1) / (eccentricity.magnitude + 1))) * 2;

        position = new Vector2((float)Math.Cos(trueAnomaly), (float)Math.Sin(trueAnomaly));
        position = position.normalized * (float)(semiLatusRectum / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly))));
        time += timeStep;
        eccentricAnomaly = eccentricAnomaly1;

        /*while ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly < meanAnomaly)
        {
            eccentricAnomaly += 1.0f;
        }

        while ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly > meanAnomaly)
        {
            eccentricAnomaly -= 0.1f;
        }

        while ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly < meanAnomaly)
        {
            eccentricAnomaly += 0.01f;
        }

        while ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly > meanAnomaly)
        {
            eccentricAnomaly -= 0.001f;
        }

        while ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly < meanAnomaly)
        {
            eccentricAnomaly += 0.0001f;
        }
        while ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly > meanAnomaly)
        {
            eccentricAnomaly -= 0.00001f;
        }

        trueAnomaly = Math.Atan(Math.Tanh(eccentricAnomaly / 2) / Math.Sqrt((eccentricity.magnitude - 1) / (eccentricity.magnitude + 1))) * 2;

        position = new Vector2((float)Math.Cos(trueAnomaly), (float)Math.Sin(trueAnomaly));
        position = position.normalized * (float)(semiLatusRectum / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly))));
        time += timeStep;*/

    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(position, 0.1f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector2(0, 0), eccentricity);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(perigee, 0.1f);
    }
}
