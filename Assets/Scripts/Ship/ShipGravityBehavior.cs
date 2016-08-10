using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ShipGravityBehavior : MonoBehaviour
{

    public bool debugMode;
    public GravityElements gravityElements;
    private ShipPatchedConics shipPatchedConics;

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

        shipPatchedConics = GetComponent<ShipPatchedConics>();
        shipPatchedConics.updateEncounters();
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
        shipPatchedConics.updateEncounters();
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
            Vector2 relativePosition = position - MiscHelperFuncs.convertToVec2(massiveBodies[i].transform.position);
            Vector2 relativeVelocity = velocity + massiveBodies[i].GetComponent<GravityElements>().velocity;
            Vector2 eccentricity = OrbitalHelper.calculateEccentricity(relativePosition, relativeVelocity, mu);
            OrbitTypes orbitType = OrbitalHelper.determineOrbitType(eccentricity);
            double mechanicalEnergy = OrbitalHelper.calculateMechanicalEnergy(relativePosition, relativeVelocity, mu, orbitType);
            double semiMajorAxis = OrbitalHelper.calculateSemiMajorAxis(mechanicalEnergy, mu, orbitType);
            Vector2 perigee = OrbitalHelper.calculatePerigee(semiMajorAxis, eccentricity, orbitType);
            double semiLatusRectum = OrbitalHelper.calculateSemiLatusRectum(semiMajorAxis, eccentricity, perigee, orbitType); //semiMajorAxis * (1 - Math.Pow(eccentricity.magnitude, 2));
            double trueAnomaly = Vector2.Angle(relativePosition, eccentricity);
            trueAnomaly = MiscHelperFuncs.convertToRadians(trueAnomaly);
            trueAnomaly = Math.Abs(MiscHelperFuncs.wrapAngle(trueAnomaly));
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
            if (distance < smallestDistance)
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
        gravityElements.MeanAnomaly = OrbitalHelper.calculateMeanAnomaly(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, gravityElements.AnomalyAtEpoch,
            gravityElements.TimeStep, gravityElements.TimeAtEpoch, gravityElements.Clockwise, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate Eccentric Anomaly
        gravityElements.EccentricAnomaly = OrbitalHelper.calculateEccentricAnomaly(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, GlobalElements.GRAV_CONST, gravityElements.TimeStep, gravityElements.TimeAtEpoch,
            gravityElements.MeanAnomaly, gravityElements.EccentricAnomaly, gravityElements.Mu, gravityElements.Clockwise, gravityElements.OrbitType);

        //CalculateTrueAnomaly
        gravityElements.TrueAnomaly = OrbitalHelper.calculateTrueAnomaly(gravityElements.Eccentricity, gravityElements.EccentricAnomaly, gravityElements.MeanAnomaly, gravityElements.OrbitType);

        //Calculate Altitude
        gravityElements.Altitude = OrbitalHelper.calculateAltitude(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, gravityElements.SemiLatusRectum, gravityElements.TrueAnomaly, gravityElements.OrbitType);

        //Calculate positionVector
        gravityElements.Position = OrbitalHelper.calculatePosition(gravityElements.Perigee, gravityElements.TrueAnomaly, gravityElements.GlobalRotationAngle, gravityElements.Altitude, gravityElements.OrbitType);

        //Are we going towards the perigee?
        gravityElements.TowardsPerigee = OrbitalHelper.towardsPerigeeOrbit(gravityElements.MeanAnomaly, gravityElements.Clockwise, gravityElements.TimeAtEpoch, gravityElements.OrbitType);

        //Calculate velocity angle
        gravityElements.VelocityAngle = OrbitalHelper.calculateVelocityAngle(gravityElements.Position, gravityElements.Eccentricity, gravityElements.SemiMajorAxis,
            gravityElements.TrueAnomaly, gravityElements.GlobalRotationAngle, gravityElements.Clockwise, gravityElements.TowardsPerigee, gravityElements.OrbitType);

        //Calculate Speed
        gravityElements.Speed = OrbitalHelper.calculateSpeed(gravityElements.Position, gravityElements.SemiMajorAxis, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate Velocity
        gravityElements.velocity = OrbitalHelper.assembleVelocityVector(gravityElements.VelocityAngle, gravityElements.Speed);

        //advance epoch
        gravityElements.AnomalyAtEpoch = gravityElements.MeanAnomaly;

        //Advance time
        gravityElements.TimeAtEpoch = OrbitalHelper.advanceTime(gravityElements.TimeAtEpoch, gravityElements.TimeStep, gravityElements.Clockwise, gravityElements.OrbitType);


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
                calculateInitialOrbitalElements(position - MiscHelperFuncs.convertToVec2(newSphereOfInfluence.transform.position), velocity + currentMassiveBody.GetComponent<GravityElements>().velocity);
                sphereChangeImmunity = 10;
                shipPatchedConics.updateEncounters();
            }
            else //big to small
            {
                gravityElements.massiveBody = newSphereOfInfluence;
                calculateInitialOrbitalElements(position - MiscHelperFuncs.convertToVec2(newSphereOfInfluence.transform.position), velocity - newSphereOfInfluence.GetComponent<GravityElements>().velocity);
                sphereChangeImmunity = 10;
                shipPatchedConics.updateEncounters();
            }
        }
        else
        {
            sphereChangeImmunity--;
        }
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
        gravityElements.GlobalTransformationVector = OrbitalHelper.calculateGlobalTranformationVector(gravityElements.MassiveBody);

        //Calculate eccentricity
        gravityElements.Eccentricity = OrbitalHelper.calculateEccentricity(position, velocity, gravityElements.Mu);

        //Determine orbit type
        gravityElements.OrbitType = OrbitalHelper.determineOrbitType(gravityElements.Eccentricity);

        //Calculate Mechanical Energy
        gravityElements.MechanicalEnergy = OrbitalHelper.calculateMechanicalEnergy(gravityElements.Position, gravityElements.velocity, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate Semi Major Axis
        gravityElements.SemiMajorAxis = OrbitalHelper.calculateSemiMajorAxis(gravityElements.MechanicalEnergy, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate SemiLatusRectum
        gravityElements.SemiLatusRectum = OrbitalHelper.calculateSemiLatusRectum(gravityElements.SemiMajorAxis, gravityElements.Eccentricity, gravityElements.Perigee, gravityElements.OrbitType);

        //Calculate Perigee
        gravityElements.Perigee = OrbitalHelper.calculatePerigee(gravityElements.SemiMajorAxis, gravityElements.Eccentricity, gravityElements.OrbitType);

        //Calculate Apogee
        gravityElements.Apogee = OrbitalHelper.calculateApogee(gravityElements.SemiMajorAxis, gravityElements.Eccentricity, gravityElements.OrbitType);

        //Calculate Center
        gravityElements.Center = OrbitalHelper.calculateCenter(gravityElements.SemiMajorAxis, gravityElements.Perigee, gravityElements.OrbitType);

        //Calculate GlobalRotationAngle
        gravityElements.GlobalRotationAngle = OrbitalHelper.calculateGlobalRotationAngle(gravityElements.Eccentricity, gravityElements.OrbitType);

        //Find orbital directions
        gravityElements.Clockwise = OrbitalHelper.clockwiseOrbit(gravityElements.Position, gravityElements.velocity);
        gravityElements.TowardsPerigee = OrbitalHelper.towardsPerigeeOrbit(gravityElements.velocity, gravityElements.Eccentricity, gravityElements.OrbitType);

        //Calculate trueAnomaly
        gravityElements.TrueAnomaly = OrbitalHelper.calculateTrueAnomaly(gravityElements.Eccentricity, gravityElements.Position, gravityElements.TowardsPerigee, gravityElements.Clockwise, gravityElements.OrbitType);

        //Calculate Eccentric Anomaly
        gravityElements.EccentricAnomaly = OrbitalHelper.calculateEccentricAnomaly(gravityElements.Eccentricity, gravityElements.TrueAnomaly, gravityElements.TowardsPerigee, gravityElements.OrbitType);

        //Calculate Anomaly at current epoch
        gravityElements.AnomalyAtEpoch = OrbitalHelper.calculateAnomalyAtCurrentEpoch(gravityElements.Eccentricity, gravityElements.EccentricAnomaly, gravityElements.Clockwise, gravityElements.OrbitType);
        gravityElements.MeanAnomaly = gravityElements.AnomalyAtEpoch;

        //Calculate Angular Momentum
        gravityElements.AngularMomentum = OrbitalHelper.calculateAngularMomentum(gravityElements.Eccentricity, gravityElements.Perigee, gravityElements.SemiMajorAxis, gravityElements.SemiLatusRectum,
            gravityElements.Mu, gravityElements.OrbitType);

        //Calculate time at epoch
        gravityElements.TimeAtEpoch = OrbitalHelper.calculateTimeAtEpoch(gravityElements.Eccentricity, gravityElements.EccentricAnomaly, gravityElements.SemiMajorAxis, gravityElements.Mu,
            gravityElements.Clockwise, gravityElements.TowardsPerigee, gravityElements.OrbitType);

    }



    //HELPER FUNCTIONS


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
            Gizmos.DrawLine(gravityElements.GlobalTransformationVector + gravityElements.Position, gravityElements.GlobalTransformationVector + gravityElements.Position + gravityElements.velocity);

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

    public Vector2 calculatePositionAtFutureTime(double timeStep)
    {
        //Adjust tranformation vector
        Vector2 globalTransformationVector = gravityElements.MassiveBody.transform.position;

        //Calculate time at epoch
        double timeAtEpoch = OrbitalHelper.advanceTime(gravityElements.TimeAtEpoch, timeStep, gravityElements.Clockwise, gravityElements.OrbitType);

        //Calculate next meanAnomaly
        double meanAnomaly = OrbitalHelper.calculateMeanAnomaly(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, gravityElements.AnomalyAtEpoch,
            timeStep, timeAtEpoch, gravityElements.Clockwise, gravityElements.Mu, gravityElements.OrbitType);

        //Calculate Eccentric Anomaly
        double eccentricAnomaly = OrbitalHelper.calculateEccentricAnomaly(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, GlobalElements.GRAV_CONST, timeStep, timeAtEpoch,
            meanAnomaly, gravityElements.EccentricAnomaly, gravityElements.Mu, gravityElements.Clockwise, gravityElements.OrbitType);

        //CalculateTrueAnomaly
        double trueAnomaly = OrbitalHelper.calculateTrueAnomaly(gravityElements.Eccentricity, eccentricAnomaly, meanAnomaly, gravityElements.OrbitType);

        //Calculate Altitude
        double altitude = OrbitalHelper.calculateAltitude(gravityElements.Eccentricity, gravityElements.SemiMajorAxis, gravityElements.SemiLatusRectum, trueAnomaly, gravityElements.OrbitType);

        //Calculate positionVector
        Vector2 position = OrbitalHelper.calculatePosition(gravityElements.Perigee, trueAnomaly, gravityElements.GlobalRotationAngle, altitude, gravityElements.OrbitType);

        //Im returning the position here, you know, just in case you couldnt figure it out on your own
        return position;
    }
}