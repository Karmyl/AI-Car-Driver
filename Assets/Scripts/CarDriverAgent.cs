﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class CarDriverAgent : Agent
{

    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private Transform spawnPosition;

    private CarDriver carDriver;
    private Vector3 originalPosition;
    private BehaviorParameters behaviorParameters;
    private Rigidbody carDriverRigiBody;

    private void Start()
    {
        carDriver = GetComponent<CarDriver>();
    }
    void Initialize()
    {        
        trackCheckpoints.OnCarCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        trackCheckpoints.OnCarWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, TrackCheckpoints.CarCheckPointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            Debug.Log("Positive reward!");
            AddReward(1f);
        }
    }
    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, TrackCheckpoints.CarCheckPointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            Debug.Log("negative reward!");
            AddReward(-1f);
        }
    }
    public override void OnEpisodeBegin()
    {
        transform.position = spawnPosition.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        transform.forward = spawnPosition.forward;
        trackCheckpoints.ResetCheckpoint(transform);
        //carDriver.StopCompletely();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 checkpointForward = trackCheckpoints.GetNextCheckpoint(transform).transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkpointForward);
        //Debug.Log(directionDot + " dd");
        sensor.AddObservation(directionDot);
    }

    public override void OnActionReceived(float[] actions)
    {

        float forwardAmount = 0f;
        float turnAmount = 0f;
        int forwardAction = Mathf.FloorToInt(actions[0]);
        int turnAction = Mathf.FloorToInt(actions[1]);

        Debug.Log(forwardAction + ", forward");
        Debug.Log(turnAction + ", turn");
        //Debug.Log(actions.ContinuousActions[0] + ", continuous");
        switch (forwardAction)
        {
            case 0: forwardAmount = 0f; break;
            case 1: forwardAmount = 1f; break;
            case 2: forwardAmount = -1f; break;
        }
        switch (turnAction)
        {
            case 0: turnAmount = 0f; break;
            case 1: turnAmount = 1f; break;
            case 2: turnAmount = -1f; break;
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
            Debug.Log("Collision with wlal");
            //hit a wall
            SetReward(-0.5f);
            EndEpisode();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            //hit a wall
            AddReward(-0.1f);
            
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        RequestDecision();
    }
}
