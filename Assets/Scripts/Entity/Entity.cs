using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    const float MinPowerUpRate = 0.0f;
    const float MinPowerDownRate = 0.0f;
    const float MinDefenceUpRate = 0.0f;
    const float MinDefenceDownRate = 0.0f;
    private Power _powerValue;
    private Defence _defenceValue;
    private HitPoint _hp;

    public Entity(int defaultPower, int defaultDefence, int maxHitPoint)
    {
        _powerValue = new Power(defaultPower);
        _defenceValue = new Defence(defaultDefence);
        _hp = new HitPoint(maxHitPoint);
    }

    public Entity((int defaultPower, int defaultDefence, int maxHitPoint) defaultStatus)
    {
        _powerValue = new Power(defaultStatus.defaultPower);
        _defenceValue = new Defence(defaultStatus.defaultDefence);
        _hp = new HitPoint(defaultStatus.maxHitPoint);
    }

    public void UpdatePower(Power power)
    {
        _powerValue = power;
    }

    public void UpdateDefence(Defence defence)
    {
        _defenceValue = defence;
    }

    public void UpdateHitPoint(HitPoint hp)
    {
        _hp = hp;
    }

    public void DefaultPowerChange(int defaultPower)
    {
        _powerValue = new Power(defaultPower, _powerValue.PowerDiff, _powerValue.PowerRate);
    }

    public void PowerUp(int powerUp)
    {
        _powerValue = _powerValue.PowerUp(powerUp);
    }

    public void PowerDown(int powerDown)
    {
        _powerValue = _powerValue.PowerDown(powerDown);
    }

    public void PowerRate(float powerRate)
    {
        _powerValue = _powerValue.PowerChangeRate(powerRate);
    }

    public void PowerUpRate(float powerUpRate)
    {
        if(powerUpRate < MinPowerUpRate)
        {
            throw new System.ArgumentException("Power up rate must be greater than 0");
        }
        // 40%上昇のようなテキストと一貫性をもたせる
        _powerValue = _powerValue.PowerChangeRate(1.0f + powerUpRate * 0.01f);
    }

    public void PowerDownRate(float powerDownRate)
    {
        if(powerDownRate < MinPowerDownRate)
        {
            throw new System.ArgumentException("Power down rate must be greater than 0");
        }
        // 40%減少のようなテキストと一貫性をもたせる
        _powerValue = _powerValue.PowerChangeRate(1.0f - powerDownRate * 0.01f);
    }

    public void PowerRateClear(float powerRate)
    {
        _powerValue = _powerValue.PowerRateClear(powerRate);
    }

    public void PowerReset()
    {
        _powerValue = _powerValue.PowerReset();
    }

    public void DefaultDefenceChange(int defaultDefence)
    {
        _defenceValue = new Defence(defaultDefence, _defenceValue.DefenceDiff, _defenceValue.DefenceRate);
    }

    public void DefenceUp(int defenceUp)
    {
        _defenceValue = _defenceValue.DefenceUp(defenceUp);
    }

    public void DefenceDown(int defenceDown)
    {
        _defenceValue = _defenceValue.DefenceDown(defenceDown);
    }

    public void DefenceRate(float defenceRate)
    {
        _defenceValue = _defenceValue.DefenceChangeRate(defenceRate);
    }

    public void DefenceUpRate(float defenceUpRate)
    {
        // 40%上昇のようなテキストと一貫性をもたせる
        if(defenceUpRate < MinDefenceUpRate)
        {
            throw new System.ArgumentException("Defence up rate must be greater than 0");
        }
        _defenceValue = _defenceValue.DefenceChangeRate(1.0f + defenceUpRate * 0.01f);
    }

    public void DefenceDownRate(float defenceDownRate)
    {
        // 40%減少のようなテキストと一貫性をもたせる
        if(defenceDownRate < MinDefenceDownRate)
        {
            throw new System.ArgumentException("Defence down rate must be greater than 0");
        }
        _defenceValue = _defenceValue.DefenceChangeRate(1.0f - defenceDownRate * 0.01f);
    }

    public void DefenceRateClear(float defenceRate)
    {
        _defenceValue = _defenceValue.DefenceRateClear(defenceRate);
    }

    public void DefenceReset()
    {
        _defenceValue = _defenceValue.DefenceReset();
    }

    public void MaxHitPointChange(int maxHitPoint)
    {
        _hp = new HitPoint(maxHitPoint, maxHitPoint - (_hp.MaxHitPoint - _hp.CurrentHitPoint));
    }

    public virtual void Damage(int damage)
    {
        _hp = _hp.Damage(damage);
    }

    public virtual void DamageRateFromMaxHP(float damageRate)
    {
        _hp = _hp.DamageRateFromMaxHP(damageRate);
    }

    public virtual void DamageRateFromCurrentHP(float damageRate)
    {
        _hp = _hp.DamageRateFromCurrentHP(damageRate);
    }

    public void Heal(int heal)
    {
        _hp = _hp.Heal(heal);
    }

    public void HealRateFromMaxHP(float healRate)
    {
        _hp = _hp.HealRateFromMaxHP(healRate);
    }

    public void HealRateFromCurrentHP(float healRate)
    {
        _hp = _hp.HealRateFromCurrentHP(healRate);
    }

    public Power PowerValue => _powerValue;
    public Defence DefenceValue => _defenceValue;
    public HitPoint HP => _hp;
}
