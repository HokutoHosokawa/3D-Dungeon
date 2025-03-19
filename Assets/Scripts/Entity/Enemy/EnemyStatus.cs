using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private Enemy _enemy;

    public void Initialize(Enemy enemy)
    {
        if(_enemy != null)
        {
            return;
        }
        _enemy = enemy;
    }

    public Enemy EnemyInfo => _enemy;
}
