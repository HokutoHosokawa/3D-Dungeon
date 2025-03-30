using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSettings : MonoBehaviour
{
    readonly float WalkingSpeed = 7.5f;
    readonly float JumpSpeed = 5f;
    readonly float ForwardDegree = 50f;
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
        if(swordCollider.enabled)
        {
            // 攻撃モーションのときは、移動できないように
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsBackWalking", false);
            anim.SetBool("IsSideWalking", false);
            return;
        }
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
        if(moveX == 0 && moveY == 0)
        {
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsBackWalking", false);
            anim.SetBool("IsSideWalking", false);
            return;
        }

        RaycastHit hit2;
        float angleY = 0;
        Vector3 rayPos = transform.position;
        Vector3 floorNormal = Vector3.up;
        if(Physics.Raycast(transform.position, Vector3.down, out hit2, 3.0f))
        {
            angleY = Vector3.Angle(hit2.normal, Vector3.up);
            rayPos = hit2.point;
            floorNormal = hit2.normal;
        }
        Vector3 direction = new Vector3(moveX, 0, moveY); //X方向とZ方向の入力の大きさを取得
        Vector3 cameraForwardOnFlat = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraForward;
        Vector3 cameraRight;
        if(angleY == 0)
        {
            cameraForward = cameraForwardOnFlat;
        }
        else
        {
            RaycastHit hit3;
            Vector3 rayPos2 = rayPos + cameraForwardOnFlat * 0.08f;
            if(Physics.Raycast(transform.position + cameraForwardOnFlat * 0.08f, Vector3.down, out hit3, 3.0f))
            {
                rayPos2 = hit3.point;
            }
            cameraForward = (rayPos2 - rayPos).normalized;
        }
        cameraRight = Vector3.Cross(floorNormal, cameraForward).normalized;
        if(direction.magnitude > 1)
        {
            direction.Normalize();
        }
        // Vector3 moveVector = direction.z * transform.forward + direction.x * transform.right;
        Vector3 moveVector = direction.z * cameraForward + direction.x * cameraRight;
        if(Vector3.Dot(moveVector.normalized, cameraForward) > Mathf.Cos(ForwardDegree * Mathf.Deg2Rad))
        {
            // 正面と移動する方向の内積がcos(50°)より大きい場合は前進
            anim.SetBool("IsWalking", true);
            anim.SetBool("IsBackWalking", false);
            anim.SetBool("IsSideWalking", false);
        }
        else if(Vector3.Dot(moveVector.normalized, cameraForward) < -Mathf.Cos(ForwardDegree * Mathf.Deg2Rad))
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
        rb.velocity = moveVector * WalkingSpeed;
    }

    private void Update(){
        if(Input.GetMouseButtonDown(0) && !swordCollider.enabled){
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
