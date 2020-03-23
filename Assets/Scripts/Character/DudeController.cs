using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DudeController : MonoBehaviour {
  protected Animator anim;
  protected CharacterController controller;
  protected CharacterIKManager ikManager;
  protected WeaponManager weaponManager;
  protected HitboxManager hitboxManager;

  protected void Awake() {
    anim = GetComponent<Animator>();
    controller = GetComponent<CharacterController>();
    ikManager = GetComponent<CharacterIKManager>();
    weaponManager = GetComponent<WeaponManager>();
    hitboxManager = GetComponent<HitboxManager>();

  }

  public void Die(Hitbox hit, Vector3 hitPoint, Vector3 hitDirection) {
    anim.enabled = false;
    hitboxManager.TurnOnRagdoll();
    hit.GetComponent<Rigidbody>().AddForceAtPosition(hitDirection * 5000f, hitPoint);
  }
}