using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBaseState : State {
    public float Duration;
    public Animator animator;
    protected bool ShouldCombo;
    protected int AttackIndex;

    public override void OnEnter(StateMachine stateMachine) {
        base.OnEnter(stateMachine);
        animator = GetComponent<Animator>();
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (Input.GetMouseButtonDown(0)) ShouldCombo = true;
    }

    public override void OnExit() {
        base.OnExit();
    }
}