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
        Transform checkpointsTransform = transform.Find("Checkpoints");

        checkpointSingleList = new List<CheckpointSingle>();

        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();

            checkpointSingle.SetTrackCheckpoints(this);

            checkpointSingleList.Add(checkpointSingle);
            Debug.Log(checkpointSingle.name);
        }

        nextCheckpointSingleIndexList = new List<int>();
        foreach (Transform carTransform in carTransformList)
        {
            nextCheckpointSingleIndexList.Add(0);
        }
    }

    public void CarThroughCheckpoint(CheckpointSingle checkpointSingle, Transform carTransform)
    {
        CarCheckPointEventArgs e = new CarCheckPointEventArgs(carTransform);
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)];
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            Debug.Log("Correct");
            Debug.Log(checkpointSingleList.IndexOf(checkpointSingle));
            Debug.Log(checkpointSingleList.Count);
            CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];
            //correctCheckpointSingle.Hide();

            nextCheckpointSingleIndexList[carTransformList.IndexOf(carTransform)]
                = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            OnCarCorrectCheckpoint?.Invoke(this, e);

            //if the car passes through the last checkpoint -> reset checkpoints
            if(checkpointSingleList.IndexOf(checkpointSingle) == (checkpointSingleList.Count - 127))
            {
                ResetCheckpoint();
                Debug.Log("checpoints resetted");
            }
            // Correct checkpoint
        }
        else
        {
            // Wrong checkpoint
            Debug.Log("Wrong");
            OnCarWrongCheckpoint?.Invoke(this, e);

            CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];
            //correctCheckpointSingle.Show();
        }
    }

    //Get next checkpoint for changing agents position
    internal CheckpointSingle GetNextCheckpoint(Transform transform)
    {
        int index = nextCheckpointSingleIndexList[carTransformList.IndexOf(transform)];
        return checkpointSingleList[index];
    }

    public void ResetCheckpoint()
    {
        Transform checkpointsTransform = transform.Find("Checkpoints");

        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();

            checkpointSingle.SetTrackCheckpoints(this);

            checkpointSingleList.Add(checkpointSingle);
            Debug.Log(checkpointSingle.name);
        }

        nextCheckpointSingleIndexList.Clear();
        foreach (Transform carTransform in carTransformList)
        {
            nextCheckpointSingleIndexList.Add(0);
        }

    }
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





