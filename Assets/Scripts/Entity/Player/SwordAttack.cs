using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SwordAttack : MonoBehaviour
{
    private Collider swordCollider;
    private Player playerStatus;
    // Start is called before the first frame update
    void Start()
    {
        swordCollider = GetComponent<Collider>();
        swordCollider.enabled = false;
        playerStatus = GetComponentInParent<PlayerStatus>().PlayerInfo;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            // 敵にダメージを与える
            AttackToEnemy(other.gameObject);
        }
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    private void AttackToEnemy(GameObject enemy)
    {
        // 敵にダメージを与える処理
        // Enemy enemyStatus = enemy.GetComponent<EnemyStatus>().EnemyInfo;
        // enemyStatus.Damage(Mathf.Max(0, playerStatus.PowerValue.CurrentPower - enemyStatus.DefenceValue.CurrentDefence));
        enemy.GetComponent<EnemyStatus>().Damage(Mathf.Max(0, playerStatus.PowerValue.CurrentPower - enemy.GetComponent<EnemyStatus>().EnemyInfo.DefenceValue.CurrentDefence));
        enemy.GetComponent<EnemyMoveController>().SetTargetToPlayer();
    }
}
