using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private GameObject DamageEffectPrefab;
    private GameObject _playerSword;
    private Enemy _enemy;

    private void Awake()
    {
        DamageEffectPrefab = Resources.Load<GameObject>("Prefabs/Enemy/DamageParticle/Hit_03");
        _playerSword = GameObject.FindGameObjectWithTag("Player").transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/mixamorig:Sword_joint/SwordCollider").gameObject;
    }

    public void Initialize(Enemy enemy)
    {
        if(_enemy != null)
        {
            return;
        }
        _enemy = enemy;
    }

    public void Damage(int damage)
    {
        if(_enemy == null)
        {
            return;
        }
        if(damage <= 0)
        {
            return;
        }
        _enemy.Damage(damage);
        Instantiate(DamageEffectPrefab, _playerSword.transform.position + (Camera.main.transform.position - _playerSword.transform.position).normalized * 0.2f, Quaternion.identity);
    }

    public Enemy EnemyInfo => _enemy;
}
