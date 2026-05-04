using UnityEngine;

public class HitboxManager2 : MonoBehaviour
{
    public Collider[] hitboxes;

    public void EnableHitbox(int index)
    {
        hitboxes[index].enabled = true;
    }

    public void DisableHitbox(int index)
    {
        hitboxes[index].enabled = false;
    }

    public void SetHitDealer(int index, float damage, float knockback)
    {
        HitDealer dealer = hitboxes[index].GetComponent<HitDealer>();
        if (dealer == null) return;
        dealer.damage = damage;
        dealer.knockbackForce = knockback;
    }

    public void DisableAll()
    {
        foreach (var h in hitboxes)
            h.enabled = false;
    }
}