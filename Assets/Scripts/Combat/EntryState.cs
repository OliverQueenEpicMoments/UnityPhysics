using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryState : MeleeBaseState {
    public override void OnEnter(StateMachine stateMachine) {
        base.OnEnter(stateMachine);

        // Attack
        AttackIndex = 1;
        Duration = 0.5f;
        animator.SetTrigger("Attack" + AttackIndex);
        Debug.Log("Player attack " + AttackIndex + " fired");
    }

    public override void OnExit() {
        base.OnExit();
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (FixedTimer >= Duration) {
            if (ShouldCombo) statemachine.SetNextState(new FinisherState());
            else statemachine.SetNextStateToMain();
        }
    }
}