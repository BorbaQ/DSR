using UnityEngine;

public class CapraHB : MonoBehaviour
{
    private HitboxManager2 hb;
    const int SwordR = 0;
    const int SwordL = 1;


    void Start()
    {
        hb = GetComponent<HitboxManager2>();
    }

    public void Jump()
    {
        hb.EnableHitbox(SwordR);
        hb.EnableHitbox(SwordL);
    }
    public void die()
    {
        AnyAttack_End();
    }
    public void Right()
    {
        hb.EnableHitbox(SwordR);
    }
    public void Left()
    {
        hb.EnableHitbox(SwordL);
    }
    public void AnyAttack_End()
    {
        hb.DisableAll();
    }
}