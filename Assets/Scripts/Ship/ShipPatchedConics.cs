using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

/**
*Must be executed after the preupdate
**/

public class ShipPatchedConics : MonoBehaviour
{

    //Data sources
    private GravityElements shipElements;
    private Encounters encounters;

    //Other behaviors
    private ShipGravityBehavior ship;
    private SpriteRenderer spriteRenderer;
    private LineDrawer lineDrawer;
    private NodeManager nodeManager;

    //Orbital POI
    public Button perigeeButton;
    public Image perigeePlacard;
    public Button apogeeButton;
    public Image apogeePlacard;

    private bool onEscapeTrajectory;
    public GameObject encounterIconPrefab;
    private int maxSphereChanges;
    private int currentSphereChanges;

    private bool instantiated;
    private int iterations;
    private int MAX_ITERATIONS = 3;

    void Start()
    {
        shipElements = GetComponent<GravityElements>();
        encounters = GetComponent<Encounters>();

        ship = GetComponent<ShipGravityBehavior>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        lineDrawer = GetComponentInChildren<LineDrawer>();
        nodeManager = GetComponent<NodeManager>();

        updateEncounters();
    }

    void FixedUpdate()
    {
        //update encounter transformation vectors
        foreach (Encounter encounter in encounters.predictedEncounters)
        {
            encounter.GravElements.GlobalTransformationVector = encounter.GravElements.massiveBody.transform.position;
        }

        foreach (Encounter encounter in encounters.maneuverEncounters)
        {
            encounter.GravElements.GlobalTransformationVector = encounter.GravElements.massiveBody.transform.position;
        }

        //Display ships patched conics
        int i = 0;
        foreach(Encounter encounter in encounters.predictedEncounters)
        {
            drawPatchedConics(encounter, Color.red);
        }

        foreach (Encounter encounter in encounters.maneuverEncounters)
        {
            drawPatchedConics(encounter, Color.cyan);
        }
        
    }

    private Vector2 drawOrbitRadiusHelper(Vector2 eccentricity, double globalRotationAngle, double semiMajorAxis, double trueAnomaly)
    {
        trueAnomaly = MiscHelperFuncs.convertTo360Angle(trueAnomaly);
        double radius = (semiMajorAxis * (1 - eccentricity.sqrMagnitude)) / (1 + (eccentricity.magnitude * Math.Cos(trueAnomaly)));
        return (float)radius * new Vector2((float)Math.Cos(trueAnomaly + globalRotationAngle), (float)Math.Sin(trueAnomaly + globalRotationAngle));
        
    }

