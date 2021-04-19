using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class CarDriverAgent : Agent
{
    //Field for inspector
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private Transform spawnPosition;

    //general
    private CarDriver carDriver;
    private Vector3 originalPosition;
    private BehaviorParameters behaviorParameters;
    private Rigidbody rBody;

    private void Start()
    {
        carDriver = GetComponent<CarDriver>();
        rBody = GetComponent<Rigidbody>();
        trackCheckpoints.OnCarCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        trackCheckpoints.OnCarWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
    }

    //give positive reward if car is on correct checkpoint
    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, TrackCheckpoints.CarCheckPointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            Debug.Log("Positive reward!");
            AddReward(1f);
        }
    }
    //give negative reward if car is on wrong checkpoint
    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, TrackCheckpoints.CarCheckPointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            Debug.Log("negative reward!");
            AddReward(-1f);
        }
    }

    //begin episode
    public override void OnEpisodeBegin()
    {
        transform.position = spawnPosition.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        transform.forward = spawnPosition.forward;
        trackCheckpoints.ResetCheckpoint();
        carDriver.StopCompletely();
        MaxStep = MaxStep + 10;
    }

    //add observations to sensor for decision making
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 checkpointForward = trackCheckpoints.GetNextCheckpoint(transform).transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkpointForward);
        sensor.AddObservation(directionDot);
    }

    //Handles actions received from the agent
    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;

        int forwardAction = actions.DiscreteActions[0]; //actions for acceleration
        int turnAction = actions.DiscreteActions[1];    //actions for turning


        //set forwardamount and turnamount based on discreteactions
        switch (forwardAction)
        {
            case 0: forwardAmount = 0f; break;  //do nothing
            case 1: forwardAmount = 1f; break;  //accelerate forward
            case 2: forwardAmount = -1f; break; //accelerate backward
        }
        switch (turnAction)
        {
            case 0: turnAmount = 0f; break;     //do nothing
            case 1: turnAmount = 1f; break;     //turn right
            case 2: turnAmount = -1f; break;    //turn left
        }

        carDriver.SetInputs(forwardAmount, turnAmount);
        //AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionOut)
    {
        //drive forward or backward
        int forwardAction = 0;
        if (Input.GetKey(KeyCode.UpArrow)) forwardAction = 1; 
        if (Input.GetKey(KeyCode.DownArrow)) forwardAction = 2;
        
        //turn left of right
        int turnAction = 0;
        if (Input.GetKey(KeyCode.RightArrow)) turnAction = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) turnAction = 2;
        
        ActionSegment<int> discreteActions = actionOut.DiscreteActions;
        discreteActions[0] = forwardAction;
        discreteActions[1] = turnAction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            //Debug.Log("negative reward: -0.5f");
            //hit a wall
            AddReward(-0.5f);
            EndEpisode();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            //Debug.Log("negative reward: -0.1f");
            //hit a wall
            AddReward(-0.1f);           
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
