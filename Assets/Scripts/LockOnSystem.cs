using UnityEngine;

public class LockOnSystem : MonoBehaviour
{
    [Header("References")]
    public CameraController cameraController;
    public Transform player;

    [Header("Settings")]
    public float lockOnRange = 15f;
    public LayerMask enemyLayer;
    public KeyCode lockOnKey = KeyCode.Tab;

    private Transform lockedTarget;
    private bool isLocked;

    void Start()
    {
        enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
    }

    void Update()
    {
        if (Input.GetKeyDown(lockOnKey))
        {
            if (isLocked) Unlock();
            else TryLockOn();
        }

        if (isLocked)
        {
            if (lockedTarget == null ||
                Vector3.Distance(player.position, lockedTarget.position) > lockOnRange * 1.5f)
            {
                Unlock();
            }
        }
    }

    void TryLockOn()
    {
        Collider[] hits = Physics.OverlapSphere(player.position, lockOnRange, enemyLayer);
        if (hits.Length == 0) return;

        Transform best = null;
        float bestScore = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            Transform root = hit.transform;
            while (root.parent != null &&
                   root.parent.gameObject.layer == hit.gameObject.layer)
                root = root.parent;

            Vector3 dir = (root.position - player.position).normalized;
            float dist = Vector3.Distance(player.position, root.position);
            float dot = Vector3.Dot(Camera.main.transform.forward, dir);
            float score = dist - (dot * 5f);

            if (score < bestScore) { bestScore = score; best = root; }
        }

        if (best != null)
        {
            lockedTarget = best;
            isLocked = true;
            cameraController.SetLockOn(lockedTarget);
        }
    }

    void Unlock()
    {
        isLocked = false;
        lockedTarget = null;
        cameraController.SetLockOn(null);
    }

    public bool IsLocked() => isLocked;
    public Transform GetTarget() => lockedTarget;
}