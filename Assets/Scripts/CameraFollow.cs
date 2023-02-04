using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform objectToFollow;
    public Transform objectToFollow2;

    [SerializeField]
    private float transitionSpeed = 5f;

    [SerializeField]
    private Camera Camera;

    [SerializeField]
    private float minCameraSize = 5f;

    [SerializeField]
    private float maxCameraSize = 6f;

    private void FixedUpdate()
    {
        if (objectToFollow == null || objectToFollow2 == null)
            return;

        Vector3 targetPosition = Vector3.Lerp(objectToFollow.position, objectToFollow2.position, 0.8f);
        targetPosition.z = transform.position.z;

        float distanceAppart = Vector3.Distance(objectToFollow.position, objectToFollow2.position);

        float cameraSize = Mathf.Clamp(distanceAppart.Remap(0, 20, minCameraSize, maxCameraSize), minCameraSize, maxCameraSize);

        //transform.position = targetPosition;

        if (Camera != null) 
        {
            Camera.orthographicSize = cameraSize;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
    }
}
