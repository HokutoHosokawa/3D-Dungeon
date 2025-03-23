using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyAttack : MonoBehaviour
{
    private Collider _attackCollider;
    private GameObject _player;
    private Player _playerStatus;
    private Enemy _enemy;
    // Start is called before the first frame update
    void Start()
    {
        _attackCollider = GetComponent<Collider>();
        _attackCollider.enabled = false;
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerStatus = _player.GetComponent<PlayerStatus>().PlayerInfo;
        _enemy = GetComponentInParent<EnemyStatus>().EnemyInfo;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player")
        {
            return;
        }
        // プレイヤーにダメージを与える
        AttackToPlayer();
    }

    // Update is called once per frame
    // void Update()
    // {
    //
    // }

    private void AttackToPlayer()
    {
        _playerStatus.Damage(Mathf.Max(0, _enemy.PowerValue.CurrentPower - _playerStatus.DefenceValue.CurrentDefence));
    }
}
