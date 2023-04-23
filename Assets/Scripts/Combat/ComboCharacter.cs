using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboCharacter : MonoBehaviour {
    private StateMachine MeleeStateMachine;

    public Collider2D Hitbox;
    public GameObject HitEffect;

    void Start() {
        MeleeStateMachine = GetComponent<StateMachine>();
    }

    void Update() {
        if (Input.GetMouseButton(0) && MeleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState)) {
            MeleeStateMachine.SetNextState(new EntryState());
        }
    }
}