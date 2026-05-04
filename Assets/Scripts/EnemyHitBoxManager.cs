using UnityEngine;

public class EnemyHitboxReceiver : MonoBehaviour
{
    public EnemyHealthSystem healthSystem;
    public LayerMask damageSourceLayer;

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & damageSourceLayer) == 0) return;

        HitDealer dealer = other.GetComponent<HitDealer>();
        if (dealer == null) return;

        Vector3 hitDir = healthSystem.transform.position - other.transform.position;
        healthSystem.TakeDamage(dealer.damage, dealer.knockbackForce, hitDir);
    }
}