﻿using UnityEngine;
using System.Collections;

public class GravityElements : MonoBehaviour
{

    public void copyGravityElementsClass(GravityElementsClass gravElements)
    {
        this.velocity = gravElements.velocity;
        this.massiveBody = gravElements.massiveBody;
        this.gravitationalType = gravElements.gravitationalType;

        this.altitude = gravElements.Altitude;
        this.speed = gravElements.Speed;
        this.velocityAngle = gravElements.VelocityAngle;

        this.orbitType = gravElements.OrbitType;
        this.mu = gravElements.Mu;

        this.position = gravElements.Position;
        this.apogee = gravElements.Apogee;
        this.perigee = gravElements.Perigee;
        this.center = gravElements.Center;
        this.altitudeOfPerigee = gravElements.AltitudeOfPerigee;
        this.angularMomentum = gravElements.AngularMomentum;

        this.semiMajorAxis = gravElements.SemiMajorAxis;
        this.semiLatusRectum = gravElements.SemiLatusRectum;

        this.eccentricity = gravElements.Eccentricity;
        this.eccentricityMagnitude = gravElements.EccentricityMagnitude;
        this.eccentricAnomaly = gravElements.EccentricAnomaly;
        this.trueAnomaly = gravElements.TrueAnomaly;
        this.meanAnomaly = gravElements.MeanAnomaly;
        this.anomalyAtEpoch = gravElements.AnomalyAtEpoch;
        this.mechanicalEnergy = gravElements.MechanicalEnergy;

        this.time = gravElements.Time;
        this.timeStep = gravElements.TimeStep;
        this.timeAtEpoch = gravElements.TimeAtEpoch;

        this.clockwise = gravElements.Clockwise;
        this.towardsPerigee = gravElements.TowardsPerigee;

        this.globalTransformationVector = gravElements.GlobalTransformationVector;
        this.globalRotationAngle = gravElements.GlobalRotationAngle;
    }

    public GravityElements()
    {

    }

    /**
    *   All vectors are relative to 0,0 and then transformed 
    *   based on the location of the celestial body. Velocity 
    *   is always calculated relative to the body it orbits.
    */

    public Vector2 velocity;
    public GameObject massiveBody;
    public GravitationalType gravitationalType;

    private double altitude;
    private double speed;
    private double velocityAngle;

    private OrbitTypes orbitType;
    private double mu;

    private Vector2 position;
    private Vector2 apogee;
    private Vector2 perigee;
    private Vector2 center;
    private double altitudeOfPerigee;
    private double angularMomentum;

    private double semiMajorAxis;
    private double semiLatusRectum;

    private Vector2 eccentricity;
    private double eccentricityMagnitude;
    private double eccentricAnomaly;
    private double trueAnomaly;
    private double meanAnomaly;
    private double anomalyAtEpoch;
    private double mechanicalEnergy;

    private double time;
    private double timeStep;
    private double timeAtEpoch;

    private bool clockwise;
    private bool towardsPerigee;

    private Vector2 globalTransformationVector;
    private double globalRotationAngle;

    public double Altitude
    {
        get
        {
            return altitude;
        }

        set
        {
            altitude = value;
        }
    }

    public double Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    public double VelocityAngle
    {
        get
        {
            return velocityAngle;
        }

        set
        {
            velocityAngle = value;
        }
    }

    public OrbitTypes OrbitType
    {
        get
        {
            return orbitType;
        }

        set
        {
            orbitType = value;
        }
    }

    public double Mu
    {
        get
        {
            return mu;
        }

        set
        {
            mu = value;
        }
    }

    public Vector2 Apogee
    {
        get
        {
            return apogee;
        }

        set
        {
            apogee = value;
        }
    }

    public Vector2 Perigee
    {
        get
        {
            return perigee;
        }

        set
        {
            perigee = value;
        }
    }

    public Vector2 Center
    {
        get
        {
            return center;
        }

        set
        {
            center = value;
        }
    }

    public double AltitudeOfPerigee
    {
        get
        {
            return altitudeOfPerigee;
        }

        set
        {
            altitudeOfPerigee = value;
        }
    }

