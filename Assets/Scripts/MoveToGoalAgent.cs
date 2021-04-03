using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTranform;
    [SerializeField] private float moveSpeed = 1f;
    private Rigidbody rBody;

    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3.8f, 1.2f), -0.4f, Random.Range(-0.9f, 5.4f));
        targetTranform.localPosition = new Vector3(Random.Range(2.3f, 6.0f), -0.4f, Random.Range(-0.9f, 5.3f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //add positions to observation list 
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTranform.localPosition);

        //add speed to observation list
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collide happened!");
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(1f);
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

}

