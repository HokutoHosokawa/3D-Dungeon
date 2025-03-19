using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation
{
    private Animator _animator;

    public EnemyAnimation(Animator animator)
    {
        _animator = animator;
    }

    public void Attack()
    {
        _animator.SetTrigger("AttackTrigger");
    }

    public void Walk()
    {
        _animator.SetBool("IsWalking", true);
    }

    public void Idle()
    {
        _animator.SetBool("IsWalking", false);
    }

    public bool IsAttacking()
    {
        return _animator.GetBool("IsAttacking");
    }
}
