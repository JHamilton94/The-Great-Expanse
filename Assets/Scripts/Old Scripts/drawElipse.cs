using UnityEngine;
using System.Collections;
using System;

public class drawElipse : MonoBehaviour {

    //Variables set by user
    public double GRAV_CONST;

    public Vector2 velocity;
    public Vector2 position;


    //Polar Coordinates
    private double length;
    private double angle;


    //Orbital Elements
    private double semiMajorAxis;
    private double trueAnomaly;

    private Vector2 eccentricity;


    //Helper Orbital Elements
    private double mechanicalEnergy;
    private double anomalyAtEpoch;
    private double meanAnomaly;
    private double stepSize;
    private double perigeeAngle;
    private double velocityAngle;
    private double eccentricAnomaly;

    private Vector2 normalAngleVector;
    private Vector2 perigee;
    private Vector2 apogee;

    private Boolean clockwise;

    //temp
    private double maxSpeed = double.PositiveInfinity;

    // Use this for initialization
    //sets up constant orbital elements
    void Start () {

        //Calculate Eccentricity
        eccentricity = ((position * (float)(Vector2.SqrMagnitude(velocity) - (GRAV_CONST / position.magnitude))) - ((float)(Vector2.Dot(position, velocity)) * velocity)) *
            (float)(1 / GRAV_CONST);

        //Calculate Mechanical Energy
        mechanicalEnergy = (Vector2.SqrMagnitude(velocity) / 2) - (GRAV_CONST / position.magnitude);

        //Calculate Semi Major Axis
        if (eccentricity.magnitude != 1)
        {
            semiMajorAxis = -(GRAV_CONST / (2 * mechanicalEnergy));

        }
        else
        {
            semiMajorAxis = double.PositiveInfinity; //WTF do i do with this?
            semiMajorAxis = 1; //Positive infinity breaks things, this is the next best thing
        }

        //Calculate starting true anomaly
        trueAnomaly = Math.Acos(
            ((eccentricity.x * position.x) * (eccentricity.y * position.y)) / 
            (eccentricity.magnitude * position.magnitude)
            );

        //Calculate starting anomaly at epoch
        eccentricAnomaly = Math.Acos((eccentricity.magnitude + Math.Cos(trueAnomaly)) / 
            (1 + eccentricity.magnitude*Math.Cos(trueAnomaly)));
        anomalyAtEpoch = eccentricAnomaly - (eccentricity.magnitude * Math.Sin(eccentricAnomaly));

        //clockwise or counterclockwise?
        if (velocity.x/velocity.y > eccentricity.x/eccentricity.y)
        {
            clockwise = true;
        }
        else
        {
            clockwise = false;
        }

        //Calculate perigee and apogee vectors
        double altitudeOfPerigee = semiMajorAxis * (1 - eccentricity.magnitude);
        double altitudeOgApogee = semiMajorAxis * (1 + eccentricity.magnitude);
        perigee = eccentricity.normalized * (float)altitudeOfPerigee;
        apogee = eccentricity.normalized * -(float)altitudeOgApogee;
        perigeeAngle = Math.Atan2(eccentricity.y, eccentricity.x);

        //Calculate mean anomaly at starting position


        stepSize = 0.01d;
    }

    // Update is called once per frame
    void FixedUpdate() {

        calculatePolarCoords();

        if(velocity.x < 0.25 && velocity.x > -0.25)
        {
            if (velocity.y > 1.75 && velocity.y < 2.25)
            {
                Debug.Log("HERE");
            }
        }
    }

    private void calculatePolarCoords()
    {

        //Calculate percentage of orbit being crossed
        double orbitalSpeed = Math.Sqrt(GRAV_CONST / semiMajorAxis);

        meanAnomaly = anomalyAtEpoch + (orbitalSpeed * stepSize);
        //Wrap meanAnomaly to 0 at max 2PI
        if ((meanAnomaly) > (2 * Math.PI))
        {
            meanAnomaly = meanAnomaly - (2 * Math.PI);
        }

        
        

        //Calculate eccentricAnomaly
        eccentricAnomaly = calculateEccentricAnomaly();

        //True anomaly calculation
        //trueAnomaly = Math.Acos(Vector2.Dot(eccentricity, position)/(eccentricity.magnitude*position.magnitude));

        //Calculate trueanomaly
        trueAnomaly = Math.Acos(
            (Math.Cos(eccentricAnomaly) - eccentricity.magnitude) /
            (1 - (eccentricity.magnitude * Math.Cos(eccentricAnomaly)))
            );
       
        //assemble polar coordinates of new position and velocity
        angle = trueAnomaly;
        length = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
            (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));

