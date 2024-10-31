using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackingState : PlayerBaseState
{
    private float previousFrameTime;
    private bool alreadyAppliedForce;
    
    private Attack attack;
    
    public PlayerAttackingState(PlayerStateMachine _playerStateMachine, int attackIndex) : base(_playerStateMachine)
    {
        attack = _playerStateMachine.Attacks[attackIndex];
    }

    public override void Enter()
    {
        _playerStateMachine.Animator.CrossFadeInFixedTime(attack.animationName, attack.transitionDuration);
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        
        FaceTarget();
        
        float normalizedTime = GetNormalizedTime();
        
        if(normalizedTime < 1)
        {
            if (normalizedTime >= attack.forceTime)
            {
                TryApplyForce();
            }
            
            if (_playerStateMachine.InputReader.isAttacking)
            {
                TryComboAttack(normalizedTime);
            }
        }
        else
        {
            if (_playerStateMachine.Targeter.currentTarget != null)
            {
                _playerStateMachine.SwitchState(new PlayerTargetingState(_playerStateMachine));
            }
            else
            {
                _playerStateMachine.SwitchState(new PlayerFreeLookState(_playerStateMachine));
            }
        }
        
        previousFrameTime = normalizedTime;
    }
    
    public override void Exit()
    {
    }
    
    private float GetNormalizedTime()
    {
        AnimatorStateInfo currentInfo = _playerStateMachine.Animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = _playerStateMachine.Animator.GetNextAnimatorStateInfo(0);
        
        if(_playerStateMachine.Animator.IsInTransition(0) && nextInfo.IsTag("Attack"))
        {
            return nextInfo.normalizedTime;
        }
        else if(!_playerStateMachine.Animator.IsInTransition(0) && currentInfo.IsTag("Attack"))
        {
            return currentInfo.normalizedTime;
        }else
        {
            return 0;
        }
    }
    
    private void TryComboAttack(float normalizedTime)
    {
        if (attack.comboStateIndex == -1) { return; }
        
        if(normalizedTime < attack.comboAttackTime) { return; }
        
        _playerStateMachine.SwitchState(new PlayerAttackingState(_playerStateMachine, attack.comboStateIndex));
    }

    private void TryApplyForce()
    {
        if(alreadyAppliedForce){return;}
        
        _playerStateMachine.ForceReceiver.AddForce(_playerStateMachine.transform.forward * attack.force);

        alreadyAppliedForce = true;
    }
}
