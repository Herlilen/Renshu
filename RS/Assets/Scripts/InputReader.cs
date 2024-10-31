using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, PlayerControls.IPlayerActions
{
    public Vector2 MovementValue {get; private set;}
    public Vector2 LookValue {get; private set;}
    public bool isAttacking {get; private set;}
    
    public event Action JumpEvent;
    public event Action DodgeEvent;
    public event Action TargetingEvent;
    
    private PlayerControls _playerControls;
    
    private void Start()
    {
        _playerControls = new PlayerControls();
        _playerControls.Player.SetCallbacks(this);
        
        _playerControls.Player.Enable();
    }

    private void OnDestroy()
    {
        _playerControls.Player.Disable();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        
        JumpEvent?.Invoke();
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        
        DodgeEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookValue = context.ReadValue<Vector2>();
    }

    public void OnTargeting(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        
        TargetingEvent?.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAttacking = true;
        }
        else if (context.canceled)
        {
            isAttacking = false;
        }
    }
}
