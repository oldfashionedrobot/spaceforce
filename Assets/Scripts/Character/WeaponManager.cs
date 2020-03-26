using System.Collections;
using System.Collections.Generic;
using SpaceForce.Combat;
using UnityEngine;

namespace SpaceForce.Character {
  [System.Serializable]
  public class Arsenal {
    public Weapon[] weapons;
  }

  public class WeaponManager : MonoBehaviour {
    [SerializeField]
    private Transform rHand;
    [SerializeField]
    private Transform lHand;
    [SerializeField]
    private Animation ikArmAnimation;
    [SerializeField]
    private Arsenal arsenal;

    private RaycastHit aimTarget;

    private EquippedWeapon equippedWeapon = null;
    private AudioSource weaponAudioSource = null;
    private Weapon weapon = null;
    private int ammo;

    private float nextShotTime = 0f;
    private readonly string recoilAnimName = "recoilAnim";
    private readonly string idleAnimName = "idleAnim";

    public int Shoot() {
      if (Time.time < nextShotTime) {
        return 0;
      } else if (ammo > 0) {
        nextShotTime = Time.time + 1 / weapon.fireRate;

        if (weapon.shootType == WeaponShootType.HitScan) {
          // effects
          weapon.Shoot(equippedWeapon.GetMuzzlePoint(), aimTarget.point, aimTarget.normal);
          AudioSource.PlayClipAtPoint(weapon.audio.fireClip, equippedWeapon.transform.position);
          ikArmAnimation.Play(recoilAnimName);

          // you hitting something
          if (aimTarget.collider != null) {
            Hitbox hit = aimTarget.collider.GetComponent<Hitbox>();
            if (hit) ApplyShootDamage(hit, weapon, aimTarget.point, (aimTarget.point - transform.position).normalized);
          }
        } else {
          // TODO: how to handle flying projectiles?
        }

        ammo -= 1;

        return 1;
      } else {
        return -1;
      }
    }

    public void Reload() {
      ammo = weapon.clipSize;
      weaponAudioSource.PlayOneShot(weapon.audio.reloadClip);
    }

    public void ApplyShootDamage(Hitbox hit, Weapon weapon, Vector3 hitPoint, Vector3 hitDirection) {
      Health target = hit.GetComponentInParent<Health>();
      float damage = weapon.GetDamage(hit.type);

      target.TakeDamage(hit, hitPoint, hitDirection, damage);
    }

    public void PerformAimRaycast(Vector3 target, LayerMask shootCheckLayer) {
      Vector3 muzzlePoint = equippedWeapon.GetMuzzlePoint().position;
      Vector3 rayDir = target - muzzlePoint;
      if (Physics.Raycast(muzzlePoint, rayDir, out aimTarget, rayDir.magnitude + 5f, shootCheckLayer)) {
        if (aimTarget.point != target) {
          // blocked
        } else {
          // direct hit
        }
      } else {
        aimTarget = new RaycastHit();
        aimTarget.point = muzzlePoint + rayDir.normalized * 500f;
        // don't care about normal just want a direction to shoot at
      }
    }

    public bool EquipWeapon(int weaponNum, out Transform leftHandIKTarget, out Transform leftElbowIKTarget, out int animId) {
      //TODO check if weapon var exists, then go ahead and equip it

      weapon = arsenal.weapons[weaponNum - 1];
      equippedWeapon = weapon.SetUpWeapon(rHand);
      weaponAudioSource = equippedWeapon.GetComponent<AudioSource>();

      // NOTE: arsenal can store more info about weapon state?
      ammo = weapon.clipSize;

      leftHandIKTarget = equippedWeapon.GetLeftHandIKTarget();
      leftElbowIKTarget = equippedWeapon.GetLeftElbowIKTarget();
      animId = weapon.animId;

      ikArmAnimation.AddClip(weapon.animations.recoilAnimation, recoilAnimName);
      ikArmAnimation.AddClip(weapon.animations.idleAnimation, idleAnimName);

      ikArmAnimation.Play(idleAnimName);

      // NOTE: could check here for reasons why cannot equip
      return true;
    }

    public bool UnequipWeapon() {
      // TODO: just inactivate the old weapon
      Destroy(equippedWeapon.gameObject);
      ikArmAnimation.RemoveClip(recoilAnimName);
      ikArmAnimation.RemoveClip(idleAnimName);

      // NOTE: could check here for reasons why cannot unequip
      return true;
    }

    public bool IsAutomaticWeapon() {
      return weapon != null && weapon.isAutomatic;
    }

    public int GetCurrentAmmo() {
      return ammo;
    }

    public int GetClipSize() {
      if (weapon != null)
        return weapon.clipSize;
      else return 0;
    }

    public bool CanReload() {
      return ammo < weapon.clipSize;
    }
  }
}