using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint
{
    const int MinHitPoint = 0;
    const int MinDamage = 0;
    const int MinHeal = 0;
    const float MinDamageRate = 0.0f;
    const float MaxDamageRate = 1.0f;
    const float MinHealRate = 0.0f;
    const float MaxHealRate = 1.0f;
    readonly int _maxHitPoint;
    readonly int _currentHitPoint;

    public HitPoint(int maxHitPoint, int currentHitPoint)
    {
        if(maxHitPoint <= MinHitPoint)
        {
            throw new System.ArgumentException("Max hit point must be greater than 0");
        }
        if(currentHitPoint < MinHitPoint)
        {
            throw new System.ArgumentException("Current hit point must be greater than 0");
        }
        _maxHitPoint = maxHitPoint;
        _currentHitPoint = currentHitPoint;
    }

    public HitPoint(int maxHitPoint){
        if(maxHitPoint < MinHitPoint)
        {
            throw new System.ArgumentException("Max hit point must be greater than 0");
        }
        _maxHitPoint = maxHitPoint;
        _currentHitPoint = maxHitPoint;
    }

    public HitPoint Damage(int damage){
        if(damage < MinDamage)
        {
            throw new System.ArgumentException("Damage must be greater than 0");
        }
        int newHitPoint = _currentHitPoint - damage;
        if(newHitPoint < MinHitPoint)
        {
            newHitPoint = MinHitPoint;
        }
        return new HitPoint(_maxHitPoint, newHitPoint);
    }

    public HitPoint DamageRateFromMaxHP(float damageRate){
        if(damageRate < MinDamageRate)
        {
            throw new System.ArgumentException("Damage rate must be greater than 0");
        }
        if(damageRate > MaxDamageRate)
        {
            throw new System.ArgumentException("Damage rate must be less than 1");
        }
        int newHitPoint = _currentHitPoint - (int)(_maxHitPoint * damageRate);
        if(newHitPoint < MinHitPoint)
        {
            newHitPoint = MinHitPoint;
        }
        return new HitPoint(_maxHitPoint, newHitPoint);
    }

    public HitPoint DamageRateFromCurrentHP(float damageRate){
        if(damageRate < MinDamageRate)
        {
            throw new System.ArgumentException("Damage rate must be greater than 0");
        }
        if(damageRate > MaxDamageRate)
        {
            throw new System.ArgumentException("Damage rate must be less than 1");
        }
        int newHitPoint = (int)(_currentHitPoint * (1 - damageRate));
        if(newHitPoint < MinHitPoint)
        {
            newHitPoint = MinHitPoint;
        }
        return new HitPoint(_maxHitPoint, newHitPoint);
    }

    public HitPoint Heal(int heal){
        if(heal < MinHeal)
        {
            throw new System.ArgumentException("Heal must be greater than 0");
        }
        int newHitPoint = _currentHitPoint + heal;
        if(newHitPoint > _maxHitPoint)
        {
            newHitPoint = _maxHitPoint;
        }
        return new HitPoint(_maxHitPoint, newHitPoint);
    }

    public HitPoint HealRateFromMaxHP(float healRate){
        if(healRate < MinHealRate)
        {
            throw new System.ArgumentException("Heal rate must be greater than 0");
        }
        if(healRate > MaxHealRate)
        {
            throw new System.ArgumentException("Heal rate must be less than 1");
        }
        int newHitPoint = _currentHitPoint + (int)(_maxHitPoint * healRate);
        if(newHitPoint > _maxHitPoint)
        {
            newHitPoint = _maxHitPoint;
        }
        return new HitPoint(_maxHitPoint, newHitPoint);
    }

    public HitPoint HealRateFromCurrentHP(float healRate){
        if(healRate < MinHealRate)
        {
            throw new System.ArgumentException("Heal rate must be greater than 0");
        }
        if(healRate > MaxHealRate)
        {
            throw new System.ArgumentException("Heal rate must be less than 1");
        }
        int newHitPoint = (int)(_currentHitPoint * (1 + healRate));
        if(newHitPoint > _maxHitPoint)
        {
            newHitPoint = _maxHitPoint;
        }
        return new HitPoint(_maxHitPoint, newHitPoint);
    }

    public int MaxHitPoint => _maxHitPoint;
    public int CurrentHitPoint => _currentHitPoint;
}
