using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    [SerializeField] private float StartingHealth;
    private Animator animator;
    public float CurrentHealth { get; private set; }

    public void Awake() {
        CurrentHealth = StartingHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage) {
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, StartingHealth);

        if (CurrentHealth > 1) {
            animator.SetTrigger("IsHit");
        } else {
            animator.SetTrigger("Death");
        }
    }

    public void AddHealth(float heal) {
        CurrentHealth = Mathf.Clamp(CurrentHealth + heal, 0, StartingHealth);
    }
}