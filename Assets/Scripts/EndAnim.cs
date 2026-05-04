using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public PlayerController playerController;

    public void OnAttackAnimationEnd()
    {
        playerController.ResetBools();
    }
}