// Put this on each child collider object
using UnityEngine;

public class HitboxRelay : MonoBehaviour
{
    public EnemyHitboxReceiver receiver;

    void OnTriggerEnter(Collider other)
    {
        receiver.OnHit(other);   // forward to parent
    }
}