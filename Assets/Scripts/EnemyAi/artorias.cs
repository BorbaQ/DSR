using UnityEngine;
using UnityEngine.AI;

public class Artorias : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public EnemyHealthSystem healthSystem;
    public UnityEngine.Animation anim;

    [Header("Animations")]
    public AnimationClip idleAnim;
    public AnimationClip walkAnim;
// regular
    public AnimationClip spin;
    public AnimationClip regattack;
    // run
    public AnimationClip runattack1;
    public AnimationClip runattack2;
    public AnimationClip runattack3;
    public AnimationClip jumpattack;
    // back
    public AnimationClip backflip;

    [Header("Attack Ranges")]
    public float closeRange = 3f;
    public float midRange = 7f;
    public float aggroRange = 15f;

    [Header("Attack Cooldowns")]
    public float attack1Cooldown = 15f;
    public float attack2Cooldown = 15f;
    public float attackbackCooldown = 15f;
    public float jump1Cooldown = 5f;
    public float jump2Cooldown = 5f;
    public float jump3Cooldown = 5f;
    public float jump4Cooldown = 5f;
    public float decisionTime = 1f;
    public float attack1Timer = 0f;
    public float attack2Timer = 3f;
    public float attackbackTimer = 6f;
    private float jump1Timer = 0f;
    private float jump2Timer = 3f;
    private float jump3Timer = 6f;
    private float jump4Timer = 9f;
    private float decisionTimer = 0f;

    [Header("Speed")]
    public float chaseSpeed = 4f;
    public float attackMoveSpeed = 1.5f;
    public float jumpSpeed = 10f;

    [Header("Rotation")]
    public float turnSpeed = 7f;

    private Vector3 attackTargetPos;
    private NavMeshAgent agent;
    private string currentAnim;
    private bool isAttacking = false;
    private bool isAttacking2 = false;
    private float distToPlayer;

    private AnimationClip currentAttackAnim;
    AnimationClip GetCurrentAttackAnim() => currentAttackAnim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.speed = chaseSpeed;
        PlayAnim(idleAnim);
    }


    void Update()
    {

        if (healthSystem.IsStaggered)
        {
            if (isAttacking) ResetBools(); // interrupt attack state
            agent.ResetPath();
            return;
        }

        distToPlayer = Vector3.Distance(transform.position, player.position);
        TickCooldowns();

        if (isAttacking)
        {
            FacePlayer();
            attackTargetPos = player.position;
            attackTargetPos.y = transform.position.y;

            // Poll: if the attack animation is done, end the attack ourselves
            if (currentAttackAnim != null && !anim.IsPlaying(currentAttackAnim.name))
            {
                ResetBools();
                return;
            }

            if (isAttacking2)
            {
                agent.speed = jumpSpeed;
                agent.SetDestination(attackTargetPos);
            }
            else if (distToPlayer > closeRange)
            {
                agent.speed = attackMoveSpeed;
                agent.SetDestination(attackTargetPos);
            }
            else
            {
                agent.ResetPath();
            }
            return;
        }

        DecideAction();
    }
    void TickCooldowns()
    {
        attack1Timer -= Time.deltaTime;
        attack2Timer -= Time.deltaTime;
        attackbackTimer -= Time.deltaTime;
        jump1Timer -= Time.deltaTime;
        jump2Timer -= Time.deltaTime;
        jump3Timer -= Time.deltaTime;
        jump4Timer -= Time.deltaTime;
    }
    public void die()
    {
        ResetBools();
    }
    void DecideAction()
    {
        if (isAttacking) return;
        Debug.Log($"DecideAction called. isAttacking={isAttacking}");
        FacePlayer();
        if (decisionTimer > 0)
        {
            decisionTimer -= Time.deltaTime;
            return;
        }

        if (distToPlayer > aggroRange)
        {
            agent.ResetPath();
            PlayAnim(idleAnim);
            return;
        }

        // Close range attacks - each has its own independent timer
        if (distToPlayer <= closeRange)
        {
            if (attack1Timer <= 0f)
            {
                StartAttack(regattack);
                attack1Timer = attack1Cooldown;
                isAttacking = true;
                return;
            }
            if (attack2Timer <= 0f)
            {
                StartAttack(spin);
                attack2Timer = attack2Cooldown;
                isAttacking = true;

                return;
            }
            if (attackbackTimer <= 0f)
            {
                StartAttack(backflip);
                attackbackTimer = attackbackCooldown;
                isAttacking = true;

                return;
            }
            
        }

        // Mid range - uses its OWN timer, not attack2Timer
        if (distToPlayer <= midRange && distToPlayer > closeRange)
        {
            if (jump1Timer <= 0f)
            {
                StartAttack(runattack1);
                jump1Timer = jump1Cooldown;
                isAttacking = true;

                return;
            }
            if (jump2Timer <= 0f)
            {
                StartAttack(runattack2);
                jump2Timer = jump2Cooldown;
                isAttacking = true;

                return;
            }
            if (jump3Timer <= 0f)
            {
                StartAttack(runattack3);
                jump3Timer = jump3Cooldown;
                isAttacking = true;

                return;
            }

            if (jump4Timer <= 0f)
            {
                StartAttack(jumpattack);
                jump4Timer = jump4Cooldown;
                isAttacking = true;
                return;
            }
        }

        Chase();
    }

    void Chase()
    {
        if (healthSystem.helathBar2.enabled == false)
        {
            healthSystem.turnOnHealth();
        }
        if (distToPlayer > closeRange)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            PlayAnim(walkAnim);
        }
        else
        {
            agent.ResetPath();
            PlayAnim(idleAnim);
        }
        FacePlayer();
    }

    void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            turnSpeed * Time.deltaTime
        );
    }

    void StartAttack(AnimationClip attackAnim)
    {
        if (isAttacking) return; // hard gate
        isAttacking = true;
        currentAttackAnim = attackAnim;
        currentAnim = attackAnim.name;
        anim[attackAnim.name].wrapMode = WrapMode.Once; // plays once, stops dead
        anim.CrossFade(attackAnim.name);
    }

    public void ResetBools()
    {
        isAttacking = false;
        currentAttackAnim = null;
        currentAnim = "";
        agent.speed = chaseSpeed;
        isAttacking2 = false;
        decisionTimer = decisionTime;
    }

    public void jump()
    {
        agent.speed = jumpSpeed;
        isAttacking2 = true;
    }
    public void backoff()
    {
        agent.speed = -jumpSpeed;
    }

    public void stand()
    {
        agent.speed = 0;
        isAttacking2 = false;
    }

    void PlayAnim(AnimationClip clip)
    {
        if (currentAnim == clip.name) return;
        anim.CrossFade(clip.name);
        currentAnim = clip.name;
    }
}