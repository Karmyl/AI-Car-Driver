using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{

    public event EventHandler OnCarCorrectCheckpoint;
    public event EventHandler OnCarWrongCheckpoint;

    [SerializeField] private List<Transform> carTransformList;

    private List<CheckpointSingle> checkpointSingleList;
    private List<int> nextCheckpointSingleIndexList;

    private void Start()
    {
        //Find checkpoints and format the list of single checkpoints
        Transform checkpointsTransform = transform.Find("Checkpoints");
        checkpointSingleList = new List<CheckpointSingle>();

        //Add checkpoints to checkpointsinglelist
        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
            checkpointSingleList.Add(checkpointSingle);
        }

        nextCheckpointSingleIndexList = new List<int>();
        //set the first checkpoint as the next correct checkpoint for each car in the scene
        foreach (Transform carTransform in carTransformList)
        {
            nextCheckpointSingleIndexList.Add(0);
        }
    }

    //Called when a car passes a checkpoint
    public void CarThroughCheckpoint(CheckpointSingle checkpointSingle, Transform carTransform)
    {
        CarCheckPointEventArgs e = new CarCheckPointEventArgs(carTransform);
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];

        //if the checkpoint matches the next correct checkpoint in the checkpointsinglelist -> invoke cardriveragents's oncarcorrectcheckpoint -method
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];

            nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)]
                = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;

            OnCarCorrectCheckpoint?.Invoke(this, e);
        }
        else //invoke cardriveragents's oncarwrongcheckpoint -method
        {
            // Wrong checkpoint
            Debug.Log("Wrong");
            OnCarWrongCheckpoint?.Invoke(this, e);
        }
    }

    //Get the next checkpoint for changing agents position
    internal CheckpointSingle GetNextCheckpoint(Transform transform)
    {
        int index = nextCheckpointSingleIndexList[carTransformList.IndexOf(transform)];
        return checkpointSingleList[index];
    }

    public void ResetCheckpoint()
    {
        checkpointSingleList.Clear();
        Transform checkpointsTransform = transform.Find("Checkpoints");

        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();

            checkpointSingle.SetTrackCheckpoints(this);

            checkpointSingleList.Add(checkpointSingle);
        }

        nextCheckpointSingleIndexList.Clear();
        foreach (Transform carTransform in carTransformList)
        {
            nextCheckpointSingleIndexList.Add(0);
        }

    }
    /**Nested custom class for handling the event for car passing checkpoints*/
    public class CarCheckPointEventArgs : EventArgs

    {
        public Transform carTransform;

        public CarCheckPointEventArgs(Transform carTransform)
        {
            this.carTransform = carTransform;
        }

        public Transform CarTransform()
        {
            return this.carTransform;
        }

    }

    public delegate void EventHandler(object sender, CarCheckPointEventArgs e);
}





