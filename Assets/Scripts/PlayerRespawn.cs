using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour {
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private AudioClip SpawnSound;
    private Health PlayerHealth;
    private UIManager UI;
    private int Lives = 3;

    private void Awake() {
        PlayerHealth = GetComponent<Health>();
        UI = FindObjectOfType<UIManager>();
    }

    public void CheckRespawn() {
        Lives--;
        Debug.Log(Lives);
        if (Lives <= 0) {
            Destroy(gameObject);
            UI.GameOver();

            return;
        }

        SoundManager.Instance.PlaySound(SpawnSound);
        transform.position = SpawnPoint.position;
        PlayerHealth.Respawn();
    }
}