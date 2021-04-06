using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    
    private TrackCheckpoints trackCheckpoints;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CarDriver>(out CarDriver carDriver))
        {
            Debug.Log(carDriver.name);
            trackCheckpoints.CarThroughCheckpoint(this, carDriver.transform);
        }
    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }

    internal void Hide()
    {
        throw new NotImplementedException();
    }

    internal void Show()
    {
        throw new NotImplementedException();
    }
}