    public double AngularMomentum
    {
        get
        {
            return angularMomentum;
        }

        set
        {
            angularMomentum = value;
        }
    }

    public double SemiMajorAxis
    {
        get
        {
            return semiMajorAxis;
        }

        set
        {
            semiMajorAxis = value;
        }
    }

    public double SemiLatusRectum
    {
        get
        {
            return semiLatusRectum;
        }

        set
        {
            semiLatusRectum = value;
        }
    }

    public Vector2 Eccentricity
    {
        get
        {
            return eccentricity;
        }

        set
        {
            eccentricity = value;
        }
    }

    public double EccentricityMagnitude
    {
        get
        {
            return eccentricityMagnitude;
        }

        set
        {
            eccentricityMagnitude = value;
        }
    }

    public double EccentricAnomaly
    {
        get
        {
            return eccentricAnomaly;
        }

        set
        {
            eccentricAnomaly = value;
        }
    }

    public double TrueAnomaly
    {
        get
        {
            return trueAnomaly;
        }

        set
        {
            trueAnomaly = value;
        }
    }

    public double MeanAnomaly
    {
        get
        {
            return meanAnomaly;
        }

        set
        {
            meanAnomaly = value;
        }
    }

    public double AnomalyAtEpoch
    {
        get
        {
            return anomalyAtEpoch;
        }

        set
        {
            anomalyAtEpoch = value;
        }
    }

    public double MechanicalEnergy
    {
        get
        {
            return mechanicalEnergy;
        }

        set
        {
            mechanicalEnergy = value;
        }
    }

    public double Time
    {
        get
        {
            return time;
        }

        set
        {
            time = value;
        }
    }

    public double TimeStep
    {
        get
        {
            return timeStep;
        }

        set
        {
            timeStep = value;
        }
    }

    public double TimeAtEpoch
    {
        get
        {
            return timeAtEpoch;
        }

        set
        {
            timeAtEpoch = value;
        }
    }

    public bool Clockwise
    {
        get
        {
            return clockwise;
        }

        set
        {
            clockwise = value;
        }
    }

    public bool TowardsPerigee
    {
        get
        {
            return towardsPerigee;
        }

        set
        {
            towardsPerigee = value;
        }
    }

    public Vector2 GlobalTransformationVector
    {
        get
        {
            return globalTransformationVector;
        }

        set
        {
            globalTransformationVector = value;
        }
    }

    public double GlobalRotationAngle
    {
        get
        {
            return globalRotationAngle;
        }

        set
        {
            globalRotationAngle = value;
        }
    }

    public GameObject MassiveBody
    {
        get
        {
            return massiveBody;
        }

        set
        {
            massiveBody = value;
        }
    }

