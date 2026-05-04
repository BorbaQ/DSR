using UnityEngine;

public class PlayerHB : MonoBehaviour
{
    private HitboxManager2 hb;

    // hitbox index reference so its readable
    const int Sword = 0;
   

    void Start()
    {
        hb = GetComponent<HitboxManager2>();
    }

    // --- Animation Events ---

    public void Light()
    {
        hb.SetHitDealer(Sword, damage: 5f, knockback: 8f);
        hb.EnableHitbox(Sword);
    }

    public void Heavy()
    {
        hb.SetHitDealer(Sword, damage: 7f, knockback: 8f);
        hb.EnableHitbox(Sword);
    }

    public void special()
    {
        hb.SetHitDealer(Sword, damage: 10f, knockback: 8f);
        hb.EnableHitbox(Sword);
    }


    public void AnyAttack_End()
    {
        hb.DisableAll();
    }
}