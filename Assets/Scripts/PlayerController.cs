using GLTFast.Schema;
using UnityEngine;
using Camera = UnityEngine.Camera;

public class PlayerController : MonoBehaviour
{
    [Header("Animations")]
    public AnimationClip idleAnim;
    public AnimationClip walkAnim;
    public AnimationClip runAnim;
    public AnimationClip attackAnim;
    public AnimationClip heavyAttackAnim;
    public AnimationClip specialAttackAnim;
    private string currentAnim;
    public UnityEngine.Animation anim;
    private bool isAttacking = false;

    [Header("Movement")]
    public float speed = 6f;
    public float baseSpeed = 6f;
    public float runSpeed = 12f;
    public Vector3 velocity;
    public float turnSpeed = 10f;
    private CharacterController controller;

    [Header("Attack Rotation")]
    public float attackWindupTurnSpeed = 3f;
    public float attackWindupMoveSpeed = 2f;
    private bool rotationLocked = false;

    [Header("Camera")]
    public LockOnSystem lockOn;
    public Camera Camera;

    [Header("Gravity")]
    public float gravity = -20f;
    public float groundedGravity = -2f;  // small constant to keep grounded check stable
    private float verticalVelocity = 0f;

    // Called by Animation Event at clip end
    public void ResetBools()
    {
        isAttacking = false;
        rotationLocked = false;
    }

    // Called by Animation Event at the commit frame
    public void LockRotation()
    {
        rotationLocked = true;
    }

    void Start()
    {
        PlayAnim(idleAnim);
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftShift) && !isAttacking)
        {
            PlayAnim(heavyAttackAnim);
            isAttacking = true;
            rotationLocked = false;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking)
        {
            PlayAnim(attackAnim);
            isAttacking = true;
            rotationLocked = false;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && !isAttacking)
        {
            PlayAnim(specialAttackAnim);
            isAttacking = true;
            rotationLocked = false;
        }

        Move();
    }

    void Move()
    {
        float x = 0f;
        float z = 0f;
        if (Input.GetKey(KeyCode.W)) z += 1;
        if (Input.GetKey(KeyCode.S)) z -= 1;
        if (Input.GetKey(KeyCode.D)) x += 1;
        if (Input.GetKey(KeyCode.A)) x -= 1;

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * z + camRight * x).normalized;

        if (moveDir.magnitude > 0.1f)
        {
            if (!rotationLocked)
            {
                float currentTurnSpeed = isAttacking ? attackWindupTurnSpeed : turnSpeed;
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(moveDir),
                    currentTurnSpeed * Time.deltaTime
                );
            }

            if (isAttacking)
            {
                if (!rotationLocked)
                    controller.Move(moveDir * attackWindupMoveSpeed * Time.deltaTime);
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift)) speed = runSpeed;
                controller.Move(moveDir * speed * Time.deltaTime);
                PlayAnim(speed == runSpeed ? runAnim : walkAnim);
                speed = baseSpeed;
            }
        }
        else
        {
            if (lockOn != null && lockOn.IsLocked() && lockOn.GetTarget() != null && !rotationLocked)
            {
                Vector3 toTarget = lockOn.GetTarget().position - transform.position;
                toTarget.y = 0f;
                if (toTarget.sqrMagnitude > 0.01f)
                {
                    float currentTurnSpeed = isAttacking ? attackWindupTurnSpeed : turnSpeed;
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(toTarget),
                        currentTurnSpeed * Time.deltaTime
                    );
                }
            }

            if (!isAttacking) PlayAnim(idleAnim);
        }
        if (controller.isGrounded)
            verticalVelocity = groundedGravity;   // tiny push keeps isGrounded reliable
        else
            verticalVelocity += gravity * Time.deltaTime;

        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    void PlayAnim(AnimationClip clip)
    {
        if (currentAnim == clip.name) return;
        anim.CrossFade(clip.name);
        currentAnim = clip.name;
    }
}