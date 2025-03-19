using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController
{
    private readonly int[] _expTable;
    private readonly int[] _maxHitPoints;
    private readonly int[] _defaultPowers;
    private readonly int[] _defaultDefences;

    public EnemyController(string fileName)
    {
        List<int[]> intList = TextToIntArray.ConvertFromFile(fileName);
        _expTable = intList[0];
        _maxHitPoints = intList[1];
        _defaultPowers = intList[2];
        _defaultDefences = intList[3];
    }

    public int GetExp(int level)
    {
        return _expTable[level - 1];
    }

    public int GetDefaultPower(int level)
    {
        return _defaultPowers[level - 1];
    }

    public int GetDefaultDefence(int level)
    {
        return _defaultDefences[level - 1];
    }

    public int GetMaxHitPoint(int level)
    {
        return _maxHitPoints[level - 1];
    }

    public (int level, int exp, int defaultPower, int defaultDefence, int maxHitPoint) GetStatus(int level)
    {
        return (level, GetExp(level), GetDefaultPower(level), GetDefaultDefence(level), GetMaxHitPoint(level));
    }

    public int GetMaxLevel()
    {
        return _expTable.Length;
    }
}
