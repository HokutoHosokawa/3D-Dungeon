using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    private int _level;
    private int _exp;
    private PlayerStatus _playerStatus;
    private GameObject _enemy;

    public Enemy(int level, int exp, int defaultPower, int defaultDefence, int maxHitPoint, GameObject enemy) : base((defaultPower, defaultDefence, maxHitPoint))
    {
        _level = level;
        _exp = exp;
        _playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        _enemy = enemy;
    }

    public Enemy((int level, int exp, int defaultPower, int defaultDefence, int maxHitPoint) defaultStatus, GameObject enemy) : base(defaultStatus.defaultPower, defaultStatus.defaultDefence, defaultStatus.maxHitPoint)
    {
        _level = defaultStatus.level;
        _exp = defaultStatus.exp;
        _playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        _enemy = enemy;
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);
        if (HP.CurrentHitPoint <= 0)
        {
            EntityDead();
        }
    }

    public override void DamageRateFromMaxHP(float damageRate)
    {
        base.DamageRateFromMaxHP(damageRate);
        if (HP.CurrentHitPoint <= 0)
        {
            EntityDead();
        }
    }

    public override void DamageRateFromCurrentHP(float damageRate)
    {
        base.DamageRateFromCurrentHP(damageRate);
        if (HP.CurrentHitPoint <= 0)
        {
            EntityDead();
        }
    }

    public void EntityDead()
    {
        // 敵が倒れた時の処理
        // 経験値をプレイヤーに渡す
        _playerStatus.PlayerInfo.AddExp(_exp);
        // エンティティを削除する
        // オブジェクトがプレイヤーを追いかけていない場合は、追いかけていたゲームオブジェクトも削除
        _enemy.GetComponent<EnemyMoveController>().DestroyTargetGameObject();
        Object.Destroy(_enemy);
    }
}
