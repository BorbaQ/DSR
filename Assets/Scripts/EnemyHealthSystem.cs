using System.Collections.Generic;
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
    public float deathStaggerDuration = 10f;
    private bool isDead = false;
    private float deathTimer = 0f;

    [Header("Multi-Hitbox Settings")]
    public float hitCooldown = 1f;

    // Maps hitId -> time when it expires
    private Dictionary<int, float> recentHits = new Dictionary<int, float>();

    void Start()
    {
        currentHp = maxHp;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Clean up expired hit IDs so the dictionary doesn't grow forever
        float now = Time.time;
        List<int> toRemove = null;
        foreach (var kv in recentHits)
        {
            if (kv.Value <= now)
            {
                toRemove ??= new List<int>();
                toRemove.Add(kv.Key);
            }
        }
        if (toRemove != null)
            foreach (int id in toRemove)
                recentHits.Remove(id);

        if (staggerTimer > 0f)
        {
            staggerTimer -= Time.deltaTime;
            if (staggerTimer <= 0f)
                anim.CrossFade(idleAnim.name);
        }

        if (isDead)
        {
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0f)
                Destroy(gameObject);
            return;
        }

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            controller.Move(knockbackVelocity * Time.deltaTime);
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 10f);
        }
    }

    /// <summary>
    /// hitId: pass HitDealer.GetInstanceID() so multi-hitbox bosses don't
    /// get hit multiple times per swing. Pass -1 to skip dedup (e.g. DoT).
    /// </summary>
    public void TakeDamage(float amount, float knockbackForce, Vector3 hitDirection, int hitId = -1)
    {
        if (currentHp <= 0f || isDead) return;

        // Dedup check — ignore if we already registered this hit recently
        if (hitId != -1)
        {
            if (recentHits.TryGetValue(hitId, out float expiry) && expiry > Time.time)
                return;

            recentHits[hitId] = Time.time + hitCooldown;
        }

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

        if (deathParticles != null)
            deathParticles.Play();

        deathTimer = deathStaggerDuration;
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