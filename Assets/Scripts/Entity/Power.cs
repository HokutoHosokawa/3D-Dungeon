using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power
{
    const int MinPower = 1;
    const int MinPowerUp = 0;
    const int MinPowerDown = 0;
    const float MinPowerRate = 0.0f;
    readonly int _defaultPower;
    readonly int _powerDiff;
    readonly float _powerRate;
    readonly int _currentPower;

    public Power(int defaultPower, int powerDiff, float powerRate)
    {
        if(defaultPower < MinPower)
        {
            throw new System.ArgumentException("Default power must be greater than 1");
        }
        if(_powerRate < MinPowerRate)
        {
            throw new System.ArgumentException("PowerRate must be greater than 0");
        }
        _defaultPower = defaultPower;
        _powerDiff = powerDiff;
        _powerRate = powerRate;
        _currentPower = Mathf.Max((int)(_defaultPower * _powerRate + _powerDiff + 0.5f), MinPower); // 割合を先に計算し、その後に加算する
    }

    public Power(int defaultPower){
        if(defaultPower < MinPower)
        {
            throw new System.ArgumentException("Default power must be greater than 1");
        }
        _defaultPower = defaultPower;
        _powerDiff = 0;
        _powerRate = 1.0f;
        _currentPower = defaultPower;
    }

    public Power PowerUp(int powerUp){
        if(powerUp < MinPowerUp)
        {
            throw new System.ArgumentException("Power up must be greater than 0");
        }
        int newPowerDiff = _powerDiff + powerUp;
        return new Power(_defaultPower, newPowerDiff, _powerRate);
    }

    public Power PowerDown(int powerDown){
        if(powerDown < MinPowerDown)
        {
            throw new System.ArgumentException("Power down must be greater than 0");
        }
        int newPowerDiff = _powerDiff - powerDown;
        return new Power(_defaultPower, newPowerDiff, _powerRate);
    }

    public Power PowerChangeRate(float powerRate){
        if(powerRate < MinPowerRate)
        {
            throw new System.ArgumentException("Power rate must be greater than 0");
        }
        float newPowerRate = _powerRate * powerRate; // 上昇割合は(和ではなく)積で表す。
        return new Power(_defaultPower, _powerDiff, newPowerRate);
    }

    public Power PowerRateClear(float powerRate){
        if(powerRate < MinPowerRate)
        {
            throw new System.ArgumentException("Power rate must be greater than 0");
        }
        return PowerChangeRate(1.0f / powerRate);
    }

    public Power PowerReset(){
        return new Power(_defaultPower, 0, 1.0f);
    }

    public int DefaultPower => _defaultPower;
    public int PowerDiff => _powerDiff;
    public float PowerRate => _powerRate;
    public int CurrentPower => _currentPower;
}
