using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    [Header("Health")]
    [SerializeField] private float StartingHealth;
    public float CurrentHealth { get; private set; }
    private Animator animator;

    [Header("IFrames")]
    [SerializeField] private float IFrameDuration;
    [SerializeField] private int NumberOfFlashes;
    private SpriteRenderer spriterenderer;
    private bool Invulnerable = false;

    [Header("Audio")]
    [SerializeField] private AudioClip HurtSound;
    [SerializeField] private AudioClip DeathSound;

    public void Awake() {
        CurrentHealth = StartingHealth;
        animator = GetComponent<Animator>();
        spriterenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage) {
        if (!Invulnerable) CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, StartingHealth);

        if (CurrentHealth > 0) {
            animator.SetTrigger("IsHit");
            StartCoroutine(Invulnerability());
            SoundManager.Instance.PlaySound(HurtSound);
        } else {
            animator.SetTrigger("Death");
            SoundManager.Instance.PlaySound(DeathSound);
        }
    }

    public void AddHealth(float heal) {
        CurrentHealth = Mathf.Clamp(CurrentHealth + heal, 0, StartingHealth);
    }

    private IEnumerator Invulnerability() {
        Invulnerable = true;
        Physics2D.IgnoreLayerCollision(3, 6, true);
        for (int i = 0; i < NumberOfFlashes; i++) {
            spriterenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(IFrameDuration / (NumberOfFlashes * 2));
            spriterenderer.color = Color.white;
            yield return new WaitForSeconds(IFrameDuration / (NumberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(3, 6, false);
        Invulnerable = false;
    }
}