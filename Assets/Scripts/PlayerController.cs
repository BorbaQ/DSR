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

    [Header("Movement")]
    public float speed = 6f;
    public float baseSpeed = 6f;
    public float runSpeed = 12f;
    public Vector3 velocity;
    public float turnSpeed = 10f;
    private CharacterController controller;
    float forwardInput;

    [Header("Camera")]
    public LockOnSystem lockOn;
    public Camera Camera;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayAnim(idleAnim);
        controller = GetComponent<CharacterController>();
        Debug.Log(controller);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
        //attack
        }
        else
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
           //heaby
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //skill
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

        Vector3 inputDir = new Vector3(x, 0f, z).normalized;

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * z + camRight * x).normalized;

        if (moveDir.magnitude > 0.1f)
        {
            if (lockOn != null && lockOn.IsLocked())
            {
                Vector3 toTarget = (lockOn.GetTarget().position - transform.position);
                toTarget.y = 0f;
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(toTarget),
                    turnSpeed * Time.deltaTime
                );
            }
            else
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(moveDir),
                    turnSpeed * Time.deltaTime
                );
            }
            if (Input.GetKeyDown(KeyCode.LeftShift)) { speed = runSpeed; }
            controller.Move(moveDir * speed * Time.deltaTime);
            speed = baseSpeed;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void PlayAnim(AnimationClip clip)
    {
        if (currentAnim == clip.name) return;

        anim.CrossFade(clip.name);
        currentAnim = clip.name;
    }
}
