using UnityEngine;

public class ArtoriasHB : MonoBehaviour
{
    private HitboxManager2 hb;

    // hitbox index reference so its readable
    const int Sword = 0;



    void Start()
    {
        hb = GetComponent<HitboxManager2>();
    }

    // --- Animation Events ---

    public void Jump()
    {
        hb.EnableHitbox(Sword);
    }

    public void die()
    {
        AnyAttack_End();
    }

    public void AttackLight()
    {
        hb.EnableHitbox(Sword);
    }

    public void AttackHeavy()
    {
        hb.EnableHitbox(Sword);
    }

    public void AnyAttack_End()
    {
        hb.DisableAll();
    }
}