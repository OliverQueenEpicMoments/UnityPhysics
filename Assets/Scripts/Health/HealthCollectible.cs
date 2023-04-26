using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour {
    [SerializeField] private float HealthRestore;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) { 
            collision.GetComponent<Health>().AddHealth(HealthRestore);
            gameObject.SetActive(false);
        }
    }
}