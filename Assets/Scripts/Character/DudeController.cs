using System.Collections;
using System.Collections.Generic;
using SpaceForce.Combat;
using UnityEngine;

namespace SpaceForce.Character {

  public class DudeController : MonoBehaviour {
    protected Animator anim;
    protected CharacterController controller;
    protected CharacterIKManager ikManager;
    protected WeaponManager weaponManager;
    protected HitboxManager hitboxManager;

    protected bool isEquipped = false;
    protected bool isAiming = false;
    protected bool isDead = false;

    protected void Awake() {
      anim = GetComponent<Animator>();
      controller = GetComponent<CharacterController>();
      ikManager = GetComponent<CharacterIKManager>();
      weaponManager = GetComponent<WeaponManager>();
      hitboxManager = GetComponent<HitboxManager>();

    }

    public void Die(Hitbox hit, Vector3 hitPoint, Vector3 hitDirection) {
      isDead = true;
      anim.enabled = false;
      hitboxManager.TurnOnRagdoll();
      ApplyRagdollForce(hit, hitPoint, hitDirection);
    }

    public void ApplyRagdollForce(Hitbox hit, Vector3 hitPoint, Vector3 hitDirection) {
      hit.GetComponent<Rigidbody>().AddForceAtPosition(hitDirection * 1000f, hitPoint);
    }

    protected void EquipWeapon(int weaponNum) {
      Transform leftHandIK;
      Transform leftElbowIK;
      int weaponAnimId;
      if (weaponManager.EquipWeapon(weaponNum, out leftHandIK, out leftElbowIK, out weaponAnimId)) {
        isEquipped = true;
        anim.SetFloat("weaponId", weaponAnimId);
        anim.SetBool("weaponEquipped", true);
        ikManager.SetLeftArmIK(leftHandIK, leftElbowIK);
      }
    }

    protected void TriggerAim() {
      anim.SetBool("weaponAim", true);
      ikManager.ToggleAim(true);
      isAiming = true;
    }

    protected void EndAim() {
      anim.SetBool("weaponAim", false);
      ikManager.ToggleAim(false);
      isAiming = false;
    }
  }
}