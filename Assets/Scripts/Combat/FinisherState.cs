using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherState : MeleeBaseState {
    public override void OnEnter(StateMachine stateMachine) {
        base.OnEnter(stateMachine);

        // Attack
        AttackIndex = 2;
        Duration = 0.5f;
        animator.SetTrigger("Attack" + AttackIndex);
        SoundManager.Instance.PlaySound(AttackSound);
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (FixedTimer >= Duration) {
            statemachine.SetNextStateToMain();
        }
    }
}
