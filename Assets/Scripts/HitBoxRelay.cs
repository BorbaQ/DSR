using UnityEngine;

public class HitboxRelay : MonoBehaviour
{
    public EnemyHitboxReceiver receiver;

    void OnTriggerEnter(Collider other)
    {
        receiver.OnHit(other);
    }
}