using UnityEngine;

public class EnemyHitboxReceiver : MonoBehaviour
{
    public EnemyHealthSystem healthSystem;
    public LayerMask damageSourceLayer;

    void OnTriggerEnter(Collider other) => OnHit(other);

    public void OnHit(Collider other)
    {
        if (((1 << other.gameObject.layer) & damageSourceLayer) == 0) return;

        HitDealer dealer = other.GetComponent<HitDealer>();
        if (dealer == null) return;

        int hitId = dealer.GetInstanceID();

        Vector3 hitDir = healthSystem.transform.position - other.transform.position;
        healthSystem.TakeDamage(dealer.damage, dealer.knockbackForce, hitDir, hitId);
    }
}