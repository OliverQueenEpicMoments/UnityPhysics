using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBaseState : State {
    public float Duration;
    public Animator animator;
    protected bool ShouldCombo;
    protected int AttackIndex;

    protected Collider2D HitCollider;
    private List<Collider2D> CollidersDamaged;
    private GameObject HitEffectPrefab;
    private float AttackPressedTimer = 0;

    public override void OnEnter(StateMachine stateMachine) {
        base.OnEnter(stateMachine);
        animator = GetComponent<Animator>();
        CollidersDamaged = new List<Collider2D>();
        HitCollider = GetComponent<ComboCharacter>().Hitbox;
        HitEffectPrefab = GetComponent<ComboCharacter>().HitEffect;
    }

    public override void OnUpdate() {
        base.OnUpdate();
        AttackPressedTimer -= Time.deltaTime;

        if (animator.GetFloat("Weapon.Active") > 0) Attack();
        if (Input.GetMouseButtonDown(0)) AttackPressedTimer = 2;
        if (animator.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0) ShouldCombo = true;
    }

    public override void OnExit() {
        base.OnExit();
    }

    protected void Attack() {
        Collider2D[] CollidersToDamage = new Collider2D[10];
        ContactFilter2D Filter = new();
        Filter.useTriggers = true;
        int ColliderCount = Physics2D.OverlapCollider(HitCollider, Filter, CollidersToDamage);

        for (int i = 0; i < ColliderCount; i++) {
            if (!CollidersDamaged.Contains(CollidersToDamage[i])) { 
                TeamComponent HitTeamComponent = CollidersToDamage[i].GetComponentInChildren<TeamComponent>();

                if (HitTeamComponent && HitTeamComponent.TeamIndex == TeamIndex.Enemy) {
                    GameObject.Instantiate(HitEffectPrefab, CollidersToDamage[i].transform);
                    Debug.Log("Enemy has taken " + AttackIndex + " damage");
                    CollidersDamaged.Add(CollidersToDamage[i]);
                }
            }
        }
    }
}