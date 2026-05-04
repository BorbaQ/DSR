using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    public float maxHp = 100f;
    public float currentHp;

    [Header("Iframes")]
    public float iframeDuration = 0.5f;
    private float iframeTimer = 0f;
    private bool isInvincible = false;

    [Header("Knockback")]
    public float knockbackDuration = 0.2f;
    public float staggerDuration = 0.4f;
    private float knockbackTimer = 0f;
    private float staggerTimer = 0f;
    private Vector3 knockbackVelocity;
    private CharacterController controller;

    [Header("Animation")]
    public UnityEngine.Animation anim;
    public AnimationClip idleAnim;          // drag your idle clip here
    private PlayerController playerController;

    [Header("Events")]
    public UnityEvent<float, float> onHit;
    public UnityEvent onDeath;

    [Header("UI")]
    public Image HealthBar;

    public bool IsStaggered => staggerTimer > 0f;
    public bool IsKnockedBack => knockbackTimer > 0f;

    void Start()
    {
        currentHp = maxHp;
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isInvincible)
        {
            iframeTimer -= Time.deltaTime;
            if (iframeTimer <= 0f)
                isInvincible = false;
        }

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            controller.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 10f);
        }

        if (staggerTimer > 0f)
        {
            staggerTimer -= Time.deltaTime;

            // once stagger ends snap back to idle and reset attack state
            if (staggerTimer <= 0f)
            {
                playerController.ResetBools();
                anim.CrossFade(idleAnim.name);
            }
        }
    }

    public void TakeDamage(float amount, float knockbackForce, Vector3 hitDirection)
    {
        if (isInvincible) return;
        if (currentHp <= 0f) return;

        currentHp = Mathf.Clamp(currentHp - amount, 0f, maxHp);
        HealthBar.fillAmount = currentHp / maxHp;
        onHit.Invoke(currentHp, maxHp);

        // interrupt whatever anim is playing and go idle immediately
        playerController.ResetBools();
        anim.CrossFade(idleAnim.name);

        knockbackVelocity = hitDirection.normalized * knockbackForce;
        knockbackVelocity.y = 0f;
        knockbackTimer = knockbackDuration;
        staggerTimer = staggerDuration;

        if (currentHp <= 0f)
            onDeath.Invoke();
        else
        {
            isInvincible = true;
            iframeTimer = iframeDuration;
        }
    }

    public void Heal(float amount)
    {
        if (currentHp <= 0f) return;
        currentHp = Mathf.Clamp(currentHp + amount, 0f, maxHp);
        onHit.Invoke(currentHp, maxHp);
        HealthBar.fillAmount = currentHp / maxHp;
    }
}