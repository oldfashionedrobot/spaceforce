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
    private Weapon weapon = null;

    private float nextShotTime = 0f;
    private readonly string recoilAnimName = "recoilAnim";
    private readonly string idleAnimName = "idleAnim";

    public bool Shoot() {
      if (Time.time < nextShotTime) {
        return false;
      } else {
        nextShotTime = Time.time + 1 / weapon.fireRate;

        if (weapon.shootType == WeaponShootType.HitScan) {
          // effects
          weapon.Shoot(equippedWeapon.GetMuzzlePoint(), aimTarget.point, aimTarget.normal);
          AudioSource.PlayClipAtPoint(weapon.fireClip, equippedWeapon.transform.position);
          ikArmAnimation.Play(recoilAnimName);

          // you hitting something
          if (aimTarget.collider != null) {
            Hitbox hit = aimTarget.collider.GetComponent<Hitbox>();
            if (hit) ApplyShootDamage(hit, weapon, aimTarget.point, (aimTarget.point - transform.position).normalized);
          }
        } else {
          // TODO: how to handle flying projectiles?
        }
        return true;
      }
    }

    public bool ApplyShootDamage(Hitbox hit, Weapon weapon, Vector3 hitPoint, Vector3 hitDirection) {
      Health target = hit.GetComponentInParent<Health>();
      float damage = weapon.GetDamage(hit.type);

      target.TakeDamage(hit, hitPoint, hitDirection, damage);

      return true;
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
        aimTarget.point = target;
        // don't care about normal just want a direction to shoot at
      }
    }

    public bool EquipWeapon(int weaponNum, out Transform leftHandIKTarget, out Transform leftElbowIKTarget, out int animId) {
      //TODO check if weapon var exists, then go ahead and equip it

      weapon = arsenal.weapons[weaponNum - 1];
      equippedWeapon = weapon.SetUpWeapon(rHand);

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
  }
}