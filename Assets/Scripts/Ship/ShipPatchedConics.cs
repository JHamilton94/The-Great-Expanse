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
    private SpriteRenderer spriteRenderer;
    private LineDrawer lineDrawer;

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

        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        lineDrawer = GetComponentInChildren<LineDrawer>();

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
        foreach (Encounter encounter in encounters.predictedEncounters)
        {
            drawPatchedConics(encounter, Color.red);
            displayOrbitalPOI(encounter, Color.red);
        }

        foreach (Encounter encounter in encounters.maneuverEncounters)
        {
            drawPatchedConics(encounter, Color.cyan);
            displayOrbitalPOI(encounter, Color.red);
        }

    }

    private void displayOrbitalPOI(Encounter encounter, Color color)
    {

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
        if (encounter.EndingTrueAnomaly == double.PositiveInfinity)
        {
            double fastAngle = Math.PI;
            while (fastAngle > -Math.PI)
            {
                lineDrawer.DrawLine(drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, fastAngle) + encounter.GravElements.GlobalTransformationVector,
                    drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, fastAngle - 0.01) + encounter.GravElements.GlobalTransformationVector, color);
                fastAngle -= 0.01;
            }
        }
        else
        {
            double angle = encounter.StartingTrueAnomaly;
            bool breakout = false;
            switch (encounter.GravElements.Clockwise)
            {
                case true:
                    while (angle > -Math.PI && !breakout)
                    {
                        if (encounter.StartingTrueAnomaly > encounter.EndingTrueAnomaly && angle < encounter.EndingTrueAnomaly)
                        {
                            breakout = true;
                            break;
                        }
                        lineDrawer.DrawLine(drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, angle) + encounter.GravElements.GlobalTransformationVector,
                            drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, angle - 0.01) + encounter.GravElements.GlobalTransformationVector, color);
                        angle -= 0.01;
                    }
                    angle = Math.PI;
                    while (angle > encounter.StartingTrueAnomaly && !breakout)
                    {
                        if (angle < encounter.EndingTrueAnomaly)
                        {
                            breakout = true;
                            break;
                        }
                        lineDrawer.DrawLine(drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, angle) + encounter.GravElements.GlobalTransformationVector,
                            drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, angle - 0.01) + encounter.GravElements.GlobalTransformationVector, color);
                        angle -= 0.01;
                    }
                    break;
                case false:
                    while (angle < Math.PI && !breakout)
                    {
                        if (encounter.StartingTrueAnomaly < encounter.EndingTrueAnomaly && angle > encounter.EndingTrueAnomaly)
                        {
                            breakout = true;
                            break;
                        }
                        lineDrawer.DrawLine(drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, angle) + encounter.GravElements.GlobalTransformationVector,
                            drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, angle + 0.01) + encounter.GravElements.GlobalTransformationVector, color);
                        angle += 0.01;
                    }
                    angle = -Math.PI;
                    while (angle < encounter.StartingTrueAnomaly && !breakout)
                    {
                        if (angle > encounter.EndingTrueAnomaly)
                        {
                            breakout = true;
                            break;
                        }
                        lineDrawer.DrawLine(drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, angle) + encounter.GravElements.GlobalTransformationVector,
                            drawOrbitRadiusHelper(encounter.GravElements.Eccentricity, encounter.GravElements.GlobalRotationAngle, encounter.GravElements.SemiMajorAxis, angle + 0.01) + encounter.GravElements.GlobalTransformationVector, color);
                        angle += 0.01;
                    }
                    break;
            }
        }
    }

    //Finds all encounters within one orbit of the craft
    public void updateEncounters()
    {

        encounters.predictedEncounters.Clear();

        //initial setup
        GravityElementsClass currentShipsGravityElements = shipElements.getClassVersion();
        
        double startingTrueAnomaly = shipElements.TrueAnomaly;

        Encounter currentEncounter = new Encounter(currentShipsGravityElements, startingTrueAnomaly, double.PositiveInfinity, 0);

        iterations = 0;

        predictAnEncounter(currentEncounter, ref encounters.predictedEncounters);
    }

    //Takes in the ships current gravity elements according to its last encounter, current massive body elements as they are at tiem 0 and current massivebodyelements as they are at time 0 
    private void predictAnEncounter(Encounter currentEncounter, ref List<Encounter> encounters)
    {

        encounters.Add(currentEncounter);

        //data sources
        MassiveBodyElements currentMassiveBody = currentEncounter.GravElements.massiveBody.GetComponent<MassiveBodyElements>();
        GravityElementsClass currentMassiveBodyGravElements = currentEncounter.GravElements.massiveBody.GetComponent<GravityElements>().getClassVersion();

        iterations++;
        double time = 0;
        double currentOrbitalPeriod = calculatePeriod(currentEncounter.GravElements.OrbitType, currentEncounter.GravElements.SemiMajorAxis, currentEncounter.GravElements.Mu);
        bool foundNewEncounter = false;
        GameObject encounteredMassiveBody = null;
        while (time < currentOrbitalPeriod && !foundNewEncounter)
        {
            time += 0.01;

            //have we exited the current soi?
            double altitude = currentEncounter.GravElements.calculateLocalPositionAtFutureTime(time).magnitude; //the ships massive body is always at time 0
            if (currentMassiveBody.SphereOfInfluence < altitude)
            {
                foundNewEncounter = true;
                encounteredMassiveBody = currentEncounter.GravElements.massiveBody.GetComponent<GravityElements>().massiveBody;
                break;
            }

            //have we entered a new soi?
            foreach (GameObject satelite in currentMassiveBody.satelites)
            {
                
                double sateliteSphereOfInfluence = satelite.GetComponent<MassiveBodyElements>().SphereOfInfluence;
                Vector2 sateliteLocalPosition = satelite.GetComponent<GravityElements>().calculateLocalPositionAtFutureTime(time + currentEncounter.TimeOfEncounter);
                Vector2 shipsLocalPosition = currentEncounter.GravElements.calculateLocalPositionAtFutureTime(time);
                if (Vector2.Distance(shipsLocalPosition, sateliteLocalPosition) < sateliteSphereOfInfluence)
                {
                    foundNewEncounter = true;
                    encounteredMassiveBody = satelite;
                    break;
                }
            }

            if (time > 100) //hyperbolic orbit with infinite periode, this prevents infinite loops
            {
                break;
            }
        }

        //We have encountered nothing, just a regular eliptical orbit, or a hyperbolic one that's really long
        if (encounteredMassiveBody == null)
        {
            return;
        }

        //We have encountered a massive body, time to assemble a new encounter
        Tuple<Vector2, Vector2> returnInfo;

        returnInfo = currentEncounter.GravElements.calculateLocalPositionAndVelocityAtFutureTime(time);
        Vector2 shipPredictedLocalPosition = returnInfo.item1;
        Vector2 shipPredictedLocalVelocity = returnInfo.item2;

        returnInfo = currentMassiveBodyGravElements.calculateGlobalPositionAndVelocityAtFutureTime(time + currentEncounter.TimeOfEncounter);
        Vector2 currentMassiveBodyPredictedGlobalPosition = returnInfo.item1;
        Vector2 currentMassiveBodyPredictedGlobalVelocity = returnInfo.item2;

        returnInfo = encounteredMassiveBody.GetComponent<GravityElements>().calculateGlobalPositionAndVelocityAtFutureTime(time + currentEncounter.TimeOfEncounter);
        Vector2 encounteredMassiveBodyPredictedGlobalPosition = returnInfo.item1;
        Vector2 encounteredmassiveBodyPredictedGlobalVelocity = returnInfo.item2;
        
        Vector2 shipPositionRelativeToEncounter = (shipPredictedLocalPosition + currentMassiveBodyPredictedGlobalPosition) - encounteredMassiveBodyPredictedGlobalPosition;
        Vector2 shipVelocityRelativeToEncounter = (shipPredictedLocalVelocity + currentMassiveBodyPredictedGlobalVelocity) - encounteredmassiveBodyPredictedGlobalVelocity;

        GravityElementsClass newShipsGravityElements = calculateInitialOrbitalElements(shipPositionRelativeToEncounter, shipVelocityRelativeToEncounter, encounteredMassiveBody); //massive body is inputed in its state at time 0

        //Change the ending true anomaly of the last encounter
        double startingTrueAnomaly = newShipsGravityElements.TrueAnomaly;
        double endingTrueAnomaly = MiscHelperFuncs.AngleBetweenVector2(currentEncounter.GravElements.Eccentricity, shipPredictedLocalPosition);
            
        encounters[encounters.Count - 1].EndingTrueAnomaly = endingTrueAnomaly;
        
        Encounter newEncounter = new Encounter(newShipsGravityElements, startingTrueAnomaly, double.PositiveInfinity, time + currentEncounter.TimeOfEncounter);

        lineDrawer.DrawLine(newEncounter.GravElements.GlobalTransformationVector, newEncounter.GravElements.GlobalTransformationVector + shipPositionRelativeToEncounter, Color.green);
        lineDrawer.DrawLine(currentEncounter.GravElements.GlobalTransformationVector, currentEncounter.GravElements.GlobalTransformationVector + shipPredictedLocalPosition, Color.cyan);

        if (iterations < MAX_ITERATIONS)
        {
            predictAnEncounter(newEncounter, ref encounters);
        }
        
    }
    
    public void updatePotentialEncounters(GravityElementsClass newManeuver)
    {
        encounters.maneuverEncounters.Clear();

        //initial setup
        Encounter currentEncounter = new Encounter(newManeuver, newManeuver.TrueAnomaly, double.PositiveInfinity, 0);

        iterations = 0;

        predictAnEncounter(currentEncounter, ref encounters.maneuverEncounters);
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

    //takes in everything relative to  massiveBody, massive body is always in the state of time 0
    private GravityElementsClass calculateInitialOrbitalElements(Vector2 position, Vector2 velocity, GameObject massiveBody)
    {
        GravityElementsClass gravityElements = new GravityElementsClass();

        gravityElements.massiveBody = massiveBody;

        gravityElements.Mu = GlobalElements.GRAV_CONST * gravityElements.massiveBody.GetComponent<MassiveBodyElements>().mass;
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