using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyHealthSystem : MonoBehaviour
{
    [Header("Health")]
    public float maxHp = 100f;
    public float currentHp;

    [Header("Knockback")]
    public float knockbackDuration = 0.2f;
    public float staggerDuration = 0.4f;
    private float knockbackTimer = 0f;
    private float staggerTimer = 0f;
    private Vector3 knockbackVelocity;
    private CharacterController controller;

    [Header("Animation")]
    public UnityEngine.Animation anim;
    public AnimationClip idleAnim;

    [Header("Events")]
    public UnityEvent<float, float> onHit;
    public UnityEvent onDeath;

    [Header("UI")]
    public Image healthBar;

    public bool IsStaggered => staggerTimer > 0f;

    void Start()
    {
        currentHp = maxHp;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            controller.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 10f);
        }

        if (staggerTimer > 0f)
        {
            staggerTimer -= Time.deltaTime;
            if (staggerTimer <= 0f)
                anim.CrossFade(idleAnim.name);
        }
    }

    public void TakeDamage(float amount, float knockbackForce, Vector3 hitDirection)
    {
        if (currentHp <= 0f) return;

        currentHp = Mathf.Clamp(currentHp - amount, 0f, maxHp);

        if (healthBar != null)
            healthBar.fillAmount = currentHp / maxHp;

        onHit.Invoke(currentHp, maxHp);


        knockbackVelocity = hitDirection.normalized * knockbackForce;
        knockbackVelocity.y = 0f;
        knockbackTimer = knockbackDuration;
        staggerTimer = staggerDuration;

        if (currentHp <= 0f)
            onDeath.Invoke();
    }

    public void Heal(float amount)
    {
        if (currentHp <= 0f) return;
        currentHp = Mathf.Clamp(currentHp + amount, 0f, maxHp);
        onHit.Invoke(currentHp, maxHp);
        if (healthBar != null)
            healthBar.fillAmount = currentHp / maxHp;
    }
}