        //Nessecary for full orbit, half orbits otherwise 
        if ((meanAnomaly) > Math.PI){angle = -angle;}
        angle = angle + perigeeAngle;

        //find new position
        normalAngleVector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        position = normalAngleVector*(float)length;

        //find new velocity vector
        double alpha = 0;
        if (eccentricity.magnitude != 0)
        {
            double k = position.magnitude / semiMajorAxis;
            alpha = Math.Acos(
                ((2 - (2 * Vector2.SqrMagnitude(eccentricity))) /
                (k * (2 - k))) - 1
            );
        }
        double trueAnomalyModifier;
        if (meanAnomaly < Math.PI)
        {
            trueAnomalyModifier = ((Math.PI - alpha) / 2);
            velocityAngle = trueAnomaly + perigeeAngle + trueAnomalyModifier;
        }
        else
        {
            trueAnomalyModifier = ((Math.PI - alpha) / 2);
            velocityAngle = -(trueAnomaly - perigeeAngle + trueAnomalyModifier + Math.PI);
        }
        velocity = new Vector2((float)Math.Cos(velocityAngle), (float)Math.Sin(velocityAngle)); //Problems when eccentricity.magnitude = 0

        //find velocity magnitude
        velocity = velocity * (float)(GRAV_CONST * ((2/position.magnitude) - (1/semiMajorAxis)));


        //advance meanAnomaly
        anomalyAtEpoch = meanAnomaly;
    }

    public void OnDrawGizmos()
    {
        //Draw ship
        Gizmos.DrawSphere(convertTo3D(position), 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(position, position+velocity);

        //Draw perogee and apogee
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(perigee, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(apogee, 0.1f);

        //Draw Semi Major axis
        Gizmos.color = Color.red;
        Gizmos.DrawLine(perigee, apogee);

        //Draw Eccentricity
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), eccentricity);
    }

    

    public Vector3 convertTo3D(Vector2 inVec)
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
}



/* OLD CODE I MIGHT BRING BACK
-----------------------------

        /*Dont know why this is here
        double spaceCraftHeading = Math.Acos((eccentricity.magnitude * Math.Sin(trueAnomaly)) /
            (1 + eccentricity.magnitude * Math.Cos(trueAnomaly)));
            *//*

        //Calculate percentage of orbit being crossed
        double orbitalSpeed = Math.Sqrt(GRAV_CONST / semiMajorAxis);
        meanAnomaly = anomalyAtEpoch + (orbitalSpeed * stepSize);

        //Wrap meanAnomaly to 0 at max 2PI
        if ((meanAnomaly) > (2 * Math.PI))
        {
            meanAnomaly = meanAnomaly - (2 * Math.PI);
        }

        //Calculate eccentricAnomaly
        double eccentricAnomaly = calculateEccentricAnomaly();

        //Calculate trueanomaly
        double trueAnomaly = Math.Acos(
            (Math.Cos(eccentricAnomaly) - eccentricity) /
            (1 - (eccentricity * Math.Cos(eccentricAnomaly)))
            );

        //Assemble polar coordinates
        angle = trueAnomaly;
        length = (semiMajorAxis * (1 - (eccentricity * eccentricity))) /
            (1 + (eccentricity * Math.Cos(trueAnomaly)));

        //Wrap cos inverse function
        if(clockwise)
        {
            if ((meanAnomaly) < Math.PI)
            {
                angle = -angle;
            }
        }
        else
        {
            if ((meanAnomaly) > Math.PI)
            {
                angle = -angle;
            }
        }

        
        //Adjust for orbit angle
        normalAngleVector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        double cosA = Math.Cos(angleOfOrbit);
        double sinA = Math.Sin(angleOfOrbit);
        normalAngleVector = new Vector2((float)(cosA * normalAngleVector.x - sinA * normalAngleVector.y), (float)(sinA * normalAngleVector.x + cosA * normalAngleVector.y));
        normalAngleVector.Normalize();


        //advance meanAnomaly
        anomalyAtEpoch = meanAnomaly;
        */

/*private double calculateEccentricAnomaly()
{
    double eccentricAnomaly = 0.0f;

    while(eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly) < meanAnomaly)
    {
        eccentricAnomaly += 1.0f;
    }

    while(eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly) > meanAnomaly)
    {
        eccentricAnomaly -= 0.1f;
    }

    while (eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly) < meanAnomaly)
    {
        eccentricAnomaly += 0.01f;
    }

    while (eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly) > meanAnomaly)
    {
        eccentricAnomaly -= 0.001f;
    }

    while (eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly) < meanAnomaly)
    {
        eccentricAnomaly += 0.0001f;
    }
    while (eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly) > meanAnomaly)
    {
        eccentricAnomaly -= 0.00001f;
    }

    return eccentricAnomaly;
}*/
