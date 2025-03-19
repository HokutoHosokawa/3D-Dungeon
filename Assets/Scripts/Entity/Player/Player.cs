using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    private static int[] maxExps;
    private static int[] maxHitPoints;
    private static int[] defaultPowers;
    private static int[] defaultDefences;
    private int _exp;
    private int _level;
    private int _maxLevel;
    private int _maxExp;

    public Player(string fileName) : base(GetDefaultStatusFromFile(fileName))
    {
        if (maxExps != null && maxHitPoints != null && defaultPowers != null && defaultDefences != null)
        {
            _exp = 0;
            _level = 1;
            _maxLevel = maxExps.Length;
            _maxExp = maxExps[_level - 1];
            return;
        }
        List<int[]> intList = TextToIntArray.ConvertFromFile(fileName);
        maxExps = intList[0];
        maxHitPoints = intList[1];
        defaultPowers = intList[2];
        defaultDefences = intList[3];
        _exp = 0;
        _level = 1;
        _maxLevel = maxExps.Length;
        _maxExp = maxExps[_level - 1];
    }

    private static (int defaultPower, int defaultDefence, int maxHitPoint) GetDefaultStatusFromFile(string fileName)
    {
        if (maxExps != null && maxHitPoints != null && defaultPowers != null && defaultDefences != null)
        {
            return (defaultPowers[0], defaultDefences[0], maxHitPoints[0]);
        }
        List<int[]> intList = TextToIntArray.ConvertFromFile(fileName);
        maxExps = intList[0];
        maxHitPoints = intList[1];
        defaultPowers = intList[2];
        defaultDefences = intList[3];
        return (defaultPowers[0], defaultDefences[0], maxHitPoints[0]);
    }

    public void AddExp(int exp)
    {
        _exp += exp;
        if (_exp >= _maxExp)
        {
            LevelUp(_exp - _maxExp);
        }
    }

    private void LevelUp(int overExp)
    {
        if (_level >= _maxLevel)
        {
            _level = _maxLevel;
            _exp = _maxExp;
            return;
        }
        _level++;
        _maxExp = maxExps[_level - 1];
        _exp = overExp;
        DefaultPowerChange(defaultPowers[_level - 1]);
        DefaultDefenceChange(defaultDefences[_level - 1]);
        MaxHitPointChange(maxHitPoints[_level - 1]);
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);
        if (HP.CurrentHitPoint <= 0)
        {
            GameOver();
        }
    }

    public override void DamageRateFromMaxHP(float damageRate)
    {
        base.DamageRateFromMaxHP(damageRate);
        if (HP.CurrentHitPoint <= 0)
        {
            GameOver();
        }
    }

    public override void DamageRateFromCurrentHP(float damageRate)
    {
        base.DamageRateFromCurrentHP(damageRate);
        if (HP.CurrentHitPoint <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        // ゲームオーバー処理
    }

    public int Level => _level;
    public int Exp => _exp;
    public int MaxLevel => _maxLevel;
    public int MaxExp => _maxExp;
}
