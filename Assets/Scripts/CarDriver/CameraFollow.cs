
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //General fields
    public Transform target;
    public Transform lookTarget;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void FixedUpdate()
    {
        //Transform the position of the camera based on the position of the car 
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        //Look at looktarget
        transform.LookAt(lookTarget.position);
    }
}
