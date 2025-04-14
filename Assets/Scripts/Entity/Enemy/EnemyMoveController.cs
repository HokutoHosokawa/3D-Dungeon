using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(CommonEnemyVariable))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class EnemyMoveController : MonoBehaviour
{
    private readonly float EnemyViewingAngle = 50.0f;
    private readonly float EnemyLoseSightViewingAngle = 90.0f;
    private readonly float EnemyViewingDistanceInPath = 2.0f;
    private readonly float EnemyLoseSightDistance = 5.0f;
    private readonly float MinimumPlayerTargetTime = 3.0f;
    private readonly float MaximumTargetChangeCoolTime = 5.0f;
    private readonly float AttackDistance = 0.5f;
    private readonly float AttackInterval = 1.0f;
    private Animator _animator;
    private EnemyAnimation _enemyAnimation;
    private GameObject _targetGameObject;
    private GameObject _player;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private CommonEnemyVariable _commonEnemyVariable;
    private CommonPlayerVariable _commonPlayerVariable;
    private float _playerTargetTime = 0.0f;
    private float _enemyAttackCoolTime = 0.0f;
    private float _enemyTargetChangeCoolTime = 0.0f;
    private bool _isAttacking = false;
    private bool _wasAttacking = false;

    [SerializeField] private Collider _attackCollider;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _enemyAnimation = new EnemyAnimation(_animator);
        _targetGameObject = CreateTargetGameObject.CreateTarget(gameObject);
        _player = GameObject.FindGameObjectWithTag("Player");
        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _navMeshAgent.areaMask |= 1 << LayerMask.NameToLayer("PatitionLayer");
        _navMeshAgent.SetDestination(_targetGameObject.transform.position);
        _commonEnemyVariable = GetComponent<CommonEnemyVariable>();
        _commonPlayerVariable = _player.GetComponent<CommonPlayerVariable>();
        _enemyTargetChangeCoolTime = 0.0f;
        _enemyAttackCoolTime = AttackInterval;
        if(_attackCollider == null)
        {
            Debug.LogError("AttackColliderがアタッチされていません");
        }
        _attackCollider.enabled = false;
    }

    void Update()
    {
        // 攻撃中は何もしない
        _wasAttacking = _isAttacking;
        _isAttacking = _enemyAnimation.IsAttacking();
        if(_isAttacking)
        {
            // 移動量を0にする
            _attackCollider.enabled = true;
            _navMeshAgent.velocity = Vector3.zero;
            _navMeshAgent.isStopped = true;
            return;
        }
        if(_wasAttacking && !_isAttacking)
        {
            // 攻撃が終わったら移動を再開
            _attackCollider.enabled = false;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(_targetGameObject.transform.position);
            _enemyAttackCoolTime = 0.0f;
        }
        _enemyAttackCoolTime += Time.deltaTime;
        // 移動中は歩くアニメーションを再生
        if(_navMeshAgent.velocity.magnitude > 0.1f)
        {
            _enemyAnimation.Walk();
        }
        else
        {
            _enemyAnimation.Idle();
        }
        // ターゲット設定
        if(_targetGameObject != _player)
        {
            MoveControllWhenTargetIsNotPlayer();
        }
        else
        {
            MoveControllWhenTargetIsPlayer();
        }
        // 攻撃判定
        Attack();
    }

    // ターゲットを設定する(攻撃された際にプレイヤーをターゲットにする等)
    public void SetTargetToPlayer()
    {
        if(_targetGameObject == _player)
        {
            return;
        }
        Destroy(_targetGameObject);
        _targetGameObject = _player;
        _navMeshAgent.SetDestination(_targetGameObject.transform.position);
    }

    private void SetTargetToNewGameObject()
    {
        if(_targetGameObject != _player)
        {
            Destroy(_targetGameObject);
        }
        _targetGameObject = CreateTargetGameObject.CreateTarget(gameObject);
        _navMeshAgent.SetDestination(_targetGameObject.transform.position);
        _enemyTargetChangeCoolTime = 0.0f;
        _playerTargetTime = 0.0f;
    }

    private void MoveControllWhenTargetIsNotPlayer()
    {
        if(_targetGameObject == _player)
        {
            return;
        }
        // ターゲットがプレイヤーでない場合
        _enemyTargetChangeCoolTime += Time.deltaTime;
        Vector3 targetPosition = _targetGameObject.transform.position;
        targetPosition.y = transform.position.y;
        if(Vector3.Distance(transform.position, targetPosition) < 0.1f || _enemyTargetChangeCoolTime > MaximumTargetChangeCoolTime)
        {
            // ターゲットに到達したら新しいターゲットを設定
            SetTargetToNewGameObject();
        }
        // プレイヤーが目線に入ったらターゲットをプレイヤーに変更
        PlayerCheckInSameRoomWhenTargetIsNotPlayer();
        PlayerCheckInPathWhenTargetIsNotPlayer();
    }

    private void MoveControllWhenTargetIsPlayer()
    {
        if(_targetGameObject != _player)
        {
            return;
        }
        _navMeshAgent.SetDestination(_player.transform.position);
        _playerTargetTime += Time.deltaTime;
        if(_playerTargetTime < MinimumPlayerTargetTime){
            return;
        }
        // ターゲットがプレイヤーの場合
        // プレイヤーを見失う条件に引っかかる場合
        // 同じ部屋にいて、プレイヤーが視界から外れた場合 or 同じ部屋にいなくて、プレイヤーが一定距離以上離れた場合
        PlayerCheckInSameRoomWhenTargetIsPlayer();
        PlayerCheckInPathWhenTargetIsPlayer();
    }

    private void Attack()
    {
        if(_isAttacking)
        {
            return;
        }
        if(Vector3.Distance(transform.position, _player.transform.position) > AttackDistance)
        {
            return;
        }
        if(_targetGameObject != _player)
        {
            return;
        }
        if(_enemyAttackCoolTime < AttackInterval)
        {
            return;
        }
        // 間に壁がない かつ 敵がプレイヤー方向を向いている場合に攻撃
        RaycastHit hit;
        Vector3 enemyForward = transform.forward;
        enemyForward.y = 0;
        if(Physics.Raycast(transform.position, enemyForward, out hit, AttackDistance))
        {
            if(hit.collider.gameObject != _player)
            {
                // 間に壁がある場合
                return;
            }
            _enemyAnimation.Attack();
        }
    }

    private void PlayerCheckInSameRoomWhenTargetIsNotPlayer()
    {
        if(_targetGameObject == _player)
        {
            return;
        }
        if(!_commonEnemyVariable.isEnemyInRoom || !_commonPlayerVariable.isPlayerInRoom)
        {
            return;
        }
        if(!(_commonEnemyVariable.currentRoomCollider.gameObject == _commonPlayerVariable.currentRoomCollider.gameObject))
        {
            return;
        }
        // プレイヤーが同じ部屋にいる場合
        // 敵の視線にプレイヤーが入ったらプレイヤーをターゲットにする
        // 正規化された敵の前方ベクトルと、正規化されたプレイヤーと敵の相対位置ベクトルの内積がcos(EnemyViewingAngle°)より大きい(正面を中心として2×EnemyViewingAngle°分)場合
        Vector3 playerDirection = _player.transform.position - transform.position;
        playerDirection.y = 0;
        Vector3 enemyForward = transform.forward;
        enemyForward.y = 0;
        if(Vector3.Dot(enemyForward.normalized, playerDirection.normalized) > Mathf.Cos(EnemyViewingAngle * Mathf.Deg2Rad))
        {
            SetTargetToPlayer();
        }
    }

    private void PlayerCheckInPathWhenTargetIsNotPlayer()
    {
        if(_targetGameObject == _player)
        {
            return;
        }
        if(_commonEnemyVariable.isEnemyInRoom)
        {
            return;
        }
        // 敵が通路にいる場合
        // プレイヤーが一定距離内に入っている かつ 敵の視線方向に壁などがなく、プレイヤーが見える場合
        RaycastHit hit;
        Vector3 enemyForward = transform.forward;
        enemyForward.y = 0;
        Vector3 enemyToPlayerDirection = _player.transform.position - transform.position;
        enemyToPlayerDirection.y = 0;
        if(Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit, EnemyViewingDistanceInPath))
        {
            if(hit.collider.gameObject == _player && Vector3.Dot(enemyForward.normalized, enemyToPlayerDirection.normalized) > Mathf.Cos(EnemyViewingAngle * Mathf.Deg2Rad))
            {
                SetTargetToPlayer();
            }
        }
    }

    private void PlayerCheckInSameRoomWhenTargetIsPlayer()
    {
        if(_targetGameObject != _player)
        {
            return;
        }
        if(!_commonEnemyVariable.isEnemyInRoom || !_commonPlayerVariable.isPlayerInRoom)
        {
            return;
        }
        if(!(_commonEnemyVariable.currentRoomCollider.gameObject == _commonPlayerVariable.currentRoomCollider.gameObject))
        {
            return;
        }
        // プレイヤーが同じ部屋にいる場合
        // 敵の視線からプレイヤーが離れたらターゲットを変更
        Vector3 playerDirection = _player.transform.position - transform.position;
        playerDirection.y = 0;
        Vector3 enemyForward = transform.forward;
        enemyForward.y = 0;
        if(Vector3.Dot(enemyForward.normalized, playerDirection.normalized) < Mathf.Cos(EnemyLoseSightViewingAngle * Mathf.Deg2Rad))
        {
            SetTargetToNewGameObject();
        }
    }

    private void PlayerCheckInPathWhenTargetIsPlayer()
    {
        if(_targetGameObject != _player)
        {
            return;
        }
        if(_commonEnemyVariable.isEnemyInRoom)
        {
            return;
        }
        // 敵が通路にいる場合
        // プレイヤーが一定距離以上離れたらターゲットを変更
        // 視線条件を追加すると、角を曲がるだけでターゲットが変わってしまうため、視線条件はなし
        if(Vector3.Distance(transform.position, _player.transform.position) > EnemyLoseSightDistance)
        {
            SetTargetToNewGameObject();
        }
    }

    public void DestroyTargetGameObject()
    {
        if(_targetGameObject == null || _targetGameObject == _player)
        {
            return;
        }
        Destroy(_targetGameObject);
    }
}
