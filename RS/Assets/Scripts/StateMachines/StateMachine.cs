using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//store the current state and change state
public abstract class StateMachine : MonoBehaviour   //have this script on player and enemies
{
    private State currentState;     //get current state

    public void SwitchState(State newState)
    {
        //run state exit
        currentState?.Exit();
        //set the new state
        currentState = newState;
        //run state enter
        currentState?.Enter();
    }
    
    // Update is called once per frame
    private void Update()
    {
        currentState?.Tick(Time.deltaTime);     //run tick function if current state is not null
    }
}
