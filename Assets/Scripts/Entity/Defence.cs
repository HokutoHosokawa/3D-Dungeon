using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defence
{
    const int MinDefence = 0;
    const int MinDefenceUp = 0;
    const int MinDefenceDown = 0;
    const float MinDefenceRate = 0.0f;
    readonly int _defaultDefence;
    readonly int _defenceDiff;
    readonly float _defenceRate;
    readonly int _currentDefence;

    public Defence(int defaultDefence, int defenceDiff, float defenceRate)
    {
        if(defaultDefence < MinDefence)
        {
            throw new System.ArgumentException("Default defence must be greater than 0");
        }
        if(defenceRate < MinDefenceRate)
        {
            throw new System.ArgumentException("DefenceRate must be greater than 0");
        }
        _defaultDefence = defaultDefence;
        _defenceDiff = defenceDiff;
        _defenceRate = defenceRate;
        _currentDefence = Mathf.Max((int)(_defaultDefence * _defenceRate + _defenceDiff + 0.5f), MinDefence); // 割合を先に計算し、その後に加算する
    }

    public Defence(int defaultDefence){
        if(defaultDefence < MinDefence)
        {
            throw new System.ArgumentException("Default defence must be greater than 0");
        }
        _defaultDefence = defaultDefence;
        _defenceDiff = 0;
        _defenceRate = 1.0f;
        _currentDefence = defaultDefence;
    }

    public Defence DefenceUp(int defenceUp){
        if(defenceUp < MinDefenceUp)
        {
            throw new System.ArgumentException("Defence up must be greater than 0");
        }
        int newDefenceDiff = _defenceDiff + defenceUp;
        return new Defence(_defaultDefence, newDefenceDiff, _defenceRate);
    }

    public Defence DefenceDown(int defenceDown){
        if(defenceDown < MinDefenceDown)
        {
            throw new System.ArgumentException("Defence down must be greater than 0");
        }
        int newDefenceDiff = _defenceDiff - defenceDown;
        return new Defence(_defaultDefence, newDefenceDiff, _defenceRate);
    }

    public Defence DefenceChangeRate(float defenceRate){
        if(defenceRate < MinDefenceRate)
        {
            throw new System.ArgumentException("Defence rate must be greater than 0");
        }
        float newDefenceRate = _defenceRate * defenceRate; // 上昇割合は(和ではなく)積で表す。
        return new Defence(_defaultDefence, _defenceDiff, newDefenceRate);
    }

    public Defence DefenceRateClear(float defenceRate){
        if(defenceRate < MinDefenceRate)
        {
            throw new System.ArgumentException("Defence rate must be greater than 0");
        }
        return DefenceChangeRate(1.0f / defenceRate);
    }

    public Defence DefenceReset(){
        return new Defence(_defaultDefence);
    }

    public int CurrentDefence => _currentDefence;
    public int DefaultDefence => _defaultDefence;
    public int DefenceDiff => _defenceDiff;
    public float DefenceRate => _defenceRate;
}
