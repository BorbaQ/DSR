using UnityEngine;

public class AsylumHB : MonoBehaviour
{
    private HitboxManager2 hb;

    // hitbox index reference so its readable
    const int ax = 0;


    void Start()
    {
        hb = GetComponent<HitboxManager2>();
    }

    // --- Animation Events ---

    public void die()
    {
        AnyAttack_End();
    }

    public void weak()
    {
        hb.EnableHitbox(ax);
        hb.SetHitDealer(ax, 30,60);
    }

    public void strong()
    {
        hb.EnableHitbox(ax);
        hb.SetHitDealer(ax, 50, 120);

    }



    public void AnyAttack_End()
    {
        hb.DisableAll();
    }
}