    public Vector2 Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
        }
    }

    //Calculate Global position at future time, cannot be used for ship
    public Tuple<Vector2, Vector2> calculateGlobalPositionAndVelocityAtFutureTime(double timeStep)
    {
        Tuple<Vector2, Vector2> returnInfo;

        Vector2 returnPosition;
        Vector2 returnVelocity;

        switch (gravitationalType)
        {
            case GravitationalType.black_hole:
                returnPosition = new Vector2(0, 0);
                returnVelocity = new Vector2(0, 0);
                return new Tuple<Vector2, Vector2>(returnPosition, returnVelocity);
            case GravitationalType.star:
                returnInfo = calculateLocalPositionAndVelocityAtFutureTime(timeStep);
                return returnInfo;
            case GravitationalType.planet:
                returnInfo = calculateLocalPositionAndVelocityAtFutureTime(timeStep);
                returnPosition = returnInfo.item1;
                returnVelocity = returnInfo.item2;
                returnInfo = this.massiveBody.GetComponent<GravityElements>().calculateLocalPositionAndVelocityAtFutureTime(timeStep);
                returnPosition += returnInfo.item1;
                returnVelocity += returnInfo.item2;
                return new Tuple<Vector2, Vector2>(returnPosition, returnVelocity);
            case GravitationalType.ship:
                Debug.LogWarning("You're using this function wrong.... dumbass...");
                return new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(0, 0));
            default:
                Debug.LogWarning("This code should be inaccessable.");
                return new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(0, 0));
        }
    }

    //Calculate Global position at future time, cannot be used for ship
    public Vector2 calculateGlobalPositionAtFutureTime(double timeStep)
    {
        switch (gravitationalType)
        {
            case GravitationalType.black_hole:
                return new Vector2(0, 0);
            case GravitationalType.star:
                return calculateLocalPositionAtFutureTime(timeStep);
            case GravitationalType.planet:
                return this.massiveBody.GetComponent<GravityElements>().calculateLocalPositionAtFutureTime(timeStep) + calculateLocalPositionAtFutureTime(timeStep);
            case GravitationalType.ship:
                Debug.LogWarning("You're using this function wrong.... dumbass...");
                return new Vector2(0, 0);
            default:
                Debug.LogWarning("This code should be inaccessable.");
                return new Vector2(0, 0);
        }
    }

    //This behavior is shared by all gravity bound objects
    public Vector2 calculateLocalPositionAtFutureTime(double timeStep)
    {
        //Adjust tranformation vector
        Vector2 globalTransformationVector = massiveBody.transform.position;

        //Calculate time at epoch
        double timeAtEpoch = OrbitalHelper.advanceTime(this.timeAtEpoch, timeStep, clockwise, orbitType);

        //Calculate next meanAnomaly
        double meanAnomaly = OrbitalHelper.calculateMeanAnomaly(eccentricity, semiMajorAxis, anomalyAtEpoch,
            timeStep, timeAtEpoch, clockwise, mu, orbitType);

        //Calculate Eccentric Anomaly
        double eccentricAnomaly = OrbitalHelper.calculateEccentricAnomaly(eccentricity, semiMajorAxis, GlobalElements.GRAV_CONST, timeStep, timeAtEpoch,
            meanAnomaly, this.eccentricAnomaly, mu, clockwise, orbitType);

        //CalculateTrueAnomaly
        double trueAnomaly = OrbitalHelper.calculateTrueAnomaly(eccentricity, eccentricAnomaly, meanAnomaly, orbitType);

        //Calculate Altitude
        double altitude = OrbitalHelper.calculateAltitude(eccentricity, semiMajorAxis, semiLatusRectum, trueAnomaly, orbitType);

        //Calculate positionVector
        Vector2 position = OrbitalHelper.calculatePosition(perigee, trueAnomaly, globalRotationAngle, altitude, orbitType);

        return position;
    }

    public Tuple<Vector2, Vector2> calculateLocalPositionAndVelocityAtFutureTime(double timeStep)
    {
        //Calculate time at epoch
        double timeAtEpoch = OrbitalHelper.advanceTime(this.timeAtEpoch, timeStep, clockwise, orbitType);

        //Calculate next meanAnomaly
        double meanAnomaly = OrbitalHelper.calculateMeanAnomaly(eccentricity, semiMajorAxis, anomalyAtEpoch,
            timeStep, timeAtEpoch, clockwise, mu, orbitType);

        //Calculate Eccentric Anomaly
        double eccentricAnomaly = OrbitalHelper.calculateEccentricAnomaly(eccentricity, semiMajorAxis, GlobalElements.GRAV_CONST, timeStep, timeAtEpoch,
            meanAnomaly, this.eccentricAnomaly, mu, clockwise, orbitType);

        //CalculateTrueAnomaly
        double trueAnomaly = OrbitalHelper.calculateTrueAnomaly(eccentricity, eccentricAnomaly, meanAnomaly, orbitType);

        //Calculate Altitude
        double altitude = OrbitalHelper.calculateAltitude(eccentricity, semiMajorAxis, semiLatusRectum, trueAnomaly, orbitType);

        //Calculate positionVector
        Vector2 position = OrbitalHelper.calculatePosition(perigee, trueAnomaly, globalRotationAngle, altitude, orbitType);

        //Are we going towards the perigee?
        bool towardsPerigee = OrbitalHelper.towardsPerigeeOrbit(meanAnomaly, clockwise);

        //Calculate velocity angle
        double velocityAngle = OrbitalHelper.calculateVelocityAngle(position, eccentricity, semiMajorAxis,
            trueAnomaly, globalRotationAngle, clockwise, towardsPerigee, orbitType);

        //Calculate Speed
        double speed = OrbitalHelper.calculateSpeed(position, semiMajorAxis, mu, orbitType);

        //Calculate Velocity
        Vector2 velocity = OrbitalHelper.assembleVelocityVector(velocityAngle, speed);

        //Im returning the position here, you know, just in case you couldnt figure it out on your own
        return new Tuple<Vector2, Vector2>(position, velocity);
    }

    public GravityElementsClass getClassVersion()
    {
        GravityElementsClass returnGravityElements = new GravityElementsClass();

        returnGravityElements.velocity = this.velocity;
        returnGravityElements.massiveBody = this.massiveBody;
        returnGravityElements.gravitationalType = this.gravitationalType;

        returnGravityElements.Altitude = this.Altitude;
        returnGravityElements.Speed = this.Speed;
        returnGravityElements.VelocityAngle = this.VelocityAngle;

        returnGravityElements.OrbitType = this.OrbitType;
        returnGravityElements.Mu = this.Mu;

        returnGravityElements.Position = this.Position;
        returnGravityElements.Apogee = this.Apogee;
        returnGravityElements.Perigee = this.Perigee;
        returnGravityElements.Center = this.Center;
        returnGravityElements.AltitudeOfPerigee = this.AltitudeOfPerigee;
        returnGravityElements.AngularMomentum = this.AngularMomentum;

        returnGravityElements.SemiMajorAxis = this.SemiMajorAxis;
        returnGravityElements.SemiLatusRectum = this.SemiLatusRectum;

        returnGravityElements.Eccentricity = this.Eccentricity;
        returnGravityElements.EccentricityMagnitude = this.EccentricityMagnitude;
        returnGravityElements.EccentricAnomaly = this.EccentricAnomaly;
        returnGravityElements.TrueAnomaly = this.TrueAnomaly;
        returnGravityElements.MeanAnomaly = this.MeanAnomaly;
        returnGravityElements.AnomalyAtEpoch = this.AnomalyAtEpoch;
        returnGravityElements.MechanicalEnergy = this.MechanicalEnergy;

        returnGravityElements.Time = this.Time;
        returnGravityElements.TimeStep = this.TimeStep;
        returnGravityElements.TimeAtEpoch = this.TimeAtEpoch;

        returnGravityElements.Clockwise = this.Clockwise;
        returnGravityElements.TowardsPerigee = this.TowardsPerigee;

        returnGravityElements.GlobalTransformationVector = this.GlobalTransformationVector;
        returnGravityElements.GlobalRotationAngle = this.GlobalRotationAngle;

        return returnGravityElements;
    }

    public void advanceToFutureState(double time)
    {
        //update timestep
        this.TimeStep = time;

        //Adjust tranformation vector
        this.GlobalTransformationVector = this.MassiveBody.transform.position;

        //Calculate next meanAnomaly
        this.MeanAnomaly = OrbitalHelper.calculateMeanAnomaly(this.Eccentricity, this.SemiMajorAxis, this.AnomalyAtEpoch,
            this.TimeStep, this.TimeAtEpoch, this.Clockwise, this.Mu, this.OrbitType);

        //Calculate Eccentric Anomaly
        this.EccentricAnomaly = OrbitalHelper.calculateEccentricAnomaly(this.Eccentricity, this.SemiMajorAxis, GlobalElements.GRAV_CONST, this.TimeStep, this.TimeAtEpoch,
            this.MeanAnomaly, this.EccentricAnomaly, this.Mu, this.Clockwise, this.OrbitType);

        //CalculateTrueAnomaly
        this.TrueAnomaly = OrbitalHelper.calculateTrueAnomaly(this.Eccentricity, this.EccentricAnomaly, this.MeanAnomaly, this.OrbitType);

        //Calculate Altitude
        this.Altitude = OrbitalHelper.calculateAltitude(this.Eccentricity, this.SemiMajorAxis, this.SemiLatusRectum, this.TrueAnomaly, this.OrbitType);

        //Calculate positionVector
        this.Position = OrbitalHelper.calculatePosition(this.Perigee, this.TrueAnomaly, this.GlobalRotationAngle, this.Altitude, this.OrbitType);

        //Are we going towards the perigee?
        this.TowardsPerigee = OrbitalHelper.towardsPerigeeOrbit(this.MeanAnomaly, this.Clockwise);

        //Calculate velocity angle
        this.VelocityAngle = OrbitalHelper.calculateVelocityAngle(this.Position, this.Eccentricity, this.SemiMajorAxis,
            this.TrueAnomaly, this.GlobalRotationAngle, this.Clockwise, this.TowardsPerigee, this.OrbitType);

        //Calculate Speed
        this.Speed = OrbitalHelper.calculateSpeed(this.Position, this.SemiMajorAxis, this.Mu, this.OrbitType);

        //Calculate Velocity
        this.velocity = OrbitalHelper.assembleVelocityVector(this.VelocityAngle, this.Speed);

        //advance epoch
        this.AnomalyAtEpoch = this.MeanAnomaly;

        //Advance time
        this.TimeAtEpoch = OrbitalHelper.advanceTime(this.TimeAtEpoch, this.TimeStep, this.Clockwise, this.OrbitType);

    }
}













