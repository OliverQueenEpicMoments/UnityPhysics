using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEntryState : State {
    public override void OnEnter(StateMachine stateMachine) {
        base.OnEnter(stateMachine);

        State NextState = (State)new EntryState();
        statemachine.SetNextState(NextState);
    }
}