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
    public Image helathBar2;

    public bool IsStaggered => staggerTimer > 0f;

    [Header("Death")]
    public ParticleSystem deathParticles;
    public float deathStaggerDuration = 10f;   // fallback if no particle system
    private bool isDead = false;
    private float deathTimer = 0f;

    void Start()
    {
        currentHp = maxHp;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (staggerTimer > 0f)
        {
            staggerTimer -= Time.deltaTime;
            if (staggerTimer <= 0f)
                anim.CrossFade(idleAnim.name);
        }
        if (isDead)
        {
            // Count down either particle duration or fallback duration
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0f)
            {
                GameObject.Destroy(gameObject);
            }
            return; // skip normal movement while death plays
        }
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            controller.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 10f);
        }

        
    }

    public void TakeDamage(float amount, float knockbackForce, Vector3 hitDirection)
    {
        if (currentHp <= 0f || isDead) return;

        currentHp = Mathf.Clamp(currentHp - amount, 0f, maxHp);

        if (healthBar != null)
            healthBar.fillAmount = currentHp / maxHp;

        onHit.Invoke(currentHp, maxHp);


        knockbackVelocity = hitDirection.normalized * knockbackForce;
        knockbackVelocity.y = 0f;
        knockbackTimer = knockbackDuration;

        if (staggerTimer <= 0.1f)
            staggerTimer = staggerDuration;

        if (currentHp <= 0f)
        {
            onDeath.Invoke();
            TriggerDeath();
        }
    }

    public void turnOnHealth()
    {
        helathBar2.enabled = true;
    }

    private void TriggerDeath()
    {
        isDead = true;
        knockbackTimer = 0f;
        staggerTimer = deathStaggerDuration;
        anim.CrossFade(idleAnim.name);


        // Play particle effect
        if (deathParticles != null)
        {
            deathParticles.Play();
            // Use actual particle duration if available, otherwise fallback
            float particleDuration = 10f;
            deathTimer = deathStaggerDuration;
        }
        else
        {
            deathTimer = deathStaggerDuration;
        }
        staggerTimer = deathStaggerDuration;

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