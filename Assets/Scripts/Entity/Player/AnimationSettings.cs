using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSettings : MonoBehaviour
{
    readonly float WalkingSpeed = 7.5f;
    readonly float JumpSpeed = 67f;
    readonly float ForwardDegree = 50f;
    readonly float DistanceToWalkForwardBetweenPlayerAndGround = 0.3f;
    readonly float TimeToWalkSpeedDecreaseSlowly = 1.0f;
    readonly float MinJumpTime = 1.0f;
    private Animator anim = null;
    private Rigidbody rb = null;

    private Ray ray;
    private RaycastHit hit;
    private float rayDistance = 0.5f;
    private bool onGround = true;
    private float jumpTime = 0.0f;

    private bool[] isMoving = new bool[4];
    private float[] previousFixedUpdateTime = new float[4];
    private float[] pushedTime = new float[4];
    private string[] directions = {"Forward", "Right", "Backward", "Left"};
    private float myMoveX = 0.0f;
    private float myMoveY = 0.0f;

    [SerializeField] private Collider swordCollider;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        for(int i = CommonConst.DirectionMin; i <= CommonConst.DirectionMax; ++i)
        {
            isMoving[i] = false;
            previousFixedUpdateTime[i] = Time.realtimeSinceStartup;
            pushedTime[i] = 0.0f;
        }
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
        ray = new Ray(transform.position + new Vector3(0, 0.45f, 0), Vector3.down);
        bool isHit = Physics.Raycast(ray, out hit, rayDistance);
        if(!onGround)
        {
            jumpTime += Time.fixedDeltaTime;
        }
        if(!onGround && isHit && jumpTime > MinJumpTime)
        {
            onGround = true;
            jumpTime = 0.0f;
        }
        if(onGround)
        {
            transform.position.Set(transform.position.x, hit.point.y, transform.position.z);
        }
        if(InputManager.IsKeyPressed(InputManager.GetKeyCode("Jump")) && onGround)
        {
            rb.AddForce(Vector3.up * rb.mass * JumpSpeed, ForceMode.Impulse);
            onGround = false;
            anim.SetTrigger("JumpStart");
        }
        float currentTime = Time.realtimeSinceStartup;
        for(int i = CommonConst.DirectionMin; i <= CommonConst.DirectionMax; ++i)
        {
            float deltaTime = currentTime - previousFixedUpdateTime[i];
            if(!isMoving[i] && !(pushedTime[i] > 0.0f))
            {
                pushedTime[i] = 0.0f;
            }
            if(!isMoving[i] && pushedTime[i] > 0.0f)
            {
                pushedTime[i] -= deltaTime;
            }
            if(isMoving[i] && !isMoving[(i + 2) % 4])
            {
                pushedTime[i] += deltaTime;
            }
            previousFixedUpdateTime[i] = currentTime;
        }
        // Input.GetAxisを再現すると、経過時間の3倍の量が移動量となっているので、3倍している
        myMoveX = Mathf.Clamp((pushedTime[CommonConst.RightDirection] - pushedTime[CommonConst.LeftDirection]) * 3.0f, -1.0f, 1.0f);
        myMoveY = Mathf.Clamp((pushedTime[CommonConst.UpDirection] - pushedTime[CommonConst.DownDirection]) * 3.0f, -1.0f, 1.0f);
        if(myMoveX == 0 && myMoveY == 0)
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
        Vector3 direction = new Vector3(myMoveX, 0, myMoveY); //X方向とZ方向の入力の大きさを取得
        Vector3 cameraForwardOnFlat = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraForward;
        Vector3 cameraRight;
        if(angleY == 0 || hit2.distance > DistanceToWalkForwardBetweenPlayerAndGround)
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

        for(int i = CommonConst.DirectionMin; i <= CommonConst.DirectionMax; ++i)
        {
            if(!isMoving[i] && InputManager.IsKeyPressed(InputManager.GetKeyCode(directions[i])))
            {
                isMoving[i] = true;
                previousFixedUpdateTime[i] = Time.realtimeSinceStartup - Time.deltaTime;
            }
            if(isMoving[i] && !InputManager.IsKeyPressed(InputManager.GetKeyCode(directions[i])))
            {
                isMoving[i] = false;
                previousFixedUpdateTime[i] = Time.realtimeSinceStartup;
                bool isReverseDirectionKeyPressed = false;
                if(InputManager.IsKeyPressed(InputManager.GetKeyCode(directions[(i + 2) % 4])))
                {
                    if(i == CommonConst.UpDirection && myMoveY > 0)
                    {
                        // 上方向が離されて、移動方向が正だったとき(移動量を0に一度する)
                        isReverseDirectionKeyPressed = true;
                    } else if (i == CommonConst.RightDirection && myMoveX > 0) {
                        // 右方向が離されて、移動方向が正だったとき(移動量を0に一度する)
                        isReverseDirectionKeyPressed = true;
                    } else if(i == CommonConst.DownDirection && myMoveY < 0) {
                        // 下方向が離されて、移動方向が負だったとき(移動量を0に一度する)
                        isReverseDirectionKeyPressed = true;
                    } else if(i == CommonConst.LeftDirection && myMoveX < 0) {
                        // 左方向が離されて、移動方向が負だったとき(移動量を0に一度する)
                        isReverseDirectionKeyPressed = true;
                    }
                }
                if (pushedTime[i] + Time.realtimeSinceStartup - previousFixedUpdateTime[i] > TimeToWalkSpeedDecreaseSlowly && !isReverseDirectionKeyPressed)
                {
                    // 一定時間以上その方向に進んでいる場合は徐々に減速する
                    // 一定時間以上その方向に進んでいても、逆キーが入力されている場合は減速せずに一気に0にする
                    pushedTime[i] = 1.0f / 3.0f;
                } else {
                    pushedTime[i] = 0f;
                    // pushedTime[(i + 2) % 4] = 0f;
                    if(i % 2 == CommonConst.VerticalDirection)
                    {
                        myMoveX = 0.0f;
                    } else {
                        myMoveY = 0.0f;
                    }
                }
            }
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
