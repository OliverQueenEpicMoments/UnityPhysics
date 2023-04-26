using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField] private Health PlayerHealth;
    [SerializeField] private Image TotalHealthBar;
    [SerializeField] private Image CurrentHealthBar;

    void Start() {
        TotalHealthBar.fillAmount = PlayerHealth.CurrentHealth / 10;
    }

    void Update() {
        CurrentHealthBar.fillAmount = PlayerHealth.CurrentHealth / 10;
    }
}