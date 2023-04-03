using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField] private Transform SpawnLocation = null;
    [SerializeField] private GameObject Prefab;

    void Start() {
        
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Instantiate(Prefab, SpawnLocation);
        }
    }
}