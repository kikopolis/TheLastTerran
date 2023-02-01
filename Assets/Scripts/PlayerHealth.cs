using System.Collections;
using System.Collections.Generic;
using Effects;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
    public static PlayerHealth instance { get; private set; }
    private readonly List<DamageEffect> damageEffects = new();
    private readonly List<HealEffect> healEffects = new();

    [ Header("Basics") ]
    [ SerializeField ]
    private float animationSpeed = 0.5f;
    [ SerializeField ]
    private PlayerSoundSystem soundSystem;

    [ Header("Colors") ]
    [ SerializeField ]
    private Color hpBarColor;
    [ SerializeField ]
    private Color hpChaserColor;
    [ SerializeField ]
    private Color hpBarHurtFlashColor;
    [ SerializeField ]
    private Color hpBarHealChaserColor;
    [ SerializeField ]
    private Color hpBarHurtChaserColor;
    [ SerializeField ]
    private Color damagedOverlayColor;

    [ Header("Health") ]
    [ SerializeField ]
    private float previousHealth;
    [ SerializeField ]
    private float currentHealth;
    [ SerializeField ]
    private float minHealth;
    [ SerializeField ]
    private float maxHealth = 100f;
    [ SerializeField ]
    private Image hpBar;
    [ SerializeField ]
    private Image hpBarChaser;

    [ Header("Hurt") ]
    [ SerializeField ]
    private float severelyHurtThreshold = 40f;
    [ SerializeField ]
    private float hurtThreshold = 60f;
    [ SerializeField ]
    private Image severelyHurtOverlay;
    [ SerializeField ]
    private Image damagedOverlay;
    [ SerializeField ]
    private int hurtFlashCount = 3;
    private bool isPlayingSound;
    private float hpBarLerpTimer;
    private Coroutine hpBarGoingDownRoutine;
    private Coroutine hpBarGoingUpRoutine;
    private Coroutine hpBarFlashRoutine;
    private Coroutine hurtOverlayRoutine;

    [ Header("Regenerate") ]
    [ SerializeField ]
    private bool canNaturalRegenerate = true;
    [ SerializeField ]
    private bool canReceiveHealOverTime = true;
    [ SerializeField ]
    private float naturalRegenerationRate = 0.5f;
    [ SerializeField ]
    private float naturalRegenerationDelay = 5f;
    private float naturalRegenerationTimer;
    private bool isRegenerating;

    [ Header("Heal") ]
    [ SerializeField ]
    private Image healOverlay;

    [ Header("Death") ]
    [ SerializeField ]
    private bool canDie = true;
    [ SerializeField ]
    private bool isDead;
    [ SerializeField ]
    private Image deathOverlay;

    private void Awake() {
        instance = this;
        damageEffects.Clear();
        healEffects.Clear();
        currentHealth = maxHealth;
        previousHealth = currentHealth;
        hpBar.color = hpBarColor;
        hpBarChaser.color = hpChaserColor;
    }

    private void FixedUpdate() {
        HandleDeath();
        HandleDamage();
        HandleHeal();
        HandleNaturalRegeneration();
        UpdateUI();
        previousHealth = currentHealth;
    }

    private static bool IsInstanceNull() {
        return instance == null;
    }

    public static void RegisterDamageEffect(DamageEffect effect) {
        if (IsInstanceNull()) {
            return;
        }
        instance.damageEffects.Add(effect);
    }

    public static void RegisterHealEffect(HealEffect effect) {
        if (IsInstanceNull()) {
            return;
        }
        instance.healEffects.Add(effect);
    }

    private void HandleDeath() {
        if (!(currentHealth <= minHealth) || !canDie) {
            return;
        }
        // isDead = true;
        // deathOverlay.gameObject.SetActive(true);
    }

    private void HandleDamage() {
        HandleDirectDamage();
        HandleDamageOverTime();
    }

    private void HandleDirectDamage() {
        for (var i = damageEffects.Count - 1; i >= 0; i--) {
            var effect = damageEffects[i];
            if (effect.overTime) {
                continue;
            }
            DealDamage(effect.damage);
            damageEffects.RemoveAt(i);
        }
    }

    private void HandleDamageOverTime() {
        for (var i = damageEffects.Count - 1; i >= 0; i--) {
            var effect = damageEffects[i];
            if (!effect.overTime) {
                continue;
            }
            // tick once per seconds
            effect.tickTimer -= Time.deltaTime;
            if (effect.tickTimer > 0) {
                continue;
            }
            // do the damage, total damage divided by ticks
            DealDamage(effect.damage / effect.ticksRemaining, false);
            // decrement ticks remaining
            effect.ticksRemaining--;
            // remove if no more ticks left, otherwise reset timer
            if (effect.ticksRemaining <= 0) {
                damageEffects.RemoveAt(i);
            } else {
                effect.tickTimer = 1f;
            }
        }
    }

    private void DealDamage(float damage, bool playSound = true) {
        previousHealth = currentHealth;
        currentHealth -= damage;
        if (playSound) {
            if (currentHealth <= severelyHurtThreshold) {
                soundSystem.PlaySeverelyHurtSound();
            } else {
                soundSystem.PlayHurtSound();
            }
        }
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
    }

    private void HandleHeal() {
        HandleDirectHeal();
        HandleHealOverTime();
    }

    private void HandleDirectHeal() {
        for (var i = healEffects.Count - 1; i >= 0; i--) {
            var effect = healEffects[i];
            // if (effect.overTime) {
            //     continue;
            // }
            Heal(effect.amount);
            healEffects.RemoveAt(i);
        }
    }

    private void HandleHealOverTime() {
        // todo
    }

    private void HandleNaturalRegeneration() {
        // todo
    }

    private void Heal(float amount, bool playSound = true) {
        previousHealth = currentHealth;
        currentHealth += amount;
        // cancel all hurt effects
        // if (severelyHurtOverlayRoutine != null && currentHealth>severelyHurtThreshold) {
        //     StopCoroutine(severelyHurtOverlayRoutine);
        // severelyHurtOverlayRoutine = null;
        // }
        if (hurtOverlayRoutine != null) {
            StopCoroutine(hurtOverlayRoutine);
            hurtOverlayRoutine = null;
        }
        if (hpBarFlashRoutine != null) {
            StopCoroutine(hpBarFlashRoutine);
            hpBarFlashRoutine = null;
        }
        if (playSound) {
            soundSystem.PlayHealSound();
        }
        ResetHpBarColor();
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
    }

    private void UpdateUI() {
        UpdateHealthBar();
        UpdateHurtOverlay();
        // UpdateHealOverlay();
        // UpdateDeathOverlay();
    }

    private void UpdateHealthBar() {
        var speed = animationSpeed * 3;
        var hpFill = hpBar.fillAmount;
        var hpChaserFill = hpBarChaser.fillAmount;
        var hpFillTarget = currentHealth / maxHealth;
        if (hpChaserFill > hpFillTarget) {
            // If going down
            if (hpBarGoingUpRoutine != null) {
                StopCoroutine(hpBarGoingUpRoutine);
                hpBarGoingUpRoutine = null;
            }
            hpBarFlashRoutine ??= StartCoroutine(HpBarFlashRoutine());
            hpBarGoingDownRoutine ??= StartCoroutine(HpBarGoingDownRoutine(hpFillTarget, speed, hpChaserFill));
        } else if (hpFill < hpFillTarget) {
            // If going up
            // Stop flashing and going down routine
            if (hpBarFlashRoutine != null) {
                StopCoroutine(hpBarFlashRoutine);
                hpBarFlashRoutine = null;
            }
            if (hpBarGoingDownRoutine != null) {
                StopCoroutine(hpBarGoingDownRoutine);
                hpBarGoingDownRoutine = null;
            }
            hpBarGoingUpRoutine ??= StartCoroutine(HpBarGoingUpRoutine(hpFillTarget, speed, hpFill));
        } else {
            // return the chaser to default color
            hpBarChaser.color = hpChaserColor;
            hpBarLerpTimer = 0;
        }
    }

    private IEnumerator HpBarGoingUpRoutine(float hpFillTarget, float speed, float hpFill) {
        hpBarLerpTimer = 0f;
        hpBarChaser.fillAmount = hpFillTarget;
        hpBarChaser.color = hpBarHealChaserColor;
        while (hpBar.fillAmount < hpFillTarget) {
            hpBarLerpTimer += Time.deltaTime * speed;
            var percentComplete = hpBarLerpTimer / speed;
            percentComplete *= percentComplete;
            hpBar.fillAmount = Mathf.Lerp(hpFill, hpFillTarget, percentComplete);
            yield return null;
        }
        StopCoroutine(hpBarGoingUpRoutine);
        hpBarGoingUpRoutine = null;
        // Reset the lerp timer. This leads to a visual problem if opposing transitions happen quickly
        hpBarLerpTimer = 0f;
    }

    private IEnumerator HpBarGoingDownRoutine(float hpFillTarget, float speed, float hpChaserFill) {
        hpBarLerpTimer = 0f;
        hpBar.fillAmount = hpFillTarget;
        hpBarChaser.color = hpBarHurtChaserColor;
        // wait for 0.1 seconds
        yield return new WaitForSeconds(0.25f);
        while (hpBarChaser.fillAmount > hpFillTarget) {
            hpBarLerpTimer += Time.deltaTime * speed;
            var percentComplete = hpBarLerpTimer / speed;
            percentComplete *= percentComplete;
            hpBarChaser.fillAmount = Mathf.Lerp(hpChaserFill, hpFillTarget, percentComplete);
            yield return null;
        }
        StopCoroutine(hpBarGoingDownRoutine);
        hpBarGoingDownRoutine = null;
        // Reset the lerp timer. This leads to a visual problem if opposing transitions happen quickly
        hpBarLerpTimer = 0f;
    }

    private IEnumerator HpBarFlashRoutine() {
        var flashDuration = animationSpeed / (hurtFlashCount * 2) * 2;
        var count = hurtFlashCount * 2;
        var toOriginalColor = false;
        yield return null;
        while (count > 0) {
            var toColor = toOriginalColor ? hpBarColor : hpBarHurtFlashColor;
            var lerpTimer = 0f;
            while (lerpTimer < flashDuration) {
                hpBar.color = Color.Lerp(hpBar.color, toColor, lerpTimer);
                lerpTimer += Time.deltaTime;
                yield return null;
            }
            toOriginalColor = !toOriginalColor;
            count--;
        }
        StopCoroutine(hpBarFlashRoutine);
        hpBarFlashRoutine = null;
        ResetHpBarColor();
    }

    private void ResetHpBarColor() {
        hpBar.color = hpBarColor;
    }

    private void UpdateHurtOverlay() {
        // If taking damage, flash damaged overlay
        if (!(previousHealth > currentHealth)) {
            return;
        }
        if (hurtOverlayRoutine != null) {
            StopCoroutine(hurtOverlayRoutine);
        }
        hurtOverlayRoutine = StartCoroutine(HurtOverlayRoutine());
    }

    private IEnumerator HurtOverlayRoutine() {
        damagedOverlay.gameObject.SetActive(true);
        var c1 = damagedOverlay.color;
        c1.a = 1;
        damagedOverlay.color = c1;
        // var color = damagedOverlay.color;
        // color.a = 1;
        // damagedOverlay.color = color;
        // damagedOverlay.CrossFadeAlpha(0, animationSpeed, false);
        // yield return new WaitForSeconds(animationSpeed);
        while (damagedOverlay.color.a > 0f) {
            var c2 = damagedOverlay.color;
            c2.a -= Time.deltaTime * animationSpeed;
            damagedOverlay.color = c2;
        }
        yield return null;
        damagedOverlay.gameObject.SetActive(false);
    }
}