public class GravityElementsClass
{

    public GravityElementsClass()
    {

    }

    public GravityElementsClass(Vector2 velocity, Vector2 position, GravityElements gravElements)
    {
        copyGravityElementsComponent(gravElements);
        this.velocity = velocity;
        this.position = position;
    }

    private void copyGravityElementsComponent(GravityElements gravElements)
    {
        this.velocity = gravElements.velocity;
        this.massiveBody = gravElements.massiveBody;
        this.gravitationalType = gravElements.gravitationalType;

        this.altitude = gravElements.Altitude;
        this.speed = gravElements.Speed;
        this.velocityAngle = gravElements.VelocityAngle;

        this.orbitType = gravElements.OrbitType;
        this.mu = gravElements.Mu;

        this.position = gravElements.Position;
        this.apogee = gravElements.Apogee;
        this.perigee = gravElements.Perigee;
        this.center = gravElements.Center;
        this.altitudeOfPerigee = gravElements.AltitudeOfPerigee;
        this.angularMomentum = gravElements.AngularMomentum;

        this.semiMajorAxis = gravElements.SemiMajorAxis;
        this.semiLatusRectum = gravElements.SemiLatusRectum;

        this.eccentricity = gravElements.Eccentricity;
        this.eccentricityMagnitude = gravElements.EccentricityMagnitude;
        this.eccentricAnomaly = gravElements.EccentricAnomaly;
        this.trueAnomaly = gravElements.TrueAnomaly;
        this.meanAnomaly = gravElements.MeanAnomaly;
        this.anomalyAtEpoch = gravElements.AnomalyAtEpoch;
        this.mechanicalEnergy = gravElements.MechanicalEnergy;

        this.time = gravElements.Time;
        this.timeStep = gravElements.TimeStep;
        this.timeAtEpoch = gravElements.TimeAtEpoch;

        this.clockwise = gravElements.Clockwise;
        this.towardsPerigee = gravElements.TowardsPerigee;

        this.globalTransformationVector = gravElements.GlobalTransformationVector;
        this.globalRotationAngle = gravElements.GlobalRotationAngle;
    }