    public void drawPatchedConics(Encounter encounter, Color color)
    {
        //No other encounters
        
        double fastAngle = Math.PI;
        while (fastAngle > -Math.PI)
        {
            lineDrawer.DrawLine(drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, fastAngle) + encounter.GravElements.GlobalTransformationVector,
                drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, fastAngle - 0.01) + encounter.GravElements.GlobalTransformationVector, color);
            fastAngle -= 0.01;
        }
    }

    //Finds all encounters within one orbit of the craft
    public void updateEncounters()
    {
        
        encounters.predictedEncounters.Clear();

        //initial setup
        GravityElementsClass currentShipsGravityElements = shipElements.getClassVersion();
        GravityElementsClass currentMassiveBodyGravityElements = shipElements.massiveBody.GetComponent<GravityElements>().getClassVersion();
        MassiveBodyElements currentMassiveBody = shipElements.massiveBody.GetComponent<MassiveBodyElements>();

        iterations = 0;

        predictAnEncounter(currentShipsGravityElements, currentMassiveBodyGravityElements, currentMassiveBody);
        
    }

    private void predictAnEncounter(GravityElementsClass currentShipsGravityElements, GravityElementsClass currentMassiveBodyGravityElements, MassiveBodyElements currentMassiveBodyElements)
    {
        iterations++;

        //will we ever encounter anything?
        double time = 0;
        double orbitalPeriod = calculatePeriod(currentShipsGravityElements.OrbitType, currentShipsGravityElements.SemiMajorAxis, currentShipsGravityElements.Mu);
        List<GameObject> satelites = currentMassiveBodyElements.satelites;
        GameObject encounteredMassiveBody = null;
        while(time < orbitalPeriod && encounteredMassiveBody == null)
        {
            time += 0.01;
            //are we going to exit this soi?
            if(currentShipsGravityElements.calculateLocalPositionAtFutureTime(time).magnitude > currentMassiveBodyElements.SphereOfInfluence)
            {
                encounteredMassiveBody = currentMassiveBodyGravityElements.massiveBody;
                break;
            }

            //Are we going to enter a new soi?
            foreach (GameObject satelite in satelites)
            {
                Vector2 satelitesFuturePosition = satelite.GetComponent<GravityElements>().calculateLocalPositionAtFutureTime(time);
                Vector2 shipsFuturePosition = currentShipsGravityElements.calculateLocalPositionAtFutureTime(time);
                if(Vector2.Distance(satelitesFuturePosition, shipsFuturePosition) < satelite.GetComponent<MassiveBodyElements>().SphereOfInfluence)
                {
                    encounteredMassiveBody = satelite;
                    break;
                }
            }

            //Are we far enough that we're probably not going to run into anything soon?
            if(time > 10000)
            {
                break;
            }
        }

        
        //no new encounters
        if (encounteredMassiveBody == null)
        {
            Encounter currentEncounter = new Encounter(currentShipsGravityElements, currentShipsGravityElements.TrueAnomaly, double.PositiveInfinity); //always add the current encounter
            encounters.predictedEncounters.Enqueue(currentEncounter);
        }

        //Found an encounter!
        else
        {
            Encounter currentEncounter = new Encounter(currentShipsGravityElements, currentShipsGravityElements.TrueAnomaly, double.PositiveInfinity); //always add the current encounter
            encounters.predictedEncounters.Enqueue(currentEncounter);

            Tuple<Vector2, Vector2> returnInfo = encounteredMassiveBody.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time);
            Vector2 encounteredMassiveBodyPredictedGlobalPosition = returnInfo.item1;
            Vector2 encounteredMassiveBodyPredictedGlobalVelocity = returnInfo.item2;

            returnInfo = currentShipsGravityElements.calculateLocalPositionAndVelocityAtFutureTime(time);
            Vector2 shipPredictedLocalPosition = returnInfo.item1;
            Vector2 shipPredictedLocalVelocity = returnInfo.item2;

            returnInfo = currentMassiveBodyGravityElements.calculateGlobalPositionAndVelocityAtFutureTime(time);
            Vector2 currentMassiveBodyPredictedGlobalPosition = returnInfo.item1;
            Vector2 currentMassiveBodyPredictedGlobalVelcity = returnInfo.item2;

            Vector2 shipPredictedEncounterLocalPosition = (shipPredictedLocalPosition + currentMassiveBodyPredictedGlobalPosition) - encounteredMassiveBodyPredictedGlobalPosition;
            Vector2 shipPredictedEncounterLocalVelocity = (shipPredictedLocalVelocity + currentMassiveBodyPredictedGlobalVelcity) - encounteredMassiveBodyPredictedGlobalVelocity;
            Vector2 predictedGlobalTransformationVector = encounteredMassiveBodyPredictedGlobalPosition;
            

            GravityElementsClass newShipsGravityElements = calculateInitialOrbitalElements(shipPredictedEncounterLocalPosition, shipPredictedEncounterLocalVelocity, encounteredMassiveBody);
            GravityElementsClass newMassiveBodysGravityElements = newShipsGravityElements.massiveBody.GetComponent<GravityElements>().getClassVersion();
            returnInfo = encounteredMassiveBody.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time);
            newMassiveBodysGravityElements.Position = returnInfo.item1;
            newMassiveBodysGravityElements.velocity = returnInfo.item2;
            MassiveBodyElements newMassiveBody = newShipsGravityElements.massiveBody.GetComponent<MassiveBodyElements>();
            foreach(GameObject satelite in newMassiveBody.satelites)
            {
                returnInfo = satelite.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time);
                satelite.GetComponent<GravityElements>().Position = returnInfo.item1;
                satelite.GetComponent<GravityElements>().velocity = returnInfo.item2;
            }

            Debug.Log("-------------------");
            Debug.Log("Encounter info dump");
            Debug.Log("Eccentricity: " + newShipsGravityElements.Eccentricity);
            Debug.Log("Mechanical Energy: " + newShipsGravityElements.MechanicalEnergy);
            Debug.Log("SemiMajor Axis: " + newShipsGravityElements.SemiMajorAxis);
            Debug.Log("SemiLatus Rectum: " + newShipsGravityElements.SemiLatusRectum);
            Debug.Log("Perigee: " + newShipsGravityElements.Perigee);
            Debug.Log("Apogee: " + newShipsGravityElements.Apogee);
            Debug.Log("Center: " + newShipsGravityElements.Center);
            Debug.Log("Global Rotation Angle: " + newShipsGravityElements.GlobalRotationAngle);
            Debug.Log("Clockwise: " + newShipsGravityElements.Clockwise);
            Debug.Log("Towards Perigee: " + newShipsGravityElements.TowardsPerigee);
            Debug.Log("TrueAnomaly: " + newShipsGravityElements.TrueAnomaly);
            Debug.Log("EccentricAnomaly: " + newShipsGravityElements.EccentricAnomaly);
            Debug.Log("Anomaly At Epoch: " + newShipsGravityElements.AnomalyAtEpoch);
            Debug.Log("Mean Anomaly: " + newShipsGravityElements.MeanAnomaly);
            Debug.Log("Angular Momentum: " + newShipsGravityElements.AngularMomentum);
            Debug.Log("Time At Epoch: " + newShipsGravityElements.TimeAtEpoch);

            if (iterations < MAX_ITERATIONS)
            {
                predictAnEncounter(newShipsGravityElements, newMassiveBodysGravityElements, newMassiveBody);
            }
        }
    }

    private void predictAManeuver(GravityElementsClass currentShipsGravityElements, GravityElementsClass currentMassiveBodyGravityElements, MassiveBodyElements currentMassiveBodyElements)
    {
        iterations++;

        //will we ever encounter anything?
        double time = 0;
        double orbitalPeriod = calculatePeriod(currentShipsGravityElements.OrbitType, currentShipsGravityElements.SemiMajorAxis, currentShipsGravityElements.Mu);
        List<GameObject> satelites = currentMassiveBodyElements.satelites;
        GameObject encounteredMassiveBody = null;
        while (time < orbitalPeriod && encounteredMassiveBody == null)
        {
            time += 0.01;
            //are we going to exit this soi?
            if (currentShipsGravityElements.calculateLocalPositionAtFutureTime(time).magnitude > currentMassiveBodyElements.SphereOfInfluence)
            {
                encounteredMassiveBody = currentMassiveBodyGravityElements.massiveBody;
                break;
            }

            //Are we going to enter a new soi?
            foreach (GameObject satelite in satelites)
            {
                Vector2 satelitesFuturePosition = satelite.GetComponent<GravityElements>().calculateLocalPositionAtFutureTime(time);
                Vector2 shipsFuturePosition = currentShipsGravityElements.calculateLocalPositionAtFutureTime(time);
                if (Vector2.Distance(satelitesFuturePosition, shipsFuturePosition) < satelite.GetComponent<MassiveBodyElements>().SphereOfInfluence)
                {
                    encounteredMassiveBody = satelite;
                    break;
                }
            }

            //Are we far enough that we're probably not going to run into anything soon?
            if (time > 10000)
            {
                break;
            }
        }


        //no new encounters
        if (encounteredMassiveBody == null)
        {
            Encounter currentEncounter = new Encounter(currentShipsGravityElements, currentShipsGravityElements.TrueAnomaly, double.PositiveInfinity); //always add the current encounter
            encounters.maneuverEncounters.Enqueue(currentEncounter);
        }

        //Found an encounter!
        else
        {
            Encounter currentEncounter = new Encounter(currentShipsGravityElements, currentShipsGravityElements.TrueAnomaly, double.PositiveInfinity); //always add the current encounter
            encounters.maneuverEncounters.Enqueue(currentEncounter);

            Tuple<Vector2, Vector2> returnInfo = encounteredMassiveBody.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time);
            Vector2 encounteredMassiveBodyPredictedGlobalPosition = returnInfo.item1;
            Vector2 encounteredMassiveBodyPredictedGlobalVelocity = returnInfo.item2;

            returnInfo = currentShipsGravityElements.calculateLocalPositionAndVelocityAtFutureTime(time);
            Vector2 shipPredictedLocalPosition = returnInfo.item1;
            Vector2 shipPredictedLocalVelocity = returnInfo.item2;

            returnInfo = currentMassiveBodyGravityElements.calculateGlobalPositionAndVelocityAtFutureTime(time);
            Vector2 currentMassiveBodyPredictedGlobalPosition = returnInfo.item1;
            Vector2 currentMassiveBodyPredictedGlobalVelcity = returnInfo.item2;

            Vector2 shipPredictedEncounterLocalPosition = (shipPredictedLocalPosition + currentMassiveBodyPredictedGlobalPosition) - encounteredMassiveBodyPredictedGlobalPosition;
            Vector2 shipPredictedEncounterLocalVelocity = (shipPredictedLocalVelocity + currentMassiveBodyPredictedGlobalVelcity) - encounteredMassiveBodyPredictedGlobalVelocity;
            Vector2 predictedGlobalTransformationVector = encounteredMassiveBodyPredictedGlobalPosition;


            GravityElementsClass newShipsGravityElements = calculateInitialOrbitalElements(shipPredictedEncounterLocalPosition, shipPredictedEncounterLocalVelocity, encounteredMassiveBody);
            GravityElementsClass newMassiveBodysGravityElements = newShipsGravityElements.massiveBody.GetComponent<GravityElements>().getClassVersion();
            returnInfo = encounteredMassiveBody.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time);
            newMassiveBodysGravityElements.Position = returnInfo.item1;
            newMassiveBodysGravityElements.velocity = returnInfo.item2;
            MassiveBodyElements newMassiveBody = newShipsGravityElements.massiveBody.GetComponent<MassiveBodyElements>();
            foreach (GameObject satelite in newMassiveBody.satelites)
            {
                returnInfo = satelite.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time);
                satelite.GetComponent<GravityElements>().Position = returnInfo.item1;
                satelite.GetComponent<GravityElements>().velocity = returnInfo.item2;
            }

            Debug.Log("-------------------");
            Debug.Log("Encounter info dump");
            Debug.Log("Eccentricity: " + newShipsGravityElements.Eccentricity);
            Debug.Log("Mechanical Energy: " + newShipsGravityElements.MechanicalEnergy);
            Debug.Log("SemiMajor Axis: " + newShipsGravityElements.SemiMajorAxis);
            Debug.Log("SemiLatus Rectum: " + newShipsGravityElements.SemiLatusRectum);
            Debug.Log("Perigee: " + newShipsGravityElements.Perigee);
            Debug.Log("Apogee: " + newShipsGravityElements.Apogee);
            Debug.Log("Center: " + newShipsGravityElements.Center);
            Debug.Log("Global Rotation Angle: " + newShipsGravityElements.GlobalRotationAngle);
            Debug.Log("Clockwise: " + newShipsGravityElements.Clockwise);
            Debug.Log("Towards Perigee: " + newShipsGravityElements.TowardsPerigee);
            Debug.Log("TrueAnomaly: " + newShipsGravityElements.TrueAnomaly);
            Debug.Log("EccentricAnomaly: " + newShipsGravityElements.EccentricAnomaly);
            Debug.Log("Anomaly At Epoch: " + newShipsGravityElements.AnomalyAtEpoch);
            Debug.Log("Mean Anomaly: " + newShipsGravityElements.MeanAnomaly);
            Debug.Log("Angular Momentum: " + newShipsGravityElements.AngularMomentum);
            Debug.Log("Time At Epoch: " + newShipsGravityElements.TimeAtEpoch);

            if (iterations < MAX_ITERATIONS)
            {
                predictAManeuver(newShipsGravityElements, newMassiveBodysGravityElements, newMassiveBody);
            }
        }
    }

    public void updatePotentialEncounters(Vector2 maneuverPosition, Vector2 maneuverVelocity)
    {
        if (encounters.maneuverEncounters == null)
        {
            return;
        }

        GravityElementsClass currentEncounterGravityElements = calculateInitialOrbitalElements(maneuverPosition, maneuverVelocity, shipElements.massiveBody);
        
        encounters.maneuverEncounters.Clear();
        
        //initial setup
        GravityElementsClass currentShipsGravityElements = currentEncounterGravityElements;
        GravityElementsClass currentMassiveBodyGravityElements = shipElements.massiveBody.GetComponent<GravityElements>().getClassVersion();
        MassiveBodyElements currentMassiveBody = shipElements.massiveBody.GetComponent<MassiveBodyElements>();

        iterations = 0;

        predictAManeuver(currentShipsGravityElements, currentMassiveBodyGravityElements, currentMassiveBody);




    }

    public void clearPotentialEncounters()
    {
        encounters.maneuverEncounters.Clear();
    }

    private double calculatePeriod(OrbitTypes orbitType, double semiMajorAxis, double mu)
    {
        switch (orbitType)
        {
            case OrbitTypes.hyperbolic:
                return double.PositiveInfinity;
            case OrbitTypes.parabolic:
                return double.PositiveInfinity;
            default:
                double period = Math.Sqrt((4 * Math.Pow(Math.PI, 2) * Math.Pow(semiMajorAxis, 3)) / mu);
                return period;

        }
    }

    //takes in everything relative to the body being orbited
    private GravityElementsClass calculateInitialOrbitalElements(Vector2 position, Vector2 velocity, GameObject massiveBody)
    {
        GravityElementsClass gravityElements = new GravityElementsClass();

        gravityElements.massiveBody = massiveBody;

        gravityElements.Mu = GlobalElements.GRAV_CONST * gravityElements.MassiveBody.GetComponent<MassiveBodyElements>().mass;
        gravityElements.Position = position;
        gravityElements.velocity = velocity;

        //Calculate Global Tranformation Vector
        gravityElements.GlobalTransformationVector = massiveBody.transform.position;

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
        
        return gravityElements;
    }
}
