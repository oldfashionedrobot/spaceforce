using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceForce.Combat {
  public enum WeaponShootType {
    HitScan,
    Projectile
  }

  [System.Serializable]
  public struct WeaponDamage {
    public float head;
    public float torso;
    public float limb;
  }

  [System.Serializable]
  public struct WeaponAnimations {
    public AnimationClip recoilAnimation;
    public AnimationClip idleAnimation;

    // TODO: maybe
    // public AnimationClip reloadAnimation;
    // public AnimationClip meleeAnimation;
  }

  [System.Serializable]
  public struct WeaponAudio {
    public AudioClip fireClip;
    public AudioClip reloadClip;
  }

  [CreateAssetMenu(fileName = "Weapon", menuName = "SpaceForce/New Weapon", order = 0)]
  public class Weapon : ScriptableObject {
    // weapon stats
    public float fireRate;
    public WeaponDamage damage;
    public int clipSize;

    // weapon mechanics
    public WeaponShootType shootType;
    public bool isAutomatic;

    // effects
    public int animId;
    public WeaponAnimations animations;
    public WeaponAudio audio;
    public GameObject impactEffectPrefab;
    public GameObject impactMarkPrefab;
    public GameObject muzzleFlashPrefab;

    // objects to instantiate
    public GameObject equippedWeaponPrefab;
    public GameObject projectilePrefab;

    // in game object instances
    private GameObject weaponInstance;
    private ParticleSystem muzzleFlashParticle;

    public EquippedWeapon SetUpWeapon(Transform parentHand) {
      weaponInstance = Instantiate(equippedWeaponPrefab, parentHand);

      EquippedWeapon eqpWep = weaponInstance.GetComponent<EquippedWeapon>();
      Transform muzzlePoint = eqpWep.GetMuzzlePoint();

      muzzleFlashParticle = Instantiate(muzzleFlashPrefab, muzzlePoint).GetComponent<ParticleSystem>();

      return eqpWep;
    }

    public void Shoot(Transform muzzleTransform, Vector3 aimPoint, Vector3 aimNormal) {
      muzzleFlashParticle.Play();
      // TODO: figure out object pooling for this
      if (aimNormal != Vector3.zero) {
        // hit effect
        GameObject hitEffect = Instantiate(impactEffectPrefab, aimPoint, Quaternion.LookRotation(aimNormal));
        Destroy(hitEffect, 2f);

        // GameObject hitMark = Instantiate(impactMarkPrefab, aimPoint + (aimNormal * 0.01f), Quaternion.LookRotation(-aimNormal));
        // Destroy(hitMark, 5f);

        GameObject proj = Instantiate(projectilePrefab, muzzleTransform.position, Quaternion.LookRotation(aimPoint - muzzleTransform.position));
        proj.GetComponent<Projectile>().ShootTarget(aimPoint);
      } else {
        GameObject proj = Instantiate(projectilePrefab, muzzleTransform.position, Quaternion.LookRotation(aimPoint - muzzleTransform.position));
        proj.GetComponent<Projectile>().ShootMiss(aimPoint - muzzleTransform.position);
      }
    }

    public float GetDamage(HitboxType hitboxType) {
      switch (hitboxType) {
        case HitboxType.Head:
          return damage.head;
        case HitboxType.Limb:
          return damage.limb;
        case HitboxType.Torso:
          return damage.torso;
        default:
          return 10;
      }
    }
  }
}