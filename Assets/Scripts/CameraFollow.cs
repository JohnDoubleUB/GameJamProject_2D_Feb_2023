using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow current;

    public Transform crosshairToFollow;
    public Transform playerToFollow;

    public Transform introductionCutsceneLocation;

    [SerializeField]
    private float transitionSpeed = 5f;

    [SerializeField]
    private Camera Camera;

    [SerializeField]
    private float minCameraSize = 5f;

    [SerializeField]
    private float maxCameraSize = 6f;

    private float cameraTransitionValue;

    private bool followPlayer = true;

    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a " + GetType().Name + " in this scene!");
        current = this;
    }

    private void FixedUpdate()
    {
        if (crosshairToFollow == null || playerToFollow == null)
            return;

        if (followPlayer == false)
        {
            if (cameraTransitionValue < 1f)
            {
                cameraTransitionValue = Mathf.Min(1f, cameraTransitionValue + Time.deltaTime);
            }
        }
        else if(cameraTransitionValue > 0f)
        {
            cameraTransitionValue = Mathf.Max(0f, cameraTransitionValue - Time.deltaTime);
        }



        Vector3 targetPosition = Vector3.Lerp(Vector3.Lerp(crosshairToFollow.position, playerToFollow.position, 0.8f), introductionCutsceneLocation.position, cameraTransitionValue);
        targetPosition.z = transform.position.z;

        float distanceAppart = Vector3.Distance(crosshairToFollow.position, playerToFollow.position);

        float cameraSize = Mathf.Clamp(distanceAppart.Remap(0, 20, minCameraSize, maxCameraSize), minCameraSize, maxCameraSize);

        //transform.position = targetPosition;

        if (Camera != null)
        {
            Camera.orthographicSize = cameraSize;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
    }

    public void SetFollowPlayer(bool enabled)
    {
        followPlayer = enabled;
    }

    private void Start()
    {
        if (Camera != null)
        {
            //AudioManager.current.AK_PlayClipOnObject("ShipRumble", gameObject);
            //AudioManager.current.AK_PlayClipOnObject("SpaceSong", gameObject);
        }
    }

    private void OnDestroy()
    {
    }

}
