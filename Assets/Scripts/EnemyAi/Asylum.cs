using UnityEngine;
using UnityEngine.AI;

public class Asylum : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public EnemyHealthSystem healthSystem;
    public UnityEngine.Animation anim;

    [Header("Animations")]
    public AnimationClip idleAnim;
    public AnimationClip walkAnim;
    public AnimationClip attack1Anim;
    public AnimationClip attack2Anim;

    [Header("Attack Ranges")]
    public float closeRange = 5f;
    public float midRange = 8f;
    public float aggroRange = 15f;

    [Header("Attack Cooldowns")]
    public float attack1Cooldown = 1.5f;
    public float attack2Cooldown = 3f;
    public float decisionTime = 1f;
    private float attack1Timer = 0f;
    private float attack2Timer = 0f;
    private float decisionTimer = 0f;

    [Header("Speed")]
    public float chaseSpeed = 4f;
    public float attackMoveSpeed = 1.5f;  // slow crawl while attacking
    public float jumpSpeed = 8f;

    [Header("Rotation")]
    public float turnSpeed = 8f;

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
            //FacePlayer();
            PlayAnim(currentAttackAnim == attack1Anim ? attack1Anim : attack2Anim);

            if (isAttacking2)
            {
                agent.speed = jumpSpeed;
                agent.SetDestination(attackTargetPos); // frozen dir
            }
            else if (distToPlayer > closeRange)
            {
                agent.speed = attackMoveSpeed;
                agent.SetDestination(attackTargetPos); // frozen dir
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
    }

    public void die()
    {
        ResetBools();
    }

    void DecideAction()
    {
        FacePlayer();
        if (decisionTimer >= 0)
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

        if (distToPlayer <= closeRange && attack1Timer <= 0f)
        {
            StartAttack(attack1Anim);
            attack1Timer = attack1Cooldown;
            return;
        }

        if (distToPlayer <= midRange && distToPlayer > closeRange && attack2Timer <= 0f)
        {
            StartAttack(attack2Anim);
            attack2Timer = attack2Cooldown;
            return;
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
        isAttacking = true;
        currentAttackAnim = attackAnim;
        attackTargetPos = player.position; // snapshot here
        attackTargetPos.y = transform.position.y;
        PlayAnim(attackAnim);
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