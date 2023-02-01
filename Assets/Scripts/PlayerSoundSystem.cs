using System;
using UnityEngine;

public class PlayerSoundSystem : MonoBehaviour {
    public static PlayerSoundSystem instance { get; private set; }
    [ SerializeField ]
    private AudioSource audioSource;
    [ SerializeField ]
    private AudioClip[] footstepSoundDirt;
    [ SerializeField ]
    private AudioClip[] footstepSoundGrass;
    [ SerializeField ]
    private AudioClip[] footstepSoundMetal;
    [ SerializeField ]
    private AudioClip[] footstepSoundConcrete;
    [ SerializeField ]
    private AudioClip[] footstepSoundWood;
    [ SerializeField ]
    private AudioClip[] footstepSoundWater;
    [ SerializeField ]
    private AudioClip[] footstepSoundGravel;
    [ SerializeField ]
    private AudioClip[] footstepSoundSnow;
    [ SerializeField ]
    private AudioClip[] footstepSoundSand;
    [ SerializeField ]
    private AudioClip[] footstepSoundMud;
    [ SerializeField ]
    private AudioClip[] dashSound;
    [ SerializeField ]
    private AudioClip[] jumpSound;
    [ SerializeField ]
    private AudioClip[] landSound;
    [ SerializeField ]
    private AudioClip[] crouchSound;
    [ SerializeField ]
    private AudioClip[] sprintSound;
    [ SerializeField ]
    private AudioClip[] hurtSound;
    [ SerializeField ]
    private AudioClip[] severeHurtSound;
    [ SerializeField ]
    private AudioClip[] healSound;
    [ SerializeField ]
    private AudioClip[] interactSound;
    [ SerializeField ]
    private AudioClip[] deathSound;

    private void Awake() {
        instance = this;
    }

    public void PlayHurtSound() {
        // todo
    }

    public void PlaySeverelyHurtSound() {
        // todo
    }

    public void PlayHealSound() {
        // todo
    }
}