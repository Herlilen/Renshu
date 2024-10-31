using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    //reference the player
    protected PlayerStateMachine _playerStateMachine;

    public PlayerBaseState(PlayerStateMachine _playerStateMachine)
    {
        this._playerStateMachine = _playerStateMachine;
    }
    
    protected void Move(float deltaTime)    //move without input
    {
        Move(Vector3.zero, deltaTime);
    }
    
    protected void Move(Vector3 motion, float deltaTime)
    {
        _playerStateMachine.CharacterController.Move((motion + _playerStateMachine.ForceReceiver.Movement) * deltaTime);
    }

    protected void FaceTarget()
    {
        if(_playerStateMachine.Targeter.currentTarget == null) return;
        
        Vector3 lookPos = _playerStateMachine.Targeter.currentTarget.transform.position - 
                          _playerStateMachine.transform.position;
        lookPos.y = 0;
        
        _playerStateMachine.transform.rotation = Quaternion.LookRotation(lookPos);
    }
    
}