    /**
    *   All vectors are relative to 0,0 and then transformed 
    *   based on the location of the celestial body. Velocity 
    *   is always calculated relative to the body it orbits.
    */
    
    public Vector2 velocity;
    public GameObject massiveBody;
    public GravitationalType gravitationalType;

    private double altitude;
    private double speed;
    private double velocityAngle;

    private OrbitTypes orbitType;
    private double mu;

    private Vector2 position;
    private Vector2 apogee;
    private Vector2 perigee;
    private Vector2 center;
    private double altitudeOfPerigee;
    private double angularMomentum;

    private double semiMajorAxis;
    private double semiLatusRectum;

    private Vector2 eccentricity;
    private double eccentricityMagnitude;
    private double eccentricAnomaly;
    private double trueAnomaly;
    private double meanAnomaly;
    private double anomalyAtEpoch;
    private double mechanicalEnergy;

    private double time;
    private double timeStep;
    private double timeAtEpoch;

    private bool clockwise;
    private bool towardsPerigee;

    private Vector2 globalTransformationVector;
    private double globalRotationAngle;


    public double Altitude
    {
        get
        {
            return altitude;
        }

        set
        {
            altitude = value;
        }
    }

    public double Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    public double VelocityAngle
    {
        get
        {
            return velocityAngle;
        }

        set
        {
            velocityAngle = value;
        }
    }

