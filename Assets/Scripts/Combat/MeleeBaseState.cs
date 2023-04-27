using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBaseState : State {
    public float Duration;
    public Animator animator;
    protected bool ShouldCombo;
    protected int AttackIndex;
    protected AudioClip AttackSound;

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
        AttackSound = GetComponent<ComboCharacter>().AttackSound;
    }

    public override void OnUpdate() {
        base.OnUpdate();
        AttackPressedTimer -= Time.deltaTime;

        if (animator.GetFloat("Weapon.Active") > 0) Attack();
        if (Input.GetMouseButtonDown(0)) AttackPressedTimer = 2.5f;
        if (animator.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0) ShouldCombo = true;
    }

    public override void OnExit() {
        base.OnExit();
    }

    protected void Attack() {
        //SoundManager.Instance.PlaySound(AttackSound);
        Collider2D[] CollidersToDamage = new Collider2D[10];
        ContactFilter2D Filter = new();
        Filter.useTriggers = true;
        int ColliderCount = Physics2D.OverlapCollider(HitCollider, Filter, CollidersToDamage);

        for (int i = 0; i < ColliderCount; i++) {
            if (!CollidersDamaged.Contains(CollidersToDamage[i])) { 
                TeamComponent HitTeamComponent = CollidersToDamage[i].GetComponentInChildren<TeamComponent>();

                if (HitTeamComponent && HitTeamComponent.TeamIndex == TeamIndex.Enemy) {
                    if (HitEffectPrefab != null) GameObject.Instantiate(HitEffectPrefab, CollidersToDamage[i].transform);
                    HitTeamComponent.GetComponent<Health>().TakeDamage(AttackIndex);
                    Debug.Log("Enemy health " + HitTeamComponent.GetComponent<Health>().CurrentHealth);
                    CollidersDamaged.Add(CollidersToDamage[i]);
                }
            }
        }
    }
}