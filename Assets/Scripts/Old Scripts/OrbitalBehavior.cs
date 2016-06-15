using UnityEngine;
using System.Collections;
using System;

public class OrbitalBehavior : MonoBehaviour {

    //user input
    public Vector2 position;
    public Vector2 velocity;
    public double GRAV_CONST;
    public Vector2 thrustVector;

    //Orbital elements
    private Vector2 eccentricity;
    private Vector2 perigee;
    private Vector2 apogee;
    private double mechanicalEnergy;
    private double semiMajorAxis;
    private double perigeeAngle;
    private double anomalyAtEpoch;
    private double meanAnomaly;
    private double eccentricAnomaly;
    private double trueAnomaly;
    private double worldAngleVector;
    private double altitude;
    private double velocityAngle;
    private double velocityMagnitude;
    private bool towardsPerigee;
    private bool clockwise;

    //Helper variables
    private double timeStep;

    // Use this for initialization
    void Start () {

        //Setup initial orbit
        calculateInitialOrbitalElements(position, velocity);

        //Set time step
        timeStep = 0.01d;
    }


	// Update is called once per frame
	void FixedUpdate () {

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W");
            calculateInitialOrbitalElements(this.position, this.velocity + this.thrustVector);
        }

        //Move space craft forward in orbit
        calculateNextPosition();

        

    }

    private void calculateNextPosition()
    {
        //Calculate percentage of orbit being crossed
        double orbitalSpeed = Math.Sqrt(GRAV_CONST / semiMajorAxis);

        //calculate mean anomaly
        if (clockwise)
        {
            meanAnomaly = anomalyAtEpoch - (orbitalSpeed * timeStep);
            if (meanAnomaly < -Math.PI)
            {
                meanAnomaly = (Math.PI + (meanAnomaly + (Math.PI)));
            }
        }
        else
        {
            meanAnomaly = anomalyAtEpoch + (orbitalSpeed * timeStep);
            if (meanAnomaly > Math.PI)
            {
                meanAnomaly = -(Math.PI - (meanAnomaly - (Math.PI)));
            }
        }




        //Calculate eccentricAnomaly
        eccentricAnomaly = calculateEccentricAnomaly();

        //calculate true anomaly
        trueAnomaly = Math.Acos(
            (Math.Cos(eccentricAnomaly) - eccentricity.magnitude) /
            (1 - (eccentricity.magnitude * Math.Cos(eccentricAnomaly)))
            );
        if (meanAnomaly < 0)
        {
            trueAnomaly = -trueAnomaly;
        }

        //Calculate world angle vector
        worldAngleVector = trueAnomaly + perigeeAngle;

        //calculate altitude
        altitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
            (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));

        //Find new position
        position = new Vector2((float)Math.Cos(worldAngleVector), (float)Math.Sin(worldAngleVector));
        position = position.normalized * (float)altitude;

        //Calculate side of orbit
        towardsPerigee = (clockwise && meanAnomaly > 0) || (!clockwise && meanAnomaly < 0);


        //Calculate velocity vector
        velocityMagnitude = Math.Sqrt(GRAV_CONST * ((2 / position.magnitude) - (1 / semiMajorAxis)));
        velocityAngle = calculateVelocityAngle();
        velocity = new Vector2((float)Math.Cos(velocityAngle), (float)Math.Sin(velocityAngle));
        velocity.Normalize();
        velocity *= (float)velocityMagnitude;

        //Calculate side of orbit
        towardsPerigee = (convertToRadians(Vector2.Angle(eccentricity, velocity)) < Math.PI / 2);

        //advance epoch
        anomalyAtEpoch = meanAnomaly;

    }

    public void calculateInitialOrbitalElements(Vector2 inPosition, Vector2 inVelocity)
    {
        //set new velocity
        velocity = inVelocity;

        //Calculate Eccentricity
        eccentricity = ((inPosition * (float)(Vector2.SqrMagnitude(inVelocity) - (GRAV_CONST / inPosition.magnitude))) - ((float)(Vector2.Dot(inPosition, inVelocity)) * inVelocity)) *
            (float)(1 / GRAV_CONST);

        //calculate mechanical energy
        mechanicalEnergy = (Vector2.SqrMagnitude(velocity) / 2) - (GRAV_CONST / inPosition.magnitude);

        //Calculate semiMajor axis
        semiMajorAxis = -(GRAV_CONST / (2 * mechanicalEnergy));

        //Calculate Perigee
        double altitudeOfPerigee = semiMajorAxis * (1 - eccentricity.magnitude);
        perigee = eccentricity.normalized * (float)altitudeOfPerigee;

        //calculate apogee
        double altitudeOgApogee = semiMajorAxis * (1 + eccentricity.magnitude);
        apogee = eccentricity.normalized * -(float)altitudeOgApogee;

        //Calculate perigee angle
        perigeeAngle = Math.Atan2(eccentricity.y, eccentricity.x);

        //Clockwise?
        clockwise = clockwiseFunc();

        //Calculate side of orbit
        towardsPerigee = (convertToRadians(Vector2.Angle(eccentricity, inVelocity)) < Math.PI / 2);

        //Starting true anomaly
        trueAnomaly = Vector2.Angle(eccentricity, inPosition);
        trueAnomaly = convertToRadians(trueAnomaly);
        if (towardsPerigee)
        {
            trueAnomaly = -trueAnomaly;
        }

        //Calculate eccentric anomaly
        eccentricAnomaly = Math.Acos((eccentricity.magnitude + Math.Cos(trueAnomaly)) /
                (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly))));
        if (towardsPerigee)
        {
            eccentricAnomaly = -eccentricAnomaly;
        }


        ////Calculate velocity vector
        //velocityMagnitude = Math.Sqrt(GRAV_CONST * ((2 / position.magnitude) - (1 / semiMajorAxis)));
        //velocityAngle = calculateVelocityAngle();
        //velocity = new Vector2((float)Math.Cos(velocityAngle), (float)Math.Sin(velocityAngle));
        //velocity.Normalize();
        //velocity *= (float)velocityMagnitude;


        //calculate anomaly at starting epoch
        anomalyAtEpoch = eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly);
    }

    public void OnDrawGizmos()
    {

        //Draw position
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(position, 0.1f);
        Gizmos.DrawLine(new Vector2(0, 0), position);

        //Draw velocity
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(position, position + velocity);

        //Draw perigee
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(perigee, 0.1f);

        //draw apogee
        Gizmos.DrawSphere(apogee, 0.1f);

        //Draw semiMajor Axis
        Gizmos.color = Color.red;
        Gizmos.DrawLine(perigee, apogee);

        //Draw eccentricity
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector2(0.0f, 0.0f), eccentricity);
    }

    private bool clockwiseFunc()
    {
        Vector3 crossProduct;
        crossProduct = Vector3.Cross(convertToVec3(position), convertToVec3(velocity));
        if(crossProduct.z > 0) {
            return false;
        }
        return true; 
    }

    private double calculateVelocityAngle()
    {

        double returnVelocityAngle = 0;

        //find new velocity vector
        double k = position.magnitude / (semiMajorAxis);
        double alpha = Math.Acos(((2 - (2 * Vector2.SqrMagnitude(eccentricity))) /
            (k * (2 - k))) - 1);

        double trueAnomalyModifier = (Math.PI - alpha) / 2;

        if (!clockwise)
        {
            if (towardsPerigee)
            {
                trueAnomalyModifier = Math.PI - trueAnomalyModifier;
            }
        }

        if (clockwise)
        {
            if (towardsPerigee)
            {
                returnVelocityAngle = Math.Abs(trueAnomaly) + trueAnomalyModifier + perigeeAngle + Math.PI;
            }
            else {
                trueAnomalyModifier = Math.PI - trueAnomalyModifier;
                returnVelocityAngle = trueAnomaly + trueAnomalyModifier + -perigeeAngle + Math.PI;
            }
        }
        else {
            returnVelocityAngle = trueAnomaly + trueAnomalyModifier + perigeeAngle;
        }

        return returnVelocityAngle;
    }

    private Vector3 convertToVec3(Vector2 inVec)
    {
        return new Vector3(inVec.x, inVec.y, 0.0f);
    }

    private double calculateEccentricAnomaly()
    {
        double eccentricAnomaly = 0.0f;

        while (eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly) < meanAnomaly)
        {
            eccentricAnomaly += 1.0f;
        }

        while (eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly) > meanAnomaly)
        {
            eccentricAnomaly -= 0.1f;
        }

        while (eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly) < meanAnomaly)
        {
            eccentricAnomaly += 0.01f;
        }

        while (eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly) > meanAnomaly)
        {
            eccentricAnomaly -= 0.001f;
        }

        while (eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly) < meanAnomaly)
        {
            eccentricAnomaly += 0.0001f;
        }
        while (eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly) > meanAnomaly)
        {
            eccentricAnomaly -= 0.00001f;
        }

        return eccentricAnomaly;
    }

    private double convertToRadians(double degrees)
    {
        return (degrees * Math.PI) / 180;
    }

   
}
