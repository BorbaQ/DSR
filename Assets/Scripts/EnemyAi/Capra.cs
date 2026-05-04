using UnityEngine;
using UnityEngine.AI;

public class Capra : MonoBehaviour
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
    public float closeRange = 2f;
    public float midRange = 4f;
    public float aggroRange = 15f;

    [Header("Attack Cooldowns")]
    public float attack1Cooldown = 1.5f;
    public float attack2Cooldown = 3f;
    private float attack1Timer = 0f;
    private float attack2Timer = 0f;

    [Header("Speed")]
    public float chaseSpeed = 4f;
    public float attackMoveSpeed = 1.5f;  // slow crawl while attacking
    public float jumpSpeed = 8f;

    [Header("Rotation")]
    public float turnSpeed = 8f;

    private NavMeshAgent agent;
    private string currentAnim;
    private bool isAttacking = false;
    private bool isAttacking2 = false;
    private float distToPlayer;

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
            agent.ResetPath();
            return;
        }

        distToPlayer = Vector3.Distance(transform.position, player.position);
        TickCooldowns();

        if (isAttacking)
        {
            // keep creeping toward player but slow
            agent.speed = (isAttacking2 ? jumpSpeed: attackMoveSpeed);
            agent.SetDestination(player.position);
            FacePlayer();
            PlayAnim(attack1Anim == GetCurrentAttackAnim() ? attack1Anim : attack2Anim);
            return;
        }

        DecideAction();
    }

    // track which attack is active so we keep playing it while moving
    private AnimationClip currentAttackAnim;
    AnimationClip GetCurrentAttackAnim() => currentAttackAnim;

    void TickCooldowns()
    {
        attack1Timer -= Time.deltaTime;
        attack2Timer -= Time.deltaTime;
    }

    void DecideAction()
    {
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
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        FacePlayer();
        PlayAnim(walkAnim);
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
        PlayAnim(attackAnim);
    }

    public void ResetBools()
    {
        isAttacking = false;
        currentAttackAnim = null;
        agent.speed = chaseSpeed;
        isAttacking2 = false;
    }

    public void jump()
    {
        agent.speed = jumpSpeed;
        isAttacking2 = true ;
    }

    public void stand()
    {
        agent.speed = 0;
        isAttacking2 = false ;
    }

    void PlayAnim(AnimationClip clip)
    {
        if (currentAnim == clip.name) return;
        anim.CrossFade(clip.name);
        currentAnim = clip.name;
    }
}