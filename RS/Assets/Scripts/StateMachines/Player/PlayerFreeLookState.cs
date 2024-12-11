using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState
{
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLookBlendTree");

    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    private const float AnimatorDampTime = 0.1f;
    
    //constructor
    public PlayerFreeLookState(PlayerStateMachine _playerStateMachine) : base(_playerStateMachine) {}

    public override void Enter()
    {
        _playerStateMachine.InputReader.TargetingEvent += OnTarget;
        
        //_playerStateMachine.Animator.Play(FreeLookBlendTreeHash);
        _playerStateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTreeHash, AnimatorDampTime);
    }
    
    public override void Tick(float deltaTime)
    {
        if (_playerStateMachine.InputReader.isAttacking)
        {
            _playerStateMachine.SwitchState(new PlayerAttackingState(_playerStateMachine, 0));
            return;
        }
        
        Vector3 movement = CalculateMovement();
        
        Move(movement * _playerStateMachine.FreeLookMovementSpeed, deltaTime);

        if (_playerStateMachine.InputReader.MovementValue == Vector2.zero)
        {
            _playerStateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }
         
        float value = Mathf.Abs(_playerStateMachine.InputReader.MovementValue.x) + Mathf.Abs(_playerStateMachine.InputReader.MovementValue.y);
        _playerStateMachine.Animator.SetFloat(FreeLookSpeedHash, value, AnimatorDampTime, deltaTime);
        
        FaceMovementDirection(movement, deltaTime);
    }
    
    public override void Exit()
    {
        _playerStateMachine.InputReader.TargetingEvent -= OnTarget;
    }
    
    private Vector3 CalculateMovement()
    {
        Vector3 forward = _playerStateMachine.MainCameraTransform.forward;
        Vector3 right = _playerStateMachine.MainCameraTransform.right;
        
        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();
        
        return forward * _playerStateMachine.InputReader.MovementValue.y + 
               right * _playerStateMachine.InputReader.MovementValue.x;
    }
    
    private void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        _playerStateMachine.transform.rotation = Quaternion.Lerp(
            _playerStateMachine.transform.rotation, 
            Quaternion.LookRotation(movement),
            deltaTime * _playerStateMachine.RotationSmoothValue);
    }
    
    private void OnTarget()
    {
        if(!_playerStateMachine.Targeter.SelectTarget()) return;
        
        _playerStateMachine.SwitchState(new PlayerTargetingState(_playerStateMachine));
    }
}
