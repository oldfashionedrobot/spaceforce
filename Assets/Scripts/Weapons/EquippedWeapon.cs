using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedWeapon : MonoBehaviour {
  [SerializeField]
  private Transform leftHandIKTarget;
  [SerializeField]
  private Transform leftElbowIKTarget;
  [SerializeField]
  private Transform muzzlePoint;

  public Transform GetLeftHandIKTarget() {
    return leftHandIKTarget;
  }

  public Transform GetLeftElbowIKTarget() {
    return leftElbowIKTarget;
  }

  public Transform GetMuzzlePoint() {
    return muzzlePoint;
  }
}