using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Free Orbit Settings")]
    public float sensitivity = 3f;
    public float distance = 5f;
    public float minPitch = -20f;
    public float maxPitch = 60f;
    public Vector3 pivotOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Lock-On Settings")]
    public float lockedDistance = 5f;
    public float lockedHeight = 5f;
    public float lockOnLerpSpeed = 5f;

    private float yaw;
    private float pitch;

    // Set by LockOnSystem
    private Transform lockedTarget;
    private bool isLocked;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        if (isLocked && lockedTarget != null)
            UpdateLockedCamera();
        else
            UpdateFreeCamera();
    }

    void UpdateFreeCamera()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Vector3 pivot = player.position + pivotOffset;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        transform.position = pivot + rotation * new Vector3(0f, 0f, -distance);
        transform.LookAt(pivot);
    }

    void UpdateLockedCamera()
    {
        // Position camera behind player relative to the enemy direction
        Vector3 dirToEnemy = (lockedTarget.position - player.position);
        dirToEnemy.y = 0f;
        dirToEnemy.Normalize();

        // Step back behind the player, raise up a bit
        Vector3 targetPos = player.position - dirToEnemy * lockedDistance;
        targetPos.y = player.position.y + lockedHeight;

        // Lerp smoothly to avoid snapping
        transform.position = Vector3.Lerp(transform.position, targetPos, lockOnLerpSpeed * Time.deltaTime);

        // Look at midpoint between player and enemy so both are in frame
        Vector3 lookTarget = (player.position + lockedTarget.position) / 2f;
        lookTarget.y = player.position.y + 1.5f;

        transform.LookAt(Vector3.Lerp(
            transform.forward + transform.position,
            lookTarget,
            lockOnLerpSpeed * Time.deltaTime)
        );
    }

    // Called by LockOnSystem
    public void SetLockOn(Transform target)
    {
        lockedTarget = target;
        isLocked = target != null;
        if (!isLocked) SyncAnglesFromTransform();
    }

    public void SyncAnglesFromTransform()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }
}