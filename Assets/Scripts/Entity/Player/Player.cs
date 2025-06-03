using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private GameObject _userInterfaceObject;
    private HitPointBarController _hitPointBarController;
    private LevelTextController _levelTextController;

    public Player(string fileName) : base(GetDefaultStatusFromFile(fileName))
    {
        if (maxExps != null && maxHitPoints != null && defaultPowers != null && defaultDefences != null)
        {
            _exp = 0;
            _level = 1;
            _maxLevel = maxExps.Length;
            _maxExp = maxExps[_level - 1];
            _userInterfaceObject = GameObject.Find("UserInterfaceObject");
            _hitPointBarController = _userInterfaceObject.GetComponent<HitPointBarController>();
            _hitPointBarController.UpdateHitPointBar(HP);
            _levelTextController = _userInterfaceObject.GetComponent<LevelTextController>();
            _levelTextController.UpdateLevelText(_level);
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
        _userInterfaceObject = GameObject.Find("UserInterfaceObject");
        _hitPointBarController = _userInterfaceObject.GetComponent<HitPointBarController>();
        _hitPointBarController.UpdateHitPointBar(HP);
        _levelTextController = _userInterfaceObject.GetComponent<LevelTextController>();
        _levelTextController.UpdateLevelText(_level);
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
        _hitPointBarController.UpdateHitPointBar(HP);
        _levelTextController.UpdateLevelText(_level);
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);
        _hitPointBarController.UpdateHitPointBar(HP);
        if (HP.CurrentHitPoint <= 0)
        {
            GameOver();
        }
    }

    public override void DamageRateFromMaxHP(float damageRate)
    {
        base.DamageRateFromMaxHP(damageRate);
        _hitPointBarController.UpdateHitPointBar(HP);
        if (HP.CurrentHitPoint <= 0)
        {
            GameOver();
        }
    }

    public override void DamageRateFromCurrentHP(float damageRate)
    {
        base.DamageRateFromCurrentHP(damageRate);
        _hitPointBarController.UpdateHitPointBar(HP);
        if (HP.CurrentHitPoint <= 0)
        {
            GameOver();
        }
    }

    public override void Heal(int heal)
    {
        base.Heal(heal);
        _hitPointBarController.UpdateHitPointBar(HP);
    }

    public override void HealRateFromMaxHP(float healRate)
    {
        base.HealRateFromMaxHP(healRate);
        _hitPointBarController.UpdateHitPointBar(HP);
    }

    public override void HealRateFromCurrentHP(float healRate)
    {
        base.HealRateFromCurrentHP(healRate);
        _hitPointBarController.UpdateHitPointBar(HP);
    }

    private void GameOver()
    {
        // ゲームオーバー処理
        SceneManager.LoadScene("GameOverScene");
    }

    public int Level => _level;
    public int Exp => _exp;
    public int MaxLevel => _maxLevel;
    public int MaxExp => _maxExp;
}
