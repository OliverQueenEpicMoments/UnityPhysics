using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {
    private State MainStatetype;
    public State CurrentState { get; private set; }
    private State NextState;

    void Update() {
        if (NextState != null) {
            SetState(NextState);
            NextState = null;
        }
        CurrentState?.OnUpdate();
    }

    private void SetState(State newstate) { 
        CurrentState?.OnExit();

        CurrentState = newstate;
        CurrentState.OnEnter(this);
    }

    public void SetNextState(State newstate) { 
        if (newstate != null) NextState = newstate;
    }

    private void LateUpdate() {
        CurrentState?.OnLateUpdate();
    }

    private void FixedUpdate() {
        CurrentState?.OnFixedUpdate();
    }

    public void SetNextStateToMain() {
        NextState = MainStatetype;
    }
}