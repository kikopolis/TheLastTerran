using Effects;
using UnityEngine;

public class TriggerableHeal : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerHealth.RegisterHealEffect(new HealEffect(30));
        }
    }
}