    public OrbitTypes OrbitType
    {
        get
        {
            return orbitType;
        }

        set
        {
            orbitType = value;
        }
    }

    public double Mu
    {
        get
        {
            return mu;
        }

        set
        {
            mu = value;
        }
    }

    public Vector2 Apogee
    {
        get
        {
            return apogee;
        }

        set
        {
            apogee = value;
        }
    }

    public Vector2 Perigee
    {
        get
        {
            return perigee;
        }

        set
        {
            perigee = value;
        }
    }

    public Vector2 Center
    {
        get
        {
            return center;
        }

        set
        {
            center = value;
        }
    }

    public double AltitudeOfPerigee
    {
        get
        {
            return altitudeOfPerigee;
        }

        set
        {
            altitudeOfPerigee = value;
        }
    }

    public double AngularMomentum
    {
        get
        {
            return angularMomentum;
        }

        set
        {
            angularMomentum = value;
        }
    }

    public double SemiMajorAxis
    {
        get
        {
            return semiMajorAxis;
        }

        set
        {
            semiMajorAxis = value;
        }
    }

    public double SemiLatusRectum
    {
        get
        {
            return semiLatusRectum;
        }

        set
        {
            semiLatusRectum = value;
        }
    }

    public Vector2 Eccentricity
    {
        get
        {
            return eccentricity;
        }

        set
        {
            eccentricity = value;
        }
    }

    public double EccentricityMagnitude
    {
        get
        {
            return eccentricityMagnitude;
        }

        set
        {
            eccentricityMagnitude = value;
        }
    }

    public double EccentricAnomaly
    {
        get
        {
            return eccentricAnomaly;
        }

        set
        {
            eccentricAnomaly = value;
        }
    }

    public double TrueAnomaly
    {
        get
        {
            return trueAnomaly;
        }

        set
        {
            trueAnomaly = value;
        }
    }

    public double MeanAnomaly
    {
        get
        {
            return meanAnomaly;
        }

        set
        {
            meanAnomaly = value;
        }
    }

    public double AnomalyAtEpoch
    {
        get
        {
            return anomalyAtEpoch;
        }

        set
        {
            anomalyAtEpoch = value;
        }
    }

    public double MechanicalEnergy
    {
        get
        {
            return mechanicalEnergy;
        }

        set
        {
            mechanicalEnergy = value;
        }
    }

    public double Time
    {
        get
        {
            return time;
        }

        set
        {
            time = value;
        }
    }

    public double TimeStep
    {
        get
        {
            return timeStep;
        }

        set
        {
            timeStep = value;
        }
    }

    public double TimeAtEpoch
    {
        get
        {
            return timeAtEpoch;
        }

        set
        {
            timeAtEpoch = value;
        }
    }

    public bool Clockwise
    {
        get
        {
            return clockwise;
        }

        set
        {
            clockwise = value;
        }
    }

    public bool TowardsPerigee
    {
        get
        {
            return towardsPerigee;
        }

        set
        {
            towardsPerigee = value;
        }
    }

    public Vector2 GlobalTransformationVector
    {
        get
        {
            return globalTransformationVector;
        }

        set
        {
            globalTransformationVector = value;
        }
    }

    public double GlobalRotationAngle
    {
        get
        {
            return globalRotationAngle;
        }

        set
        {
            globalRotationAngle = value;
        }
    }

    public GameObject MassiveBody
    {
        get
        {
            return massiveBody;
        }

        set
        {
            massiveBody = value;
        }
    }

