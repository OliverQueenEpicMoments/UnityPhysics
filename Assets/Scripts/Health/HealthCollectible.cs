using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour {
    [SerializeField] private float HealthRestore;
    [SerializeField] private AudioClip Heal;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            SoundManager.Instance.PlaySound(Heal);
            collision.GetComponent<Health>().AddHealth(HealthRestore);
            gameObject.SetActive(false);
        }
    }
}