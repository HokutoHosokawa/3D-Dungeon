using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSettings : MonoBehaviour
{
    const float WalkingSpeed = 5f;
    const float JumpSpeed = 5f;
    private Animator anim = null;
    private Rigidbody rb = null;

    private Ray ray;
    private RaycastHit hit;
    private float rayDistance = 0.5f;
    private bool onGround = true;

    [SerializeField] private Collider swordCollider;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(!onGround)
        {
            ray = new Ray(transform.position + new Vector3(0, 0.45f, 0), Vector3.down);
            if(Physics.Raycast(ray, out hit, rayDistance))
            {
                onGround = true;
            }
        }
        if(onGround)
        {
            transform.position.Set(transform.position.x, hit.point.y, transform.position.z);
        }
        if(Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            rb.AddForce(Vector3.up * JumpSpeed, ForceMode.Impulse);
            onGround = false;
            anim.SetTrigger("JumpStart");
        }
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        if(moveX != 0 || moveY != 0)
        {
            Vector3 direction = new Vector3(moveX, 0, moveY); //X方向とZ方向の入力の大きさを取得
            Vector3 cameraForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
            if(direction.magnitude > 1)
            {
                direction.Normalize();
            }
            direction = direction.z * transform.forward + direction.x * transform.right;
            if(Vector3.Dot(direction, cameraForward) > 0.65)
            {
                anim.SetBool("IsWalking", true);
                anim.SetBool("IsBackWalking", false);
                anim.SetBool("IsSideWalking", false);
            }
            else if(Vector3.Dot(direction, cameraForward) < -0.65)
            {
                anim.SetBool("IsWalking", false);
                anim.SetBool("IsBackWalking", true);
                anim.SetBool("IsSideWalking", false);
            }
            else
            {
                anim.SetBool("IsWalking", false);
                anim.SetBool("IsBackWalking", false);
                anim.SetBool("IsSideWalking", true);
            }
            rb.velocity = direction * WalkingSpeed;
        }
        else
        {
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsBackWalking", false);
            anim.SetBool("IsSideWalking", false);
        }
    }

    private void Update(){
        if(Input.GetMouseButtonDown(0)){
            anim.SetTrigger("Attack");
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsBackWalking", false);
            anim.SetBool("IsSideWalking", false);
        }
    }

    public void OnAttackStart()
    {
        swordCollider.enabled = true;
    }

    public void OnAttackEnd()
    {
        swordCollider.enabled = false;
    }
}