    public Vector2 Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
        }
    }

    //Calculate Global position at future time, cannot be used for ship
    public Tuple<Vector2, Vector2> calculateGlobalPositionAndVelocityAtFutureTime(double timeStep)
    {
        Tuple<Vector2, Vector2> returnInfo;

        Vector2 returnPosition;
        Vector2 returnVelocity;

        switch (gravitationalType)
        {
            case GravitationalType.black_hole:
                returnPosition = new Vector2(0, 0);
                returnVelocity = new Vector2(0, 0);
                return new Tuple<Vector2, Vector2>(returnPosition, returnVelocity);
            case GravitationalType.star:
                returnInfo = calculateLocalPositionAndVelocityAtFutureTime(timeStep);
                return returnInfo;
            case GravitationalType.planet:
                returnInfo = calculateLocalPositionAndVelocityAtFutureTime(timeStep);
                returnPosition = returnInfo.item1;
                returnVelocity = returnInfo.item2;
                returnInfo = this.massiveBody.GetComponent<GravityElements>().calculateLocalPositionAndVelocityAtFutureTime(timeStep);
                returnPosition += returnInfo.item1;
                returnVelocity += returnInfo.item2;
                return new Tuple<Vector2, Vector2>(returnPosition, returnVelocity);
            case GravitationalType.ship:
                Debug.LogWarning("You're using this function wrong.... dumbass...");
                return new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(0, 0));
            default:
                Debug.LogWarning("This code should be inaccessable.");
                return new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(0, 0));
        }
    }

    //Calculate Global position at future time, cannot be used for ship
    public Vector2 calculateGlobalPositionAtFutureTime(double timeStep)
    {
        switch (gravitationalType)
        {
            case GravitationalType.black_hole:
                return new Vector2(0, 0);
            case GravitationalType.star:
                return calculateLocalPositionAtFutureTime(timeStep);
            case GravitationalType.planet:
                return this.massiveBody.GetComponent<GravityElements>().calculateLocalPositionAtFutureTime(timeStep) + calculateLocalPositionAtFutureTime(timeStep);
            case GravitationalType.ship:
                Debug.LogWarning("You're using this function wrong.... dumbass...");
                return new Vector2(0, 0);
            default:
                Debug.LogWarning("This code should be inaccessable.");
                return new Vector2(0, 0);
        }
    }

    //This behavior is shared by all gravity bound objects
    public Vector2 calculateLocalPositionAtFutureTime(double timeStep)
    {

        //Calculate time at epoch
        double timeAtEpoch = OrbitalHelper.advanceTime(this.timeAtEpoch, timeStep, clockwise, orbitType);

        //Calculate next meanAnomaly
        double meanAnomaly = OrbitalHelper.calculateMeanAnomaly(eccentricity, semiMajorAxis, anomalyAtEpoch,
            timeStep, timeAtEpoch, clockwise, mu, orbitType);

        //Calculate Eccentric Anomaly
        double eccentricAnomaly = OrbitalHelper.calculateEccentricAnomaly(eccentricity, semiMajorAxis, GlobalElements.GRAV_CONST, timeStep, timeAtEpoch,
            meanAnomaly, this.eccentricAnomaly, mu, clockwise, orbitType);

        //CalculateTrueAnomaly
        double trueAnomaly = OrbitalHelper.calculateTrueAnomaly(eccentricity, eccentricAnomaly, meanAnomaly, orbitType);

        //Calculate Altitude
        double altitude = OrbitalHelper.calculateAltitude(eccentricity, semiMajorAxis, semiLatusRectum, trueAnomaly, orbitType);

        //Calculate positionVector
        Vector2 position = OrbitalHelper.calculatePosition(perigee, trueAnomaly, globalRotationAngle, altitude, orbitType);

        return position;
    }

    public Tuple<Vector2, Vector2> calculateLocalPositionAndVelocityAtFutureTime(double timeStep)
    {
        //Calculate time at epoch
        double timeAtEpoch = OrbitalHelper.advanceTime(this.timeAtEpoch, timeStep, clockwise, orbitType);

        //Calculate next meanAnomaly
        double meanAnomaly = OrbitalHelper.calculateMeanAnomaly(eccentricity, semiMajorAxis, anomalyAtEpoch,
            timeStep, timeAtEpoch, clockwise, mu, orbitType);

        //Calculate Eccentric Anomaly
        double eccentricAnomaly = OrbitalHelper.calculateEccentricAnomaly(eccentricity, semiMajorAxis, GlobalElements.GRAV_CONST, timeStep, timeAtEpoch,
            meanAnomaly, this.eccentricAnomaly, mu, clockwise, orbitType);

        //CalculateTrueAnomaly
        double trueAnomaly = OrbitalHelper.calculateTrueAnomaly(eccentricity, eccentricAnomaly, meanAnomaly, orbitType);

        //Calculate Altitude
        double altitude = OrbitalHelper.calculateAltitude(eccentricity, semiMajorAxis, semiLatusRectum, trueAnomaly, orbitType);

        //Calculate positionVector
        Vector2 position = OrbitalHelper.calculatePosition(perigee, trueAnomaly, globalRotationAngle, altitude, orbitType);

        //Are we going towards the perigee?
        bool towardsPerigee = OrbitalHelper.towardsPerigeeOrbit(meanAnomaly, clockwise);

        //Calculate velocity angle
        double velocityAngle = OrbitalHelper.calculateVelocityAngle(position, eccentricity, semiMajorAxis,
            trueAnomaly, globalRotationAngle, clockwise, towardsPerigee, orbitType);

        //Calculate Speed
        double speed = OrbitalHelper.calculateSpeed(position, semiMajorAxis, mu, orbitType);

        //Calculate Velocity
        Vector2 velocity = OrbitalHelper.assembleVelocityVector(velocityAngle, speed);

        //Im returning the position here, you know, just in case you couldnt figure it out on your own
        return new Tuple<Vector2, Vector2>(position, velocity);
    }

    public void advanceToFutureState(double time)
    {
        //update timestep
        this.TimeStep = time;

        //Adjust tranformation vector
        this.GlobalTransformationVector = this.MassiveBody.transform.position;

        //Calculate next meanAnomaly
        this.MeanAnomaly = OrbitalHelper.calculateMeanAnomaly(this.Eccentricity, this.SemiMajorAxis, this.AnomalyAtEpoch,
            this.TimeStep, this.TimeAtEpoch, this.Clockwise, this.Mu, this.OrbitType);

        //Calculate Eccentric Anomaly
        this.EccentricAnomaly = OrbitalHelper.calculateEccentricAnomaly(this.Eccentricity, this.SemiMajorAxis, GlobalElements.GRAV_CONST, this.TimeStep, this.TimeAtEpoch,
            this.MeanAnomaly, this.EccentricAnomaly, this.Mu, this.Clockwise, this.OrbitType);

        //CalculateTrueAnomaly
        this.TrueAnomaly = OrbitalHelper.calculateTrueAnomaly(this.Eccentricity, this.EccentricAnomaly, this.MeanAnomaly, this.OrbitType);

        //Calculate Altitude
        this.Altitude = OrbitalHelper.calculateAltitude(this.Eccentricity, this.SemiMajorAxis, this.SemiLatusRectum, this.TrueAnomaly, this.OrbitType);

        //Calculate positionVector
        this.Position = OrbitalHelper.calculatePosition(this.Perigee, this.TrueAnomaly, this.GlobalRotationAngle, this.Altitude, this.OrbitType);

        //Are we going towards the perigee?
        this.TowardsPerigee = OrbitalHelper.towardsPerigeeOrbit(this.MeanAnomaly, this.Clockwise);

        //Calculate velocity angle
        this.VelocityAngle = OrbitalHelper.calculateVelocityAngle(this.Position, this.Eccentricity, this.SemiMajorAxis,
            this.TrueAnomaly, this.GlobalRotationAngle, this.Clockwise, this.TowardsPerigee, this.OrbitType);

        //Calculate Speed
        this.Speed = OrbitalHelper.calculateSpeed(this.Position, this.SemiMajorAxis, this.Mu, this.OrbitType);

        //Calculate Velocity
        this.velocity = OrbitalHelper.assembleVelocityVector(this.VelocityAngle, this.Speed);

        //advance epoch
        this.AnomalyAtEpoch = this.MeanAnomaly;

        //Advance time
        this.TimeAtEpoch = OrbitalHelper.advanceTime(this.TimeAtEpoch, this.TimeStep, this.Clockwise, this.OrbitType);

    }
}