using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingState : PlayerBaseState
{
    private readonly int TargetingBlendTreeHash = Animator.StringToHash("TargetingBlendTree");
    
    private readonly int TargetingForwardSpeedHash = Animator.StringToHash("TargetingForwardSpeed");
    private readonly int TargetingRightSpeedHash = Animator.StringToHash("TargetingRightSpeed");
    
    private const float AnimatorDampTime = 0.1f;
    
    public PlayerTargetingState(PlayerStateMachine _playerStateMachine) : base(_playerStateMachine) {}

    public override void Enter()
    {
        _playerStateMachine.InputReader.TargetingEvent += OnTargetCancel;
        
        //_playerStateMachine.Animator.Play(TargetingBlendTreeHash);
        _playerStateMachine.Animator.CrossFadeInFixedTime(TargetingBlendTreeHash, AnimatorDampTime);
    }
    
    public override void Tick(float deltaTime)
    {
        if (_playerStateMachine.InputReader.isAttacking)
        {
            _playerStateMachine.SwitchState(new PlayerAttackingState(_playerStateMachine, 0));
            return;
        }
        
        if(_playerStateMachine.Targeter.currentTarget == null)
        {
            _playerStateMachine.SwitchState(new PlayerFreeLookState(_playerStateMachine));
            return;
        }
        
        Vector3 movement = CalculateMovement();
        Move(movement * _playerStateMachine.TargetingMovementSpeed, deltaTime);

        UpdateAnimator(deltaTime);
        
        FaceTarget();
    }

    public override void Exit()
    {
        _playerStateMachine.InputReader.TargetingEvent -= OnTargetCancel;
    }
    
    private void OnTargetCancel()
    {
        _playerStateMachine.Targeter.ClearTarget();
        
        _playerStateMachine.SwitchState(new PlayerFreeLookState(_playerStateMachine));
    }

    private Vector3 CalculateMovement()
    {
        Vector3 movement = new Vector3();

        movement += _playerStateMachine.transform.right * _playerStateMachine.InputReader.MovementValue.x;
        movement += _playerStateMachine.transform.forward * _playerStateMachine.InputReader.MovementValue.y;

        return movement;
    }
    
    private void UpdateAnimator(float deltaTime)
    {
        if (_playerStateMachine.InputReader.MovementValue.y == 0)
        {
            _playerStateMachine.Animator.SetFloat(TargetingForwardSpeedHash, 0, AnimatorDampTime, deltaTime);
        }
        else
        {
            float value = _playerStateMachine.InputReader.MovementValue.y;
            _playerStateMachine.Animator.SetFloat(TargetingForwardSpeedHash, value, AnimatorDampTime, deltaTime);
        }
        
        if (_playerStateMachine.InputReader.MovementValue.x == 0)
        {
            _playerStateMachine.Animator.SetFloat(TargetingRightSpeedHash, 0, AnimatorDampTime, deltaTime);
        }
        else
        {
            float value = _playerStateMachine.InputReader.MovementValue.x;
            _playerStateMachine.Animator.SetFloat(TargetingRightSpeedHash, value, AnimatorDampTime, deltaTime);
        }
    }
}
