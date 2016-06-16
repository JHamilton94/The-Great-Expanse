using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ShipGravityBehavior : MonoBehaviour
{

    public bool debugMode;
    public GravityElements gravityElements;

    private int sphereChangeImmunity;

    // Use this for initialization
    void Start()
    {
        
        //First time setup
        gravityElements = GetComponent<GravityElements>();
        gravityElements.MassiveBody = findInfluencingCelestialBody(transform.position, gravityElements.velocity, null);
        gravityElements.TimeStep = GlobalElements.timeStep;
        gravityElements.Position = transform.position - gravityElements.MassiveBody.transform.position;
        sphereChangeImmunity = 0;

        calculateInitialOrbitalElements(gravityElements.Position, gravityElements.velocity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //change spheres of influence
        changeSpheresOfInfluence(this.transform.position, gravityElements.velocity, gravityElements.massiveBody);
        
        //Calculate Next Orbital Position
        calculateNextOrbitalElements();

        //Move to that orbital position
        moveShip();
    }

    private void moveShip()
    {
        transform.position = gravityElements.Position + gravityElements.GlobalTransformationVector;
    }

    public void applyThrust(Vector2 thrust)
    {
        calculateInitialOrbitalElements(gravityElements.Position, gravityElements.velocity + thrust);
    }

    //<summary>
    //Takes in everything in global coordinates (relative to the origin(0, 0)) and returns the game object that is influencing the craft
    //</summary>
    //<param name="position"> Re-read the summary
    //<param name="asdf"> Here's to the little guys
    private GameObject findInfluencingCelestialBody(Vector2 position, Vector2 velocity, GameObject currentMassiveBody)
    {
        GameObject[] massiveBodies = GameObject.FindGameObjectsWithTag("MassiveBody");
        List<GameObject> spheresOfInfluence = new List<GameObject>();
        
        for (int i = 0; i < massiveBodies.Length; i++)
        {
            //quick and dirty calculation of altitude
            double mu = massiveBodies[i].GetComponent<MassiveBodyElements>().mass * GlobalElements.GRAV_CONST;
            Vector2 relativePosition = position - convertToVec2(massiveBodies[i].transform.position);
            Vector2 relativeVelocity = velocity + massiveBodies[i].GetComponent<GravityElements>().velocity;
            Vector2 eccentricity = calculateEccentricity(relativePosition, relativeVelocity, mu);
            OrbitTypes orbitType = determineOrbitType(eccentricity);
            double mechanicalEnergy = calculateMechanicalEnergy(relativePosition, relativeVelocity, mu, orbitType);
            double semiMajorAxis = calculateSemiMajorAxis(mechanicalEnergy, mu, orbitType);
            Vector2 perigee = calculatePerigee(semiMajorAxis, eccentricity, orbitType);
            double semiLatusRectum = calculateSemiLatusRectum(semiMajorAxis, eccentricity, perigee, orbitType); //semiMajorAxis * (1 - Math.Pow(eccentricity.magnitude, 2));
            double trueAnomaly = Vector2.Angle(relativePosition, eccentricity);
            trueAnomaly = convertToRadians(trueAnomaly);
            trueAnomaly = Math.Abs(wrapAngle(trueAnomaly));
            double altitude = Vector2.Distance(massiveBodies[i].transform.position, transform.position);//semiLatusRectum / (1 + eccentricity.magnitude * Math.Cos(trueAnomaly));

            

            if (massiveBodies[i].GetComponent<MassiveBodyElements>().SphereOfInfluence > altitude)
            {
                if (altitude < 0)
                {
                    Debug.Log("altitude: " + altitude);
                    Debug.Log("semilatusrectum: " + semiLatusRectum);
                    Debug.Log("Eccentricity: " + eccentricity.magnitude);
                    Debug.Log("true anomaly: " + trueAnomaly);
                }
                spheresOfInfluence.Add(massiveBodies[i]);
                
            }
        }
        
        double smallestDistance = double.PositiveInfinity;
        GameObject returnGameObject = currentMassiveBody;
        foreach (GameObject massiveBody in spheresOfInfluence)
        {
            double distance = Vector2.Distance(massiveBody.transform.position, transform.position);
            if(distance < smallestDistance)
            {
                smallestDistance = distance;
                returnGameObject = massiveBody;
            }
        }

        return returnGameObject;
    }

    private void calculateNextOrbitalElements()
    {
        //update timestep
        gravityElements.TimeStep = GlobalElements.timeStep;

        //Adjust tranformation vector
        gravityElements.GlobalTransformationVector = gravityElements.MassiveBody.transform.position;

        //Calculate next meanAnomaly
        gravityElements.MeanAnomaly = calculateMeanAnomaly(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, gravityElements.AnomalyAtEpoch,
            gravityElements.TimeStep, gravityElements.TimeAtEpoch, gravityElements.Clockwise, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate Eccentric Anomaly
        gravityElements.EccentricAnomaly = calculateEccentricAnomaly(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, GlobalElements.GRAV_CONST, gravityElements.TimeStep, gravityElements.TimeAtEpoch,
            gravityElements.MeanAnomaly, gravityElements.EccentricAnomaly, gravityElements.Mu, gravityElements.Clockwise, gravityElements.OrbitType);

        //CalculateTrueAnomaly
        gravityElements.TrueAnomaly = calculateTrueAnomaly(gravityElements.Eccentricity, gravityElements.EccentricAnomaly, gravityElements.MeanAnomaly, gravityElements.OrbitType);

        //Calculate Altitude
        gravityElements.Altitude = calculateAltitude(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, gravityElements.SemiLatusRectum, gravityElements.TrueAnomaly, gravityElements.OrbitType);
        
        //Calculate positionVector
        gravityElements.Position = calculatePosition(gravityElements.Perigee, gravityElements.TrueAnomaly, gravityElements.GlobalRotationAngle, gravityElements.Altitude, gravityElements.OrbitType);

        //Are we going towards the perigee?
        gravityElements.TowardsPerigee = towardsPerigeeOrbit(gravityElements.MeanAnomaly, gravityElements.Clockwise, gravityElements.TimeAtEpoch, gravityElements.OrbitType);

        //Calculate velocity angle
        gravityElements.VelocityAngle = calculateVelocityAngle(gravityElements.Position, gravityElements.Eccentricity, gravityElements.SemiMajorAxis,
            gravityElements.TrueAnomaly, gravityElements.GlobalRotationAngle, gravityElements.Clockwise, gravityElements.TowardsPerigee, gravityElements.OrbitType);

        //Calculate Speed
        gravityElements.Speed = calculateSpeed(gravityElements.Position, gravityElements.SemiMajorAxis, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate Velocity
        gravityElements.velocity = assembleVelocityVector(gravityElements.VelocityAngle, gravityElements.Speed);

        //advance epoch
        gravityElements.AnomalyAtEpoch = gravityElements.MeanAnomaly;

        //Advance time
        gravityElements.TimeAtEpoch = advanceTime(gravityElements.TimeAtEpoch, gravityElements.TimeStep, gravityElements.Clockwise, gravityElements.OrbitType);

        
    }

    //<summary>
    //Determines whether or not to change the sphere of influence then changes it
    //</summary>
    //<param name="position"> The position of the ship in global coordinates
    //<param name="velocity"> The velocity relative to the current body being orbited
    //<param name="the rest"> Fuck it you get the idea
    //<returns> Returns nothing, like anyone who went to blockbuster after netflix got big
    private void changeSpheresOfInfluence(Vector2 position, Vector2 velocity, GameObject currentMassiveBody)
    {
        GameObject newSphereOfInfluence = findInfluencingCelestialBody(position, velocity, currentMassiveBody);
        //change spheres of influence
        if (newSphereOfInfluence.transform.GetInstanceID() != currentMassiveBody.transform.GetInstanceID()
            && sphereChangeImmunity < 1)
        {
            //From small to big
            if (newSphereOfInfluence.GetComponent<MassiveBodyElements>().massiveBodyType > currentMassiveBody.GetComponent<MassiveBodyElements>().massiveBodyType)
            {
                gravityElements.massiveBody = newSphereOfInfluence;
                calculateInitialOrbitalElements(position - convertToVec2(newSphereOfInfluence.transform.position), velocity + currentMassiveBody.GetComponent<GravityElements>().velocity);
                sphereChangeImmunity = 10;
            }
            else
            {
                gravityElements.massiveBody = newSphereOfInfluence;
                calculateInitialOrbitalElements(position - convertToVec2(newSphereOfInfluence.transform.position), velocity - newSphereOfInfluence.GetComponent<GravityElements>().velocity);
                sphereChangeImmunity = 10;
            }
        }
        else
        {
            sphereChangeImmunity--;
        }
    }


    private double advanceTime(double timeAtEpoch, double timeStep, bool clockwise, OrbitTypes orbitType)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return double.PositiveInfinity;
            case OrbitTypes.elliptical:
                return double.PositiveInfinity;
            case OrbitTypes.parabolic:
                return double.PositiveInfinity;
            case OrbitTypes.hyperbolic:
                if (clockwise)
                {
                    return timeAtEpoch - timeStep;
                }
                else
                {
                    return timeAtEpoch + timeStep;
                }
            default:
                return double.PositiveInfinity;
        }
    }

    private Vector2 assembleVelocityVector(double velocityAngle, double speed)
    {
        return (new Vector2((float)Math.Cos(velocityAngle), (float)Math.Sin(velocityAngle)).normalized * (float)speed);
    }

    private double calculateVelocityAngle(Vector2 position, Vector2 eccentricity, double semiMajorAxis,
        double trueAnomaly, double globalRotationAngle, bool clockwise, bool towardsPerigee, OrbitTypes orbitType)
    {
        double returnVelocityAngle = 0;
        double trueAnomalyModifier = 0;
        double alpha = 0;
        double k = 0;

        switch (orbitType)
        {
            case OrbitTypes.circular:
                //find new velocity vector
                k = position.magnitude / (semiMajorAxis);
                alpha = Math.Acos(((2 - (2 * Vector2.SqrMagnitude(eccentricity))) /
                    (k * (2 - k))) - 1);
                trueAnomalyModifier = (Math.PI - alpha) / 2;
                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;

                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                    }
                }
                break;
            case OrbitTypes.elliptical:
                //find new velocity vector
                k = position.magnitude / (semiMajorAxis);
                alpha = Math.Acos(((2 - (2 * Vector2.SqrMagnitude(eccentricity))) /
                    (k * (2 - k))) - 1);

                trueAnomalyModifier = (Math.PI - alpha) / 2;

                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;

                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                    }
                }
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                //find new velocity vector
                k = position.magnitude / (semiMajorAxis);
                alpha = Math.Acos(((2 - (2 * Vector2.SqrMagnitude(eccentricity))) /
                    (k * (2 - k))) - 1);

                trueAnomalyModifier = (Math.PI - alpha) / 2;

                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnVelocityAngle = trueAnomaly - trueAnomalyModifier + globalRotationAngle;
                        returnVelocityAngle -= Math.PI;
                    }
                    else
                    {
                        returnVelocityAngle = trueAnomaly + trueAnomalyModifier + globalRotationAngle;
                    }
                }
                break;
        }
        return returnVelocityAngle;
    }

    private bool towardsPerigeeOrbit(double meanAnomaly, bool clockwise, double timeAtEpoch, OrbitTypes orbitType)
    {
        return (clockwise && meanAnomaly > 0) || (!clockwise && meanAnomaly < 0);
    }

    private Vector2 calculatePosition(Vector2 perigee, double trueAnomaly, double globalRotationAngle, double altitude, OrbitTypes orbitType)
    {
        Vector2 returnPosition = Vector2.right * float.PositiveInfinity;
        returnPosition = new Vector2((float)Math.Cos(trueAnomaly + globalRotationAngle), (float)Math.Sin(trueAnomaly + globalRotationAngle));
        returnPosition = returnPosition.normalized * (float)altitude;
        return returnPosition;
    }

    private double calculateTrueAnomaly(Vector2 eccentricity, double eccentricAnomaly, double meanAnomaly, OrbitTypes orbitType)
    {
        double returnTrueAnomaly = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                //calculate true anomaly
                returnTrueAnomaly = Math.Acos(
                    (Math.Cos(eccentricAnomaly) - eccentricity.magnitude) /
                    (1 - (eccentricity.magnitude * Math.Cos(eccentricAnomaly)))
                    );

                if (meanAnomaly < 0)
                {
                    returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                }
                break;
            case OrbitTypes.elliptical:
                //calculate true anomaly
                returnTrueAnomaly = Math.Acos(
                    (Math.Cos(eccentricAnomaly) - eccentricity.magnitude) /
                    (1 - (eccentricity.magnitude * Math.Cos(eccentricAnomaly)))
                    );
                if (meanAnomaly < 0)
                {
                    returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                }
                break;
            case OrbitTypes.parabolic:
                returnTrueAnomaly = Math.Atan(
                    (Math.Sqrt((eccentricity.magnitude + 1) /
                    (eccentricity.magnitude - 1))) *
                    Math.Tanh(eccentricAnomaly / 2)
                    );
                if (meanAnomaly < 0)
                {
                    returnTrueAnomaly = -returnTrueAnomaly;
                }
                break;
            case OrbitTypes.hyperbolic:
                returnTrueAnomaly = Math.Atan(Math.Tanh(eccentricAnomaly / 2) / Math.Sqrt((eccentricity.magnitude - 1) / (eccentricity.magnitude + 1))) * 2;

                if (eccentricAnomaly < 0)
                {
                    returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                }
                else
                {
                    returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                }


                break;
        }
        return returnTrueAnomaly;
    }

    private double calculateEccentricAnomaly(Vector2 eccentricity, double semiMajorAxis, double GRAV_CONST,
        double timeStep, double timeAtEpoch, double meanAnomaly, double eccentricAnomaly, double mu, bool clockwise, OrbitTypes orbitType)
    {
        double returnEccentricAnomaly = 0.0d;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnEccentricAnomaly = meanAnomaly + (2 * eccentricity.magnitude * Math.Sin(meanAnomaly)) + (1.25 * Vector2.SqrMagnitude(eccentricity) * Math.Sin(2 * meanAnomaly));
                break;
            case OrbitTypes.elliptical:
                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) < meanAnomaly)
                {
                    returnEccentricAnomaly += 1.0f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) > meanAnomaly)
                {
                    returnEccentricAnomaly -= 0.1f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) < meanAnomaly)
                {
                    returnEccentricAnomaly += 0.01f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) > meanAnomaly)
                {
                    returnEccentricAnomaly -= 0.001f;
                }

                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) < meanAnomaly)
                {
                    returnEccentricAnomaly += 0.0001f;
                }
                while (returnEccentricAnomaly - eccentricity.magnitude * Math.Sin(returnEccentricAnomaly) > meanAnomaly)
                {
                    returnEccentricAnomaly -= 0.00001f;
                }
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) < timeAtEpoch)
                {
                    returnEccentricAnomaly += 1.0f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) > timeAtEpoch)
                {
                    returnEccentricAnomaly -= 0.1f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) < timeAtEpoch)
                {
                    returnEccentricAnomaly += 0.01f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) > timeAtEpoch)
                {
                    returnEccentricAnomaly -= 0.001f;
                }

                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) < timeAtEpoch)
                {
                    returnEccentricAnomaly += 0.0001f;
                }
                while (Math.Sqrt(Math.Pow(-semiMajorAxis, 3) / mu) * (eccentricity.magnitude * Math.Sinh(returnEccentricAnomaly) - returnEccentricAnomaly) > timeAtEpoch)
                {
                    returnEccentricAnomaly -= 0.00001f;
                }
                break;
        }



        return returnEccentricAnomaly;
    }

    private double calculateMeanAnomaly(Vector2 eccentricity, double semiMajorAxis,
        double anomalyAtEpoch, double timeStep, double timeAtEpoch, bool clockwise, double mu, OrbitTypes orbitType)
    {
        double orbitalSpeed;
        double returnMeanAnomaly = double.PositiveInfinity;

        //Calculate percentage of orbit being crossed
        orbitalSpeed = Math.Sqrt((mu) / Math.Pow(Math.Abs(semiMajorAxis), 3));
        if (clockwise)
        {
            returnMeanAnomaly = anomalyAtEpoch - (orbitalSpeed * timeStep);
        }
        else
        {
            returnMeanAnomaly = anomalyAtEpoch + (orbitalSpeed * timeStep);
        }

        if (orbitType != OrbitTypes.hyperbolic && orbitType != OrbitTypes.parabolic)
        {
            returnMeanAnomaly = wrapAngle(returnMeanAnomaly);
        }
        return returnMeanAnomaly;
    }

    //<summary>
    //Takes in position and velocity realtive to the body being orbited
    //</summary>
    private void calculateInitialOrbitalElements(Vector2 position, Vector2 velocity)
    {
        gravityElements.Mu = GlobalElements.GRAV_CONST * gravityElements.MassiveBody.GetComponent<MassiveBodyElements>().mass;
        gravityElements.Position = position;
        gravityElements.velocity = velocity;

        //Calculate Global Tranformation Vector
        gravityElements.GlobalTransformationVector = calculateGlobalTranformationVector(gravityElements.MassiveBody);

        //Calculate eccentricity
        gravityElements.Eccentricity = calculateEccentricity(position, velocity, gravityElements.Mu);

        //Determine orbit type
        gravityElements.OrbitType = determineOrbitType(gravityElements.Eccentricity);

        //Calculate Mechanical Energy
        gravityElements.MechanicalEnergy = calculateMechanicalEnergy(gravityElements.Position, gravityElements.velocity, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate Semi Major Axis
        gravityElements.SemiMajorAxis = calculateSemiMajorAxis(gravityElements.MechanicalEnergy, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate SemiLatusRectum
        gravityElements.SemiLatusRectum = calculateSemiLatusRectum(gravityElements.SemiMajorAxis, gravityElements.Eccentricity, gravityElements.Perigee, gravityElements.OrbitType);

        //Calculate Perigee
        gravityElements.Perigee = calculatePerigee(gravityElements.SemiMajorAxis, gravityElements.Eccentricity, gravityElements.OrbitType);

        //Calculate Apogee
        gravityElements.Apogee = calculateApogee(gravityElements.SemiMajorAxis, gravityElements.Eccentricity, gravityElements.OrbitType);

        //Calculate Center
        gravityElements.Center = calculateCenter(gravityElements.SemiMajorAxis, gravityElements.Perigee, gravityElements.OrbitType);

        //Calculate GlobalRotationAngle
        gravityElements.GlobalRotationAngle = calculateGlobalRotationAngle(gravityElements.Eccentricity, gravityElements.OrbitType);

        //Find orbital directions
        gravityElements.Clockwise = clockwiseOrbit(gravityElements.Position, gravityElements.velocity);
        gravityElements.TowardsPerigee = towardsPerigeeOrbit(gravityElements.velocity, gravityElements.Eccentricity, gravityElements.OrbitType);

        //Calculate trueAnomaly
        gravityElements.TrueAnomaly = calculateTrueAnomaly(gravityElements.Eccentricity, gravityElements.Position, gravityElements.TowardsPerigee, gravityElements.Clockwise, gravityElements.OrbitType);

        //Calculate Eccentric Anomaly
        gravityElements.EccentricAnomaly = calculateEccentricAnomaly(gravityElements.Eccentricity, gravityElements.TrueAnomaly, gravityElements.TowardsPerigee, gravityElements.OrbitType);

        //Calculate Anomaly at current epoch
        gravityElements.AnomalyAtEpoch = calculateAnomalyAtCurrentEpoch(gravityElements.Eccentricity, gravityElements.EccentricAnomaly, gravityElements.Clockwise, gravityElements.OrbitType);
        gravityElements.MeanAnomaly = gravityElements.AnomalyAtEpoch;

        //Calculate Angular Momentum
        gravityElements.AngularMomentum = calculateAngularMomentum(gravityElements.Eccentricity, gravityElements.Perigee, gravityElements.SemiMajorAxis, gravityElements.SemiLatusRectum,
            gravityElements.Mu, gravityElements.OrbitType);

        //Calculate time at epoch
        gravityElements.TimeAtEpoch = calculateTimeAtEpoch(gravityElements.Eccentricity, gravityElements.EccentricAnomaly, gravityElements.SemiMajorAxis, gravityElements.Mu,
            gravityElements.Clockwise, gravityElements.TowardsPerigee, gravityElements.OrbitType);

    }

    private Vector2 calculateGlobalTranformationVector(GameObject celestialBody)
    {
        return celestialBody.transform.position;
    }

    private double convertToWorldAngle(double localAngle, double globalRotationAngle, bool towardsPerigee)
    {
        if (towardsPerigee)
        {
            if (globalRotationAngle < 0)
            {
                localAngle = Math.Abs(localAngle) - Math.Abs(globalRotationAngle);
            }
            else
            {
                localAngle = Math.Abs(globalRotationAngle) - Math.Abs(localAngle);
            }
        }
        else
        {
            if (globalRotationAngle < 0)
            {
                localAngle = Math.Abs((2 * Math.PI) - Math.Abs(globalRotationAngle) - Math.Abs(localAngle));
            }
            else
            {
                localAngle = -Math.Abs((2 * Math.PI) - globalRotationAngle - localAngle);
            }
        }
        localAngle = wrapAngle(localAngle);
        return localAngle;
    }

    private double calculateTimeAtEpoch(Vector2 eccentricity, double eccentricAnomaly, double semiMajorAxis, double mu, bool clockwise, bool towardsPerigee, OrbitTypes orbitType)
    {
        double returnTime = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                break;
            case OrbitTypes.elliptical:
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                //Fuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuck
                returnTime = Math.Pow((Math.Pow(-semiMajorAxis, 3) / mu), 0.5) * ((eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly);
                break;
        }
        if (clockwise)
        {
            if (towardsPerigee)
            {
                returnTime = Math.Abs(returnTime);
            }
            else
            {
                returnTime = -Math.Abs(returnTime);
            }
        }
        else
        {
            if (towardsPerigee)
            {
                returnTime = -Math.Abs(returnTime);
            }
            else
            {
                returnTime = Math.Abs(returnTime);
            }
        }

        return returnTime;
    }

    private double calculateAngularMomentum(Vector2 eccentricity, Vector2 perigee, double semiMajorAxis, double semiLatusRectum, double mu, OrbitTypes orbitType)
    {
        return calculateSpeed(perigee, semiMajorAxis, mu, orbitType) * calculateAltitude(eccentricity, semiMajorAxis, semiLatusRectum, 0, orbitType);
    }

    private double calculateAltitude(Vector2 eccentricity, double semiMajorAxis, double semiLatusRectum, double trueAnomaly, OrbitTypes orbitType)
    {
        double returnAltitude = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.elliptical:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.parabolic:
                returnAltitude = (semiMajorAxis * (1 - (eccentricity.magnitude * eccentricity.magnitude))) /
                    (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
            case OrbitTypes.hyperbolic:
                returnAltitude = semiLatusRectum / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                break;
        }
        return returnAltitude;
    }

    private double calculateSpeed(Vector2 position, double semiMajorAxis, double mu, OrbitTypes orbitType)
    {
        return Math.Sqrt((mu) * ((2 / position.magnitude) - (1 / semiMajorAxis)));
    }

    private double calculateAnomalyAtCurrentEpoch(Vector2 eccentricity, double eccentricAnomaly, bool clockwise, OrbitTypes orbitType)
    {
        double returnAnomaly = 0;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnAnomaly = eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly);
                //flip sign if clockwise
                if (clockwise)
                {
                    returnAnomaly = -returnAnomaly;
                }
                break;
            case OrbitTypes.elliptical:
                returnAnomaly = eccentricAnomaly - eccentricity.magnitude * Math.Sin(eccentricAnomaly);
                //flip sign if clockwise
                if (clockwise)
                {
                    returnAnomaly = -returnAnomaly;
                }
                break;
            case OrbitTypes.parabolic:
                returnAnomaly = eccentricAnomaly + (Math.Pow(eccentricAnomaly, 3) / 3);
                //flip sign if clockwise
                if (clockwise)
                {
                    returnAnomaly = -returnAnomaly;
                }
                break;
            case OrbitTypes.hyperbolic:
                returnAnomaly = (eccentricity.magnitude * Math.Sinh(eccentricAnomaly)) - eccentricAnomaly;
                break;
        }


        return returnAnomaly;
    }

    private double calculateEccentricAnomaly(Vector2 eccentricity, double trueAnomaly, bool towardsPerigee, OrbitTypes orbitType)
    {
        double returnEccentricAnomaly = 0;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnEccentricAnomaly = Math.Acos((eccentricity.magnitude + Math.Cos(trueAnomaly)) /
                (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly))));
                if (towardsPerigee)
                {
                    returnEccentricAnomaly = -returnEccentricAnomaly;
                }
                break;
            case OrbitTypes.elliptical:
                returnEccentricAnomaly = Math.Acos((eccentricity.magnitude + Math.Cos(trueAnomaly)) /
                        (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly))));
                if (towardsPerigee)
                {
                    returnEccentricAnomaly = -returnEccentricAnomaly;
                }
                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                double x = (eccentricity.magnitude + Math.Cos(trueAnomaly)) / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
                returnEccentricAnomaly = Math.Log(x + Math.Sqrt((x * x) - 1));
                if (trueAnomaly < 0)
                {
                    returnEccentricAnomaly = -Math.Abs(returnEccentricAnomaly);
                }
                else
                {
                    returnEccentricAnomaly = Math.Abs(returnEccentricAnomaly);
                }
                break;
        }



        return returnEccentricAnomaly;
    }

    private double calculateTrueAnomaly(Vector2 eccentricity, Vector2 position, bool towardsPerigee, bool clockwise, OrbitTypes orbitType)
    {
        double returnTrueAnomaly = double.PositiveInfinity;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnTrueAnomaly = Vector2.Angle(position, Vector2.right);
                returnTrueAnomaly = convertToRadians(returnTrueAnomaly);
                break;
            case OrbitTypes.elliptical:
                returnTrueAnomaly = Vector2.Angle(eccentricity, position);
                returnTrueAnomaly = convertToRadians(returnTrueAnomaly);
                if (towardsPerigee)
                {
                    returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                }
                else
                {
                    returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                }

                break;
            case OrbitTypes.parabolic:
                break;
            case OrbitTypes.hyperbolic:
                returnTrueAnomaly = Vector2.Angle(eccentricity, position);
                returnTrueAnomaly = convertToRadians(returnTrueAnomaly);
                if (clockwise)
                {
                    if (towardsPerigee)
                    {
                        returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                    }
                    else
                    {
                        returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                    }
                }
                else
                {
                    if (towardsPerigee)
                    {
                        returnTrueAnomaly = -Math.Abs(returnTrueAnomaly);
                    }
                    else
                    {
                        returnTrueAnomaly = Math.Abs(returnTrueAnomaly);
                    }
                }
                break;
        }

        return returnTrueAnomaly;
    }

    private bool towardsPerigeeOrbit(Vector2 velocity, Vector2 eccentricity, OrbitTypes orbitType)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return (convertToRadians(Vector2.Angle(Vector2.right, velocity)) < Math.PI / 2);
            case OrbitTypes.elliptical:
                return (convertToRadians(Vector2.Angle(eccentricity, velocity)) < Math.PI / 2);
            case OrbitTypes.parabolic:
                return true;
            case OrbitTypes.hyperbolic:
                return (convertToRadians(Vector2.Angle(eccentricity, velocity)) < Math.PI / 2);
        }
        return true;

    }

    private bool clockwiseOrbit(Vector2 position, Vector2 velocity)
    {
        Vector3 crossProduct;
        crossProduct = Vector3.Cross(convertToVec3(position), convertToVec3(velocity));
        if (crossProduct.z > 0)
        {
            return false;
        }
        return true;
    }

    private double calculateGlobalRotationAngle(Vector2 eccentricity, OrbitTypes orbitType)
    {
        double returnGlobalRotationAngle = 0;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                returnGlobalRotationAngle = 0;
                break;
            case OrbitTypes.elliptical:
                returnGlobalRotationAngle = Math.Atan2(eccentricity.y, eccentricity.x);
                break;
            case OrbitTypes.parabolic:
                returnGlobalRotationAngle = Math.Atan2(eccentricity.y, eccentricity.x);
                break;
            case OrbitTypes.hyperbolic:
                returnGlobalRotationAngle = Math.Atan2(eccentricity.y, eccentricity.x);
                break;
        }
        //conver to positive angle
        if (returnGlobalRotationAngle < 0)
        {
            returnGlobalRotationAngle = (Math.PI - Math.Abs(returnGlobalRotationAngle)) + Math.PI;
        }
        return returnGlobalRotationAngle;
    }

    private Vector2 calculateCenter(double semiMajorAxis, Vector2 perigee, OrbitTypes orbitType)
    {
        return -perigee.normalized * (float)semiMajorAxis + perigee;
    }

    private Vector2 calculatePerigee(double semiMajorAxis, Vector2 eccentricity, OrbitTypes orbitType)
    {
        double altitudeOfPerigee = 0;
        Vector2 returnPerigee = Vector2.right;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                altitudeOfPerigee = semiMajorAxis;
                returnPerigee = Vector2.right;
                returnPerigee = returnPerigee * (float)altitudeOfPerigee;
                break;
            case OrbitTypes.elliptical:
                altitudeOfPerigee = semiMajorAxis * (1 - eccentricity.magnitude);
                returnPerigee = eccentricity.normalized;
                returnPerigee = returnPerigee * (float)altitudeOfPerigee;
                break;
            case OrbitTypes.parabolic:
                altitudeOfPerigee = -semiMajorAxis * (eccentricity.magnitude - 1);
                returnPerigee = eccentricity.normalized * (float)altitudeOfPerigee;
                break;
            case OrbitTypes.hyperbolic:
                altitudeOfPerigee = -semiMajorAxis * (eccentricity.magnitude - 1);
                returnPerigee = eccentricity.normalized * (float)altitudeOfPerigee;
                break;
        }
        return returnPerigee;
    }

    private Vector2 calculateApogee(double semiMajorAxis, Vector2 eccentricity, OrbitTypes orbitType)
    {
        double altitudeOfApogee = 0;
        Vector2 returnApogee = Vector2.right;
        switch (orbitType)
        {
            case OrbitTypes.circular:
                altitudeOfApogee = semiMajorAxis;
                returnApogee = Vector2.right;
                returnApogee = -returnApogee * (float)altitudeOfApogee;
                break;
            case OrbitTypes.elliptical:
                altitudeOfApogee = semiMajorAxis * (1 + eccentricity.magnitude);
                returnApogee = -eccentricity.normalized;
                returnApogee = returnApogee * (float)altitudeOfApogee;
                break;
            case OrbitTypes.parabolic:
                altitudeOfApogee = double.PositiveInfinity;
                returnApogee = -Vector2.right * float.PositiveInfinity;
                break;
            case OrbitTypes.hyperbolic:
                altitudeOfApogee = double.PositiveInfinity;
                returnApogee = -Vector2.right * float.PositiveInfinity;
                break;
        }
        return returnApogee;
    }

    private double calculateSemiLatusRectum(double semiMajorAxis, Vector2 eccentricity, Vector2 perigee, OrbitTypes orbitType)
    {
        if (orbitType != OrbitTypes.parabolic)
        {
            return semiMajorAxis * (1 - Vector2.SqrMagnitude(eccentricity));
        }
        else
        {
            return 2 * perigee.magnitude;
        }
    }

    private double calculateSemiMajorAxis(double mechanicalEnergy, double mu, OrbitTypes orbitType)
    {
        switch (orbitType)
        {
            case OrbitTypes.circular:
                return -((mu) / (2 * mechanicalEnergy));
            case OrbitTypes.elliptical:
                return -((mu) / (2 * mechanicalEnergy));
            case OrbitTypes.parabolic:
                return double.PositiveInfinity;
            case OrbitTypes.hyperbolic:
                return -((mu) / (2 * mechanicalEnergy));
        }
        return double.NegativeInfinity;
    }

    private double calculateMechanicalEnergy(Vector2 position, Vector2 velocity, double mu, OrbitTypes orbitType)
    {
        return (Vector2.SqrMagnitude(velocity) / 2) - ((mu) / position.magnitude);
    }

    private OrbitTypes determineOrbitType(Vector2 eccentricity)
    {
        if (eccentricity.magnitude == 0)
        {
            return OrbitTypes.circular;
        }
        else if (eccentricity.magnitude < 1
          && eccentricity.magnitude > 0)
        {
            return OrbitTypes.elliptical;
        }
        else if (eccentricity.magnitude == 1.0f)
        {
            return OrbitTypes.parabolic;
        }
        else if (eccentricity.magnitude > 1)
        {
            return OrbitTypes.hyperbolic;
        }

        //Shouled never reach this
        return OrbitTypes.error;
    }

    private Vector2 calculateEccentricity(Vector2 inPosition, Vector2 inVelocity, double inMu)
    {
        return ((inPosition * (float)(Vector2.SqrMagnitude(inVelocity) - ((inMu) / inPosition.magnitude))) - ((float)(Vector2.Dot(inPosition, inVelocity)) * inVelocity)) *
            (float)(1 / (inMu));
    }

    //HELPER FUNCTIONS
    private double wrapAngle(double angleToWrap)
    {
        if (angleToWrap > Math.PI)
        {
            angleToWrap -= Math.PI;
            angleToWrap = -Math.PI + angleToWrap;
        }

        if (angleToWrap < -Math.PI)
        {
            angleToWrap += Math.PI;
            angleToWrap = Math.Abs(angleToWrap);
            angleToWrap = Math.PI - angleToWrap;
        }

        return angleToWrap;
    }

    private double convertToRadians(double degrees)
    {
        return (degrees * Math.PI) / 180;
    }

    private Vector3 convertToVec3(Vector2 inVec)
    {
        return new Vector3(inVec.x, inVec.y, 0.0f);
    }

    private Vector2 convertToVec2(Vector3 inVec)
    {
        return new Vector2(inVec.x, inVec.y);
    }

    public void OnDrawGizmos()
    {
        if (debugMode && gravityElements != null)
        {
            //Draw perigee
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(gravityElements.GlobalTransformationVector + gravityElements.Perigee, 0.1f);

            //draw apogee
            Gizmos.DrawSphere(gravityElements.GlobalTransformationVector + gravityElements.Apogee, 0.1f);

            //Draw semiMajor Axis
            Gizmos.color = Color.red;
            Gizmos.DrawLine(gravityElements.GlobalTransformationVector + gravityElements.Perigee, gravityElements.GlobalTransformationVector + gravityElements.Perigee + (gravityElements.Perigee.normalized * -(float)gravityElements.SemiMajorAxis));

            //Draw eccentricity
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(gravityElements.GlobalTransformationVector, gravityElements.GlobalTransformationVector + gravityElements.Eccentricity);

            //Draw the ellipse
            double angle = 0;
            while (angle < 2 * Math.PI)
            {
                Gizmos.DrawLine(drawOrbitRadiusHelper(gravityElements.Eccentricity, gravityElements.GlobalRotationAngle, gravityElements.SemiMajorAxis, angle) + gravityElements.GlobalTransformationVector,
                    drawOrbitRadiusHelper(gravityElements.Eccentricity, gravityElements.GlobalRotationAngle, gravityElements.SemiMajorAxis, angle + 0.01d) + gravityElements.GlobalTransformationVector);
                angle += 0.01d;
            }

            //Draw Center
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(gravityElements.GlobalTransformationVector + gravityElements.Center, 0.2f);

            //Draw position
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(gravityElements.Position + gravityElements.GlobalTransformationVector, 0.1f);
            Gizmos.DrawLine(gravityElements.GlobalTransformationVector, gravityElements.GlobalTransformationVector + gravityElements.Position);

            //Draw velocity
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(gravityElements.GlobalTransformationVector + gravityElements.Position, gravityElements.GlobalTransformationVector + gravityElements.Position + gravityElements.velocity );

            //Draw mean anomaly
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gravityElements.GlobalTransformationVector, gravityElements.GlobalTransformationVector + new Vector2((float)Math.Cos(gravityElements.MeanAnomaly + gravityElements.GlobalRotationAngle), (float)Math.Sin(gravityElements.MeanAnomaly + gravityElements.GlobalRotationAngle)) * 10f);
        }
    }

    private Vector2 drawOrbitRadiusHelper(Vector2 eccentricity, double globalRotationAngle, double semiMajorAxis, double angle)
    {
        double radius = (semiMajorAxis * (1 - Vector2.SqrMagnitude(eccentricity))) / (1 - eccentricity.magnitude * (Math.Cos(angle - globalRotationAngle + Math.PI)));
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (float)radius;
    }
}
