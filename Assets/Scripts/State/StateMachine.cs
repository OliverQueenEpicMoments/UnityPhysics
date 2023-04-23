using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {
    public string CustomName;

    private State MainStatetype;

    public State CurrentState { get; private set; }
    private State NextState;

    void Update() {
        if (NextState != null) SetState(NextState);
        CurrentState?.OnUpdate();
    }

    private void SetState(State newstate) {
        NextState = null;
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

    private void Awake() {
        SetNextStateToMain();
    }

    private void OnValidate() {
        if (MainStatetype == null) {
            if (CustomName == "Combat") MainStatetype = new IdleCombatState();
        }
    }
}