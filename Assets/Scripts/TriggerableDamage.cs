using Effects;
using UnityEngine;

public class TriggerableDamage : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerHealth.RegisterDamageEffect(new DamageEffect(30));
        }
